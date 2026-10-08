export interface AuthUser {
  id: number
  fullName: string
  email: string
  phone: string | null
  role: 'user' | 'admin'
}

/** Respuesta de register, login y refresh (sin envoltorio `data`). `expiresIn` va en segundos. */
export interface AuthResponse {
  accessToken: string
  refreshToken: string
  tokenType: string
  expiresIn: number
  user: AuthUser
}

export interface LoginBody {
  email: string
  password: string
}

export interface RegisterBody {
  fullName: string
  email: string
  password: string
  phone: string | null
}