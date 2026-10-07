import type { LoanStatus } from '../../../../api/types/loan'
import { Button } from '../../../../shared/components/Button/Button'

interface LoanButtonProps {
  status?: LoanStatus
  loading?: boolean
  hasError?: boolean
  onAction?: (status: LoanStatus) => void
}

interface ButtonState {
  label: string
  disabled: boolean
}

const UNAVAILABLE: ButtonState = { label: 'Unavailable', disabled: true }

const STATES: Record<LoanStatus, ButtonState> = {
  LoginRequired: { label: 'Log in to borrow', disabled: false },
  Available: { label: 'Borrow', disabled: false },
  BorrowedByYou: { label: 'Deliver', disabled: false },
  Unavailable: UNAVAILABLE,
}

function getState(status: LoanStatus | undefined, loading: boolean, hasError: boolean): ButtonState {
  if (loading) return { label: 'Loading…', disabled: true }
  if (hasError || !status) return { label: 'Status unavailable', disabled: true }
  return STATES[status] ?? UNAVAILABLE
}

export function LoanButton({ status, loading = false, hasError = false, onAction }: LoanButtonProps) {
  const state = getState(status, loading, hasError)

  return (
    <Button
      block
      disabled={state.disabled}
      aria-busy={loading}
      onClick={() => status && onAction?.(status)}
    >
      {state.label}
    </Button>
  )
}