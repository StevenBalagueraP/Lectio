const ACCESS_TOKEN_KEY = 'lectio:accessToken'

export function getAccessToken(): string | null {
  try {
    return localStorage.getItem(ACCESS_TOKEN_KEY)
  } catch {
    return null
  }
}