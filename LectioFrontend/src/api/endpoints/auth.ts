import { http } from '../http'
import type { AuthResponse, LoginBody, RegisterBody } from '../types/auth'

const AUTH_PATH = '/api/auth'

export function login(body: LoginBody, signal?: AbortSignal) {
  return http<AuthResponse>(`${AUTH_PATH}/login`, signal, { method: 'POST', body })
}

export function register(body: RegisterBody, signal?: AbortSignal) {
  return http<AuthResponse>(`${AUTH_PATH}/register`, signal, { method: 'POST', body })
}

export function logout(refreshToken: string, signal?: AbortSignal) {
  return http<void>(`${AUTH_PATH}/logout`, signal, { method: 'POST', body: { refreshToken } })
}