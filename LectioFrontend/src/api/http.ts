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
  auth?: boolean
  method?: 'GET' | 'POST'
  body?: unknown
}

interface SendOptions {
  method: 'GET' | 'POST'
  body?: unknown
  token: string | null
}

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
    if (current !== used) return true 

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
      if (res.status === 400 || res.status === 401) clearSession()
      return false
    } catch {
      return false
    }
  }

  return navigator.locks ? navigator.locks.request('lectio:refresh', run) : run()
}

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

  if (res.status === 401 && auth && getRefreshToken() !== null && (await refreshTokens())) {
    res = await send(path, signal, { method, body, token: getAccessToken() })
  }

  if (!res.ok) throw new ApiError(res.status, path, await readErrorDetail(res))
  if (res.status === 204) return undefined as T // logout responde sin cuerpo
  return res.json() as Promise<T>
}