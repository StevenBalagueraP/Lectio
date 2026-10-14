import type { AuthResponse, AuthUser } from './types/auth'

const ACCESS_TOKEN_KEY = 'lectio:accessToken'
const REFRESH_TOKEN_KEY = 'lectio:refreshToken'
const EXPIRES_AT_KEY = 'lectio:accessTokenExpiresAt'
const USER_KEY = 'lectio:user'

const EXPIRY_MARGIN_MS = 30 * 1000

const listeners = new Set<() => void>()

function read(key: string): string | null {
  try {
    return localStorage.getItem(key)
  } catch {
    return null
  }
}

function write(key: string, value: string | null) {
  try {
    if (value === null) localStorage.removeItem(key)
    else localStorage.setItem(key, value)
  } catch {
    // Sin localStorage la sesión simplemente no persiste
  }
}

function notify() {
  listeners.forEach((listener) => listener())
}

export function getAccessToken(): string | null {
  return read(ACCESS_TOKEN_KEY)
}

export function getRefreshToken(): string | null {
  return read(REFRESH_TOKEN_KEY)
}

export function isAccessTokenExpired(): boolean {
  const expiresAt = Number(read(EXPIRES_AT_KEY))
  return expiresAt > 0 && Date.now() >= expiresAt - EXPIRY_MARGIN_MS
}

export function saveSession(auth: AuthResponse) {
  write(ACCESS_TOKEN_KEY, auth.accessToken)
  write(REFRESH_TOKEN_KEY, auth.refreshToken)
  write(EXPIRES_AT_KEY, String(Date.now() + auth.expiresIn * 1000))
  write(USER_KEY, JSON.stringify(auth.user))
  notify()
}

export function clearSession() {
  write(ACCESS_TOKEN_KEY, null)
  write(REFRESH_TOKEN_KEY, null)
  write(EXPIRES_AT_KEY, null)
  write(USER_KEY, null)
  notify()
}

let cachedRaw: string | null = null
let cachedUser: AuthUser | null = null

export function getStoredUser(): AuthUser | null {
  const raw = read(USER_KEY)
  if (raw !== cachedRaw) {
    cachedRaw = raw
    cachedUser = parseUser(raw)
  }
  return cachedUser
}

function parseUser(raw: string | null): AuthUser | null {
  if (!raw) return null
  try {
    return JSON.parse(raw) as AuthUser
  } catch {
    return null
  }
}

export function subscribeSession(listener: () => void) {
  listeners.add(listener)
  window.addEventListener('storage', listener)

  return () => {
    listeners.delete(listener)
    window.removeEventListener('storage', listener)
  }
}