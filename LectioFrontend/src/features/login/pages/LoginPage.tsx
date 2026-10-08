import { useState, type ChangeEvent, type FormEvent } from 'react'
import { Link, Navigate, useLocation } from 'react-router'
import { login } from '../../../api/endpoints/auth'
import { saveSession } from '../../../api/token'
import { AuthForm } from '../../../shared/components/AuthForm/AuthForm'
import { TextField } from '../../../shared/components/TextField/TextField'
import { useSession } from '../../../shared/hooks/useSession'
import { getApiErrorMessage } from '../../../shared/utils/apiError'
import { getRedirectTarget } from '../../../shared/utils/redirect'
import { validateLogin, type LoginErrors, type LoginValues } from '../utils'

export function LoginPage() {
  const user = useSession()
  const location = useLocation()

  const [values, setValues] = useState<LoginValues>({ email: '', password: '' })
  const [errors, setErrors] = useState<LoginErrors>({})
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  // Con sesión (también justo después de iniciarla) se vuelve a donde venía el usuario
  if (user) return <Navigate to={getRedirectTarget(location.state)} replace />

  const handleChange = (field: keyof LoginValues) => (event: ChangeEvent<HTMLInputElement>) => {
    setValues((current) => ({ ...current, [field]: event.target.value }))
    setErrors((current) => ({ ...current, [field]: undefined }))
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (submitting) return

    const nextErrors = validateLogin(values)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setSubmitting(true)
    setSubmitError(null)
    try {
      const auth = await login({ email: values.email.trim(), password: values.password })
      saveSession(auth) // useSession cambia y el <Navigate> de arriba redirige
    } catch (error) {
      setSubmitError(getApiErrorMessage(error, 'Something went wrong. Please try again.'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <AuthForm
      title="Log in"
      subtitle="Welcome back! Sign in to borrow books and leave reviews."
      submitLabel="Log in"
      submittingLabel="Logging in…"
      submitting={submitting}
      error={submitError}
      onSubmit={handleSubmit}
      footer={
        <>
          Don't have an account?{' '}
          <Link to="/register" state={location.state}>
            Sign up
          </Link>
        </>
      }
    >
      <TextField
        label="Email"
        type="email"
        autoComplete="email"
        value={values.email}
        error={errors.email}
        onChange={handleChange('email')}
      />
      <TextField
        label="Password"
        type="password"
        autoComplete="current-password"
        value={values.password}
        error={errors.password}
        onChange={handleChange('password')}
      />
    </AuthForm>
  )
}