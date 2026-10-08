import { http } from '../http'
import type { AuthResponse, LoginBody, RegisterBody } from '../types/auth'

const AUTH_PATH = '/api/auth'

// La renovación (POST /api/auth/refresh) la hace `http` por su cuenta; no se llama desde las pantallas.

export function login(body: LoginBody, signal?: AbortSignal) {
  return http<AuthResponse>(`${AUTH_PATH}/login`, signal, { method: 'POST', body })
}

export function register(body: RegisterBody, signal?: AbortSignal) {
  return http<AuthResponse>(`${AUTH_PATH}/register`, signal, { method: 'POST', body })
}

// 204 sin cuerpo; el access token ya emitido no se invalida en el servidor, hay que descartarlo en el cliente
export function logout(refreshToken: string, signal?: AbortSignal) {
  return http<void>(`${AUTH_PATH}/logout`, signal, { method: 'POST', body: { refreshToken } })
}