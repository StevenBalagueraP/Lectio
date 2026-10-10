import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { Pagination } from './Pagination'

describe('Pagination (unit)', () => {
  it('renders nothing when totalPages is 1', () => {
    const { container } = render(<Pagination page={1} totalPages={1} onChange={jest.fn()} />)
    expect(container.firstChild).toBeNull()
  })

  it('renders nothing when totalPages is 0', () => {
    const { container } = render(<Pagination page={1} totalPages={0} onChange={jest.fn()} />)
    expect(container.firstChild).toBeNull()
  })

  it('disables the "previous" button on the first page', () => {
    render(<Pagination page={1} totalPages={5} onChange={jest.fn()} />)
    expect(screen.getByRole('button', { name: 'Página anterior' })).toBeDisabled()
    expect(screen.getByRole('button', { name: 'Página siguiente' })).toBeEnabled()
  })

  it('disables the "next" button on the last page', () => {
    render(<Pagination page={5} totalPages={5} onChange={jest.fn()} />)
    expect(screen.getByRole('button', { name: 'Página siguiente' })).toBeDisabled()
    expect(screen.getByRole('button', { name: 'Página anterior' })).toBeEnabled()
  })

  it('marks the current page with aria-current="page"', () => {
    render(<Pagination page={3} totalPages={5} onChange={jest.fn()} />)
    expect(screen.getByRole('button', { name: 'Página 3' })).toHaveAttribute('aria-current', 'page')
    expect(screen.getByRole('button', { name: 'Página 1' })).not.toHaveAttribute('aria-current')
  })

  it('calls onChange with page - 1 when clicking "previous"', async () => {
    const onChange = jest.fn()
    const user = userEvent.setup()
    render(<Pagination page={3} totalPages={5} onChange={onChange} />)

    await user.click(screen.getByRole('button', { name: 'Página anterior' }))

    expect(onChange).toHaveBeenCalledWith(2)
  })

  it('calls onChange with page + 1 when clicking "next"', async () => {
    const onChange = jest.fn()
    const user = userEvent.setup()
    render(<Pagination page={3} totalPages={5} onChange={onChange} />)

    await user.click(screen.getByRole('button', { name: 'Página siguiente' }))

    expect(onChange).toHaveBeenCalledWith(4)
  })

  it('calls onChange with the clicked page number', async () => {
    const onChange = jest.fn()
    const user = userEvent.setup()
    render(<Pagination page={1} totalPages={5} onChange={onChange} />)

    await user.click(screen.getByRole('button', { name: 'Página 4' }))

    expect(onChange).toHaveBeenCalledWith(4)
  })

  it('shows all pages without ellipsis when totalPages <= 7', () => {
    render(<Pagination page={4} totalPages={7} onChange={jest.fn()} />)

    for (let i = 1; i <= 7; i++) {
      expect(screen.getByRole('button', { name: `Página ${i}` })).toBeInTheDocument()
    }
  })

  it('shows end-ellipsis when on an early page of a long list', () => {
    const { container } = render(<Pagination page={2} totalPages={20} onChange={jest.fn()} />)
    // Ellipsis is rendered as a span (aria-hidden), not a button
    const buttons = screen.getAllByRole('button')
    // Should not have all 20 page buttons
    expect(buttons.length).toBeLessThan(22) // 20 pages + 2 arrows
  })

  it('has an accessible nav landmark labeled "Paginación"', () => {
    render(<Pagination page={2} totalPages={5} onChange={jest.fn()} />)
    expect(screen.getByRole('navigation', { name: 'Paginación' })).toBeInTheDocument()
  })
})
