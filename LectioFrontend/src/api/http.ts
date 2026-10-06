import { env } from '../config/env'
import { getAccessToken } from './token'

interface HttpOptions {
  auth?: boolean
}

export async function http<T>(
  path: string,
  signal?: AbortSignal,
  { auth = false }: HttpOptions = {},
): Promise<T> {
  const token = auth ? getAccessToken() : null

  const res = await fetch(`${env.apiUrl}${path}`, {
    signal,
    headers: token ? { Authorization: `Bearer ${token}` } : undefined,
  })
  if (!res.ok) throw new Error(`Error ${res.status} al consultar ${path}`)
  return res.json() as Promise<T>
}