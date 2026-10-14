import { useState, type ChangeEvent, type SubmitEvent } from 'react'
import { Link, Navigate, useLocation } from 'react-router'
import { register } from '../../../api/endpoints/auth'
import { saveSession } from '../../../api/token'
import { AuthForm } from '../../../shared/components/AuthForm/AuthForm'
import { TextField } from '../../../shared/components/TextField/TextField'
import { useSession } from '../../../shared/hooks/useSession'
import { getApiErrorMessage } from '../../../shared/utils/apiError'
import { getRedirectTarget } from '../../../shared/utils/redirect'
import { validateRegister, type RegisterErrors, type RegisterValues } from '../utils'

export function RegisterPage() {
  const user = useSession()
  const location = useLocation()

  const [values, setValues] = useState<RegisterValues>({
    fullName: '',
    email: '',
    password: '',
    phone: '',
  })
  const [errors, setErrors] = useState<RegisterErrors>({})
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  if (user) return <Navigate to={getRedirectTarget(location.state)} replace />

  const handleChange = (field: keyof RegisterValues) => (event: ChangeEvent<HTMLInputElement>) => {
    setValues((current) => ({ ...current, [field]: event.target.value }))
    setErrors((current) => ({ ...current, [field]: undefined }))
  }

  const handleSubmit = async (event: SubmitEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (submitting) return

    const nextErrors = validateRegister(values)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setSubmitting(true)
    setSubmitError(null)
    try {
      const auth = await register({
        fullName: values.fullName.trim(),
        email: values.email.trim(),
        password: values.password,
        phone: values.phone.trim() || null,
      })
      saveSession(auth) 
    } catch (error) {
      setSubmitError(getApiErrorMessage(error, 'Something went wrong. Please try again.'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <AuthForm
      title="Create account"
      subtitle="Join Lectio to borrow books and share your reviews."
      submitLabel="Sign up"
      submittingLabel="Creating account…"
      submitting={submitting}
      error={submitError}
      onSubmit={handleSubmit}
      footer={
        <>
          Already have an account?{' '}
          <Link to="/login" state={location.state}>
            Log in
          </Link>
        </>
      }
    >
      <TextField
        label="Full name"
        autoComplete="name"
        value={values.fullName}
        error={errors.fullName}
        onChange={handleChange('fullName')}
      />
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
        autoComplete="new-password"
        hint="At least 8 characters, with a letter and a number."
        value={values.password}
        error={errors.password}
        onChange={handleChange('password')}
      />
      <TextField
        label="Phone (optional)"
        type="tel"
        autoComplete="tel"
        maxLength={20}
        value={values.phone}
        error={errors.phone}
        onChange={handleChange('phone')}
      />
    </AuthForm>
  )
}