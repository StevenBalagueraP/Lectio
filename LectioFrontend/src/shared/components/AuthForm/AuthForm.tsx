import type { FormEvent, ReactNode } from 'react'
import { Button } from '../Button/Button'
import styles from './AuthForm.module.css'

interface AuthFormProps {
  title: string
  subtitle: string
  submitLabel: string
  submittingLabel: string
  submitting: boolean
  /** Error del servidor (o de red) para mostrar sobre el botón. */
  error: string | null
  onSubmit: (event: FormEvent<HTMLFormElement>) => void
  /** Enlace a la otra pantalla ("¿No tienes cuenta?"). */
  footer: ReactNode
  /** Los campos (`TextField`). */
  children: ReactNode
}

export function AuthForm({
  title,
  subtitle,
  submitLabel,
  submittingLabel,
  submitting,
  error,
  onSubmit,
  footer,
  children,
}: AuthFormProps) {
  return (
    <main className={styles.main}>
      <div className={styles.card}>
        <h1 className={styles.title}>{title}</h1>
        <p className={styles.subtitle}>{subtitle}</p>

        <form className={styles.form} onSubmit={onSubmit} noValidate>
          {children}

          {error && (
            <p role="alert" className={styles.error}>
              {error}
            </p>
          )}

          <div className={styles.actions}>
            <Button type="submit" block disabled={submitting}>
              {submitting ? submittingLabel : submitLabel}
            </Button>
          </div>
        </form>

        <p className={styles.footer}>{footer}</p>
      </div>
    </main>
  )
}