import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import type { BookSummary } from '../../../api/types/book'
import { BookGrid } from './BookGrid'

const books: BookSummary[] = [
  {
    id: 1,
    title: 'Clean Code',
    imageUrl: 'http://covers.test/clean-code.jpg',
    rating: { stars: 4.5 },
    authors: [{ id: 10, name: 'Robert C. Martin' }],
    categories: [{ id: 1, name: 'Programming' }],
  },
  {
    id: 2,
    title: 'Refactoring',
    imageUrl: null,
    rating: null,
    authors: [
      { id: 11, name: 'Martin Fowler' },
      { id: 12, name: 'Kent Beck' },
    ],
    categories: [],
  },
]

describe('BookGrid', () => {
  it('shows only the error message (and no books) when there is an error', () => {
    render(<BookGrid books={books} loading={false} error="Error 500 al consultar /api/books" />)

    expect(screen.getByRole('alert')).toHaveTextContent('Error 500 al consultar /api/books')
    expect(screen.queryByText('Clean Code')).not.toBeInTheDocument()
  })

  it('shows 4 skeletons by default while loading without data', () => {
    const { container } = render(<BookGrid loading />)

    expect(container.querySelectorAll('.skeleton')).toHaveLength(4)
    expect(container.firstChild).toHaveAttribute('aria-busy', 'true')
  })

  it('respects skeletonCount', () => {
    const { container } = render(<BookGrid loading skeletonCount={2} />)

    expect(container.querySelectorAll('.skeleton')).toHaveLength(2)
  })

  it('shows the default empty message when there are no books', () => {
    render(<BookGrid books={[]} loading={false} />)

    expect(screen.getByText('No hay libros todavía.')).toBeInTheDocument()
  })

  it('shows a custom empty message', () => {
    render(<BookGrid books={[]} loading={false} emptyMessage="No books found." />)

    expect(screen.getByText('No books found.')).toBeInTheDocument()
  })

  it('renders one card per book', () => {
    render(<BookGrid books={books} loading={false} />)

    expect(screen.getAllByRole('article')).toHaveLength(2)
    expect(screen.getByRole('img', { name: 'Portada de Clean Code' })).toBeInTheDocument()
    expect(screen.getByText('Robert C. Martin')).toBeInTheDocument()
    expect(screen.getByText('Martin Fowler +1')).toBeInTheDocument()
  })

  it('upgrades cover URLs from http to https', () => {
    render(<BookGrid books={books} loading={false} />)

    expect(screen.getByRole('img', { name: 'Portada de Clean Code' })).toHaveAttribute(
      'src',
      'https://covers.test/clean-code.jpg',
    )
  })

  it('calls onOpen with the book id when "See More" is pressed', async () => {
    const user = userEvent.setup()
    const onOpen = jest.fn()
    render(<BookGrid books={books} loading={false} onOpen={onOpen} />)

    await user.click(screen.getByRole('button', { name: 'Ver detalle de Refactoring' }))

    expect(onOpen).toHaveBeenCalledWith(2)
  })

  it('keeps showing previous books (dimmed) while a new page is loading', () => {
    const { container } = render(<BookGrid books={books} loading />)

    expect(screen.getAllByRole('article')).toHaveLength(2)
    expect(container.firstChild).toHaveClass('stale')
    expect(container.firstChild).toHaveAttribute('aria-busy', 'true')
  })
})