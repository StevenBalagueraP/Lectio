import type { ComponentProps } from 'react'
import { fireEvent, render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ReviewForm } from './ReviewForm'

function setup(props: Partial<ComponentProps<typeof ReviewForm>> = {}) {
  const onSubmit = jest.fn()
  const onCancel = jest.fn()
  const user = userEvent.setup()

  render(<ReviewForm submitting={false} error={null} onSubmit={onSubmit} onCancel={onCancel} {...props} />)

  return { user, onSubmit, onCancel }
}

const star = (n: number) => screen.getByRole('radio', { name: `${n} de 5 estrellas` })

describe('ReviewForm', () => {
  it('starts with no rating selected and Submit disabled', () => {
    setup()

    expect(screen.getAllByRole('radio')).toHaveLength(5)
    screen.getAllByRole('radio').forEach((radio) => expect(radio).toHaveAttribute('aria-checked', 'false'))
    expect(screen.getByRole('button', { name: 'Submit' })).toBeDisabled()
  })

  it('selects a rating and enables Submit', async () => {
    const { user } = setup()

    await user.click(star(4))

    expect(star(4)).toHaveAttribute('aria-checked', 'true')
    expect(star(3)).toHaveAttribute('aria-checked', 'false')
    expect(screen.getByRole('button', { name: 'Submit' })).toBeEnabled()
  })

  it('submits the stars and the trimmed comment', async () => {
    const { user, onSubmit } = setup()

    await user.click(star(5))
    await user.type(screen.getByLabelText('Comment (optional)'), '   Great book!  ')
    await user.click(screen.getByRole('button', { name: 'Submit' }))

    expect(onSubmit).toHaveBeenCalledTimes(1)
    expect(onSubmit).toHaveBeenCalledWith({ stars: 5, comment: 'Great book!' })
  })

  it('allows an empty comment (it is optional)', async () => {
    const { user, onSubmit } = setup()

    await user.click(star(3))
    await user.click(screen.getByRole('button', { name: 'Submit' }))

    expect(onSubmit).toHaveBeenCalledWith({ stars: 3, comment: '' })
  })

  it('does not submit without a rating', async () => {
    const { user, onSubmit } = setup()

    await user.type(screen.getByLabelText('Comment (optional)'), 'Only text')
    await user.click(screen.getByRole('button', { name: 'Submit' }))

    expect(onSubmit).not.toHaveBeenCalled()
  })

  it('ignores a submit event fired without a rating (guard against bypassing the disabled button)', () => {
    const { onSubmit } = setup()

    fireEvent.submit(screen.getByRole('button', { name: 'Submit' }).closest('form')!)

    expect(onSubmit).not.toHaveBeenCalled()
  })

  it('ignores a submit event fired while a request is already in progress', async () => {
    const { user, onSubmit } = setup({ submitting: true })

    await user.click(star(5))
    fireEvent.submit(screen.getByRole('button', { name: 'Sending…' }).closest('form')!)

    expect(onSubmit).not.toHaveBeenCalled()
  })

  it('updates the character counter and limits the comment to 1000 characters', async () => {
    const { user } = setup()
    const textarea = screen.getByLabelText('Comment (optional)')

    expect(screen.getByText('0/1000')).toBeInTheDocument()
    await user.type(textarea, 'Hola')

    expect(screen.getByText('4/1000')).toBeInTheDocument()
    expect(textarea).toHaveAttribute('maxLength', '1000')
  })

  it('disables the buttons and shows "Sending…" while submitting', async () => {
    const { user, onSubmit } = setup({ submitting: true })

    expect(screen.getByRole('button', { name: 'Sending…' })).toBeDisabled()
    expect(screen.getByRole('button', { name: 'Cancel' })).toBeDisabled()

    await user.click(star(5))
    expect(screen.getByRole('button', { name: 'Sending…' })).toBeDisabled()
    expect(onSubmit).not.toHaveBeenCalled()
  })

  it('shows the error message received by props', () => {
    setup({ error: 'Your session has expired. Please log in again.' })

    expect(screen.getByRole('alert')).toHaveTextContent('Your session has expired. Please log in again.')
  })

  it('does not render an alert when there is no error', () => {
    setup()

    expect(screen.queryByRole('alert')).not.toBeInTheDocument()
  })

  it('calls onCancel when Cancel is pressed', async () => {
    const { user, onCancel, onSubmit } = setup()

    await user.click(screen.getByRole('button', { name: 'Cancel' }))

    expect(onCancel).toHaveBeenCalledTimes(1)
    expect(onSubmit).not.toHaveBeenCalled()
  })
})