import { isValidEmail } from '../../shared/utils/validation'
import { validateLogin } from '../../features/login/utils'

describe('isValidEmail (unit)', () => {
  it('returns true for a standard email address', () => {
    expect(isValidEmail('user@example.com')).toBe(true)
  })

  it('returns true for an email with subdomains', () => {
    expect(isValidEmail('steven@mail.internal.corp')).toBe(true)
  })

  it('returns true for an email with plus addressing', () => {
    expect(isValidEmail('user+tag@example.org')).toBe(true)
  })

  it('returns false for an address missing the @', () => {
    expect(isValidEmail('notanemail')).toBe(false)
  })

  it('returns false for an address missing the domain', () => {
    expect(isValidEmail('user@')).toBe(false)
  })

  it('returns false for an address missing the local part', () => {
    expect(isValidEmail('@domain.com')).toBe(false)
  })

  it('returns false for an empty string', () => {
    expect(isValidEmail('')).toBe(false)
  })

  it('ignores leading/trailing whitespace when validating', () => {
    expect(isValidEmail('  user@example.com  ')).toBe(true)
  })
})


describe('validateLogin (unit)', () => {
  it('returns an empty object for valid credentials', () => {
    expect(validateLogin({ email: 'valid@email.com', password: 'secret' })).toEqual({})
  })

  it('reports a required error when email is empty', () => {
    const errors = validateLogin({ email: '', password: 'secret' })
    expect(errors.email).toBe('Email is required.')
    expect(errors.password).toBeUndefined()
  })

  it('reports an invalid error when email is malformed', () => {
    const errors = validateLogin({ email: 'not-an-email', password: 'secret' })
    expect(errors.email).toBe('Enter a valid email address.')
  })

  it('reports a required error when password is empty', () => {
    const errors = validateLogin({ email: 'valid@email.com', password: '' })
    expect(errors.password).toBe('Password is required.')
    expect(errors.email).toBeUndefined()
  })

  it('reports both errors when email and password are empty', () => {
    const errors = validateLogin({ email: '', password: '' })
    expect(errors.email).toBeDefined()
    expect(errors.password).toBeDefined()
  })

  it('accepts an email with leading/trailing spaces (trimmed internally by isValidEmail)', () => {
    const errors = validateLogin({ email: '  user@example.com  ', password: '1234' })
    expect(errors.email).toBeUndefined()
  })
})
