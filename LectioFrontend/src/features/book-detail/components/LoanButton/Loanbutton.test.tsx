import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import type { LoanStatus } from '../../../../api/types/loan'
import { LoanButton } from './LoanButton'

describe('LoanButton', () => {
  it.each<[LoanStatus, string, boolean]>([
    ['LoginRequired', 'Log in to borrow', false],
    ['Available', 'Borrow', false],
    ['BorrowedByYou', 'Deliver', false],
    ['Unavailable', 'Unavailable', true],
  ])('shows the right label and state for "%s"', (status, label, disabled) => {
    render(<LoanButton status={status} />)

    const button = screen.getByRole('button', { name: label })
    if (disabled) expect(button).toBeDisabled()
    else expect(button).toBeEnabled()
  })

  it('is disabled and marked as busy while loading', () => {
    render(<LoanButton status="Available" loading />)

    const button = screen.getByRole('button', { name: 'Loading…' })
    expect(button).toBeDisabled()
    expect(button).toHaveAttribute('aria-busy', 'true')
  })

  it('shows "Status unavailable" when the request failed', () => {
    render(<LoanButton status="Available" hasError />)

    expect(screen.getByRole('button', { name: 'Status unavailable' })).toBeDisabled()
  })

  it('shows "Status unavailable" when there is no status yet', () => {
    render(<LoanButton />)

    expect(screen.getByRole('button', { name: 'Status unavailable' })).toBeDisabled()
  })

  it('calls onAction with the current status when clicked', async () => {
    const user = userEvent.setup()
    const onAction = jest.fn()
    render(<LoanButton status="LoginRequired" onAction={onAction} />)

    await user.click(screen.getByRole('button', { name: 'Log in to borrow' }))

    expect(onAction).toHaveBeenCalledTimes(1)
    expect(onAction).toHaveBeenCalledWith('LoginRequired')
  })

  it('does not call onAction when the button is disabled', async () => {
    const user = userEvent.setup()
    const onAction = jest.fn()
    render(<LoanButton status="Unavailable" onAction={onAction} />)

    await user.click(screen.getByRole('button', { name: 'Unavailable' }))

    expect(onAction).not.toHaveBeenCalled()
  })
})