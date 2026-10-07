import { env } from '../config/env'
import { getAccessToken } from './token'

interface HttpOptions {
  /** Envía `Authorization: Bearer <JWT>` si hay un token guardado. */
  auth?: boolean
  method?: 'GET' | 'POST'
  /** Se envía como JSON (agrega `Content-Type: application/json`). */
  body?: unknown
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

export async function http<T>(
  path: string,
  signal?: AbortSignal,
  { auth = false, method = 'GET', body }: HttpOptions = {},
): Promise<T> {
  const token = auth ? getAccessToken() : null

  const headers: Record<string, string> = {}
  if (token) headers.Authorization = `Bearer ${token}`
  if (body !== undefined) headers['Content-Type'] = 'application/json'

  const res = await fetch(`${env.apiUrl}${path}`, {
    method,
    signal,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })
  if (!res.ok) throw new ApiError(res.status, path, await readErrorDetail(res))
  return res.json() as Promise<T>
}