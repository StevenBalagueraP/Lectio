import { env } from '../config/env'

export async function http<T>(path: string, signal?: AbortSignal): Promise<T> {
  const res = await fetch(`${env.apiUrl}${path}`, { signal })
  if (!res.ok) throw new Error(`Error ${res.status} al consultar ${path}`)
  return res.json() as Promise<T>
}