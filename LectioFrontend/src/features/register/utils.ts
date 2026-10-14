import { isValidEmail } from '../../shared/utils/validation'

export interface RegisterValues {
  fullName: string
  email: string
  password: string
  phone: string
}

export type RegisterErrors = Partial<Record<keyof RegisterValues, string>>

export function validateRegister({ fullName, email, password }: RegisterValues): RegisterErrors {
  const errors: RegisterErrors = {}

  const name = fullName.trim()
  if (name.length < 2) errors.fullName = 'Enter your full name (at least 2 characters).'
  else if (name.length > 255) errors.fullName = 'Name must be at most 255 characters.'

  if (!email.trim()) errors.email = 'Email is required.'
  else if (!isValidEmail(email)) errors.email = 'Enter a valid email address.'
  else if (email.trim().length > 255) errors.email = 'Email must be at most 255 characters.'

  if (password.length < 8) errors.password = 'Password must be at least 8 characters.'
  else if (password.length > 72 || new TextEncoder().encode(password).length > 72) {
    errors.password = 'Password must be at most 72 characters.'
  } else if (!/\p{L}/u.test(password) || !/\d/.test(password)) {
    errors.password = 'Password must include at least one letter and one number.'
  }

  return errors
}