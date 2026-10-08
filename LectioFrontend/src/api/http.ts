import { env } from '../config/env'
import {
  clearSession,
  getAccessToken,
  getRefreshToken,
  isAccessTokenExpired,
  saveSession,
} from './token'
import type { AuthResponse } from './types/auth'

const REFRESH_PATH = '/api/auth/refresh'

interface HttpOptions {
  /** Envía `Authorization: Bearer <JWT>`; si el token venció (o responde 401) lo renueva con el refresh token. */
  auth?: boolean
  method?: 'GET' | 'POST'
  /** Se envía como JSON (agrega `Content-Type: application/json`). */
  body?: unknown
}

interface SendOptions {
  method: 'GET' | 'POST'
  body?: unknown
  token: string | null
}

/**
 * Error de una respuesta no-2xx. `message` conserva el texto genérico de siempre;
 * `serverMessage` es el `detail` que manda el backend (null en 401/403, que vienen sin cuerpo).
 */
export class ApiError extends Error {
  status: number
  serverMessage: string | null

  constructor(status: number, path: string, serverMessage: string | null) {
    super(`Error ${status} al consultar ${path}`)
    this.name = 'ApiError'
    this.status = status
    this.serverMessage = serverMessage
  }
}

// El backend responde problem+json ({ detail }) o ValidationProblemDetails ({ errors })
async function readErrorDetail(res: Response): Promise<string | null> {
  try {
    const body: unknown = await res.json()
    if (typeof body !== 'object' || body === null) return null

    const { detail, errors, title } = body as Record<string, unknown>
    if (typeof detail === 'string') return detail

    if (typeof errors === 'object' && errors !== null) {
      const first = Object.values(errors).flat().find((m) => typeof m === 'string')
      if (typeof first === 'string') return first
    }

    return typeof title === 'string' ? title : null
  } catch {
    return null
  }
}

function send(path: string, signal: AbortSignal | undefined, { method, body, token }: SendOptions) {
  const headers: Record<string, string> = {}
  if (token) headers.Authorization = `Bearer ${token}`
  if (body !== undefined) headers['Content-Type'] = 'application/json'

  return fetch(`${env.apiUrl}${path}`, {
    method,
    signal,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })
}

// --- Renovación de sesión -------------------------------------------------------------
// El refresh token es de un solo uso: si se gasta dos veces el backend cierra TODAS las
// sesiones del usuario. Por eso hay una sola renovación en curso (las demás esperan su
// resultado) y un lock entre pestañas.

let refreshing: Promise<boolean> | null = null

function refreshTokens(): Promise<boolean> {
  refreshing ??= doRefresh().finally(() => {
    refreshing = null
  })
  return refreshing
}

async function doRefresh(): Promise<boolean> {
  const used = getRefreshToken()
  if (used === null) return false

  const run = async () => {
    const current = getRefreshToken()
    if (current === null) return false
    if (current !== used) return true // otra pestaña ya lo renovó mientras esperábamos el lock

    try {
      const res = await send(REFRESH_PATH, undefined, {
        method: 'POST',
        body: { refreshToken: current },
        token: null,
      })

      if (res.ok) {
        saveSession((await res.json()) as AuthResponse)
        return true
      }
      // 400/401: el refresh token ya no sirve (vencido, usado o revocado) → sesión terminada
      if (res.status === 400 || res.status === 401) clearSession()
      return false
    } catch {
      return false // sin red: no se cierra la sesión, se puede reintentar luego
    }
  }

  return navigator.locks ? navigator.locks.request('lectio:refresh', run) : run()
}

// Antes de una petición con auth: si el access token venció (o falta) y hay refresh token, renueva
async function getValidAccessToken(): Promise<string | null> {
  const token = getAccessToken()
  if (getRefreshToken() !== null && (token === null || isAccessTokenExpired())) {
    await refreshTokens()
    return getAccessToken()
  }
  return token
}

export async function http<T>(
  path: string,
  signal?: AbortSignal,
  { auth = false, method = 'GET', body }: HttpOptions = {},
): Promise<T> {
  let res = await send(path, signal, {
    method,
    body,
    token: auth ? await getValidAccessToken() : null,
  })

  // 401 con auth: renovar una vez y reintentar una vez
  if (res.status === 401 && auth && getRefreshToken() !== null && (await refreshTokens())) {
    res = await send(path, signal, { method, body, token: getAccessToken() })
  }

  if (!res.ok) throw new ApiError(res.status, path, await readErrorDetail(res))
  if (res.status === 204) return undefined as T // logout responde sin cuerpo
  return res.json() as Promise<T>
}