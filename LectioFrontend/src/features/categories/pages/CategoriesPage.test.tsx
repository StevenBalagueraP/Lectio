import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router'
import type { BookSummary } from '../../../api/types/book'
import type { Category } from '../../../api/types/category'
import { emptyResponse, jsonResponse, mockFetch } from '../../../test/mockFetch'
import { CategoriesPage } from './CategoriesPage'


let pageCounter = 100 

const CATEGORIES: Category[] = [
  { id: 1, name: 'Fiction' },
  { id: 2, name: 'Science' },
  { id: 3, name: 'History' },
]

function makeBook(id: number, title: string): BookSummary {
  return {
    id,
    title,
    imageUrl: null,
    rating: { stars: 4 },
    authors: [{ id: 1, name: 'Author A' }],
    categories: [{ id: 1, name: 'Fiction' }],
  }
}

const pagedResponse = (books: BookSummary[], page = 1, totalPages = 1) => ({
  data: books,
  pagination: { page, pageSize: 10, totalCount: books.length, totalPages },
})

function mockApi({
  books = [makeBook(1, 'Clean Code'), makeBook(2, 'Refactoring')],
  categories = CATEGORIES,
  searchStatus = 200,
}: {
  books?: BookSummary[]
  categories?: Category[]
  searchStatus?: number
} = {}) {
  return mockFetch((url) => {
    const { pathname } = new URL(url)
    if (pathname === '/api/categories') return jsonResponse({ data: categories })
    if (pathname === '/api/books') {
      if (searchStatus !== 200) return emptyResponse(searchStatus)
      return jsonResponse(pagedResponse(books))
    }
    return emptyResponse(404)
  })
}

type FetchMock = ReturnType<typeof mockApi>

function booksUrls(fetchMock: FetchMock) {
  return fetchMock.mock.calls
    .map(([input]) => String(input))
    .filter((url) => url.includes('/api/books'))
}

function renderPage() {
  const page = pageCounter++
  const user = userEvent.setup()

  render(
    <MemoryRouter initialEntries={[`/categories?page=${page}`]}>
      <Routes>
        <Route path="/categories" element={<CategoriesPage />} />
        <Route path="/books/:bookId" element={<p>Book Detail</p>} />
      </Routes>
    </MemoryRouter>,
  )

  return { user }
}

describe('CategoriesPage (integration)', () => {
  it('renders the book list returned by the API on load', async () => {
    mockApi()
    renderPage()

    expect(await screen.findByText('Clean Code')).toBeInTheDocument()
    expect(screen.getByText('Refactoring')).toBeInTheDocument()
  })

  it('shows the book count returned by the backend', async () => {
    mockApi()
    renderPage()

    expect(await screen.findByText('2 books found')).toBeInTheDocument()
  })

  it('shows an error alert when the book search API responds with 500', async () => {
    mockApi({ searchStatus: 500 })
    renderPage()

    expect(await screen.findByRole('alert')).toBeInTheDocument()
  })

  it('shows the empty message when the API returns no books', async () => {
    mockApi({ books: [] })
    renderPage()

    expect(
      await screen.findByText('No books found. Try another search or remove some filters.'),
    ).toBeInTheDocument()
  })

  it('opens the category filter dropdown and lists all categories', async () => {
    mockApi()
    const { user } = renderPage()

    await screen.findByText('Clean Code')

    await user.click(screen.getByRole('button', { name: /filter by category/i }))

    expect(screen.getByRole('checkbox', { name: 'Fiction' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'Science' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'History' })).toBeInTheDocument()
  })

  it('opens the sort and filter dropdown and lists all options', async () => {
    mockApi()
    const { user } = renderPage()

    await screen.findByText('Clean Code')
    await user.click(screen.getByRole('button', { name: /sort and filter/i }))

    expect(screen.getByRole('checkbox', { name: 'Top rated' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'Available only' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'Newest first' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'Oldest first' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'Longest first' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'Shortest first' })).toBeInTheDocument()
  })

  it('sends the selected filters to the API in selection order', async () => {
    const fetchMock = mockApi()
    const { user } = renderPage()

    await screen.findByText('Clean Code')
    await user.click(screen.getByRole('button', { name: /sort and filter/i }))
    await user.click(screen.getByRole('checkbox', { name: 'Top rated' }))
    await user.click(screen.getByRole('checkbox', { name: 'Oldest first' }))

    await waitFor(() => {
      const urls = booksUrls(fetchMock)
      expect(urls[urls.length - 1]).toContain('filter=TopRated&filter=Oldest')
    })
  })

  it('replaces the opposite option instead of sending both', async () => {
    const fetchMock = mockApi()
    const { user } = renderPage()

    await screen.findByText('Clean Code')
    await user.click(screen.getByRole('button', { name: /sort and filter/i }))
    await user.click(screen.getByRole('checkbox', { name: 'Newest first' }))
    await user.click(screen.getByRole('checkbox', { name: 'Oldest first' }))

    await waitFor(() => {
      const urls = booksUrls(fetchMock)
      const last = urls[urls.length - 1]
      expect(last).toContain('filter=Oldest')
      expect(last).not.toContain('filter=Newest')
    })
  })

  it('navigates to the book detail page when "Ver detalle" is clicked', async () => {
    mockApi()
    const { user } = renderPage()

    await screen.findByText('Clean Code')
    await user.click(screen.getByRole('button', { name: 'Ver detalle de Clean Code' }))

    expect(await screen.findByText('Book Detail')).toBeInTheDocument()
  })

  it('shows the search input with the correct placeholder', () => {
    mockApi()
    renderPage()

    expect(screen.getByPlaceholderText('Type a book title…')).toBeInTheDocument()
  })
})