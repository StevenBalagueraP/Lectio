import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router'
import type { BookSummary } from '../../../api/types/book'
import type { Category } from '../../../api/types/category'
import { emptyResponse, jsonResponse, mockFetch } from '../../../test/mockFetch'
import { CategoriesPage } from './CategoriesPage'

// Prueba de integración: la página real + router real + hooks reales (useBookSearch, useFetch, cache, http).
// Solo se simula `fetch`.
//
// El cache de useFetch/useBookSearch vive en memoria a nivel de módulo.
// Cada prueba usa una querystring distinta (page=<n>) para evitar colisiones de clave.

let pageCounter = 100 // número de página alto para no colisionar con otros suites

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

/** Each test uses a unique starting page so the cache key is always fresh. */
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

    // Wait for the component to fully load first
    await screen.findByText('Clean Code')

    // The dropdown button has an accessible name from the "Filter by category" label
    await user.click(screen.getByRole('button', { name: /filter by category/i }))

    expect(screen.getByRole('checkbox', { name: 'Fiction' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'Science' })).toBeInTheDocument()
    expect(screen.getByRole('checkbox', { name: 'History' })).toBeInTheDocument()
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
