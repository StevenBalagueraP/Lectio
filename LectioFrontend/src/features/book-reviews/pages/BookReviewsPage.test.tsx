import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router'
import type { BookDetail } from '../../../api/types/book'
import type { BookRatings } from '../../../api/types/rating'
import { emptyResponse, jsonResponse, mockFetch } from '../../../test/mockFetch'
import { BookReviewsPage } from './BookReviewsPage'

let nextBookId = 200

const makeBook = (id: number): BookDetail => ({
  id,
  title: `Book ${id}`,
  imageUrl: null,
  description: 'A test book',
  pageCount: 100,
  publishedYear: 2021,
  publisher: 'Test Press',
  language: 'en',
  authors: [{ id: 1, name: 'Jane Doe' }],
  categories: [{ id: 1, name: 'Fiction' }],
})

const review = (idUser: number, fullName: string, rating: number, comment: string | null) => ({
  idUser,
  fullName,
  comment,
  rating,
  createdAt: '2025-06-14T00:00:00Z',
  updatedAt: '2025-06-14T00:00:00Z',
})

const ratingsPage: BookRatings = {
  averageRating: 4.5,
  numberOfRatings: 2,
  users: [review(1, 'Ana Torres', 5, 'Loved it'), review(2, 'Luis Pérez', 4, null)],
}

interface ApiOptions {
  userId?: number | null
  ratingsAfterPost?: BookRatings
  postResponse?: Response
}

function mockApi(bookId: number, { userId = 5, ratingsAfterPost, postResponse }: ApiOptions = {}) {
  let posted = false

  return mockFetch((url, init) => {
    const { pathname } = new URL(url)
    const method = init.method ?? 'GET'

    if (method === 'GET' && pathname === `/api/books/${bookId}`) return jsonResponse(makeBook(bookId))

    if (method === 'GET' && pathname === `/api/books/${bookId}/loan-status`) {
      return jsonResponse({
        bookId,
        userId,
        status: userId === null ? 'LoginRequired' : 'Available',
        loanId: null,
        dueDate: null,
      })
    }

    if (method === 'GET' && pathname === `/api/books/${bookId}/ratings`) {
      return jsonResponse(posted && ratingsAfterPost ? ratingsAfterPost : ratingsPage)
    }

    if (method === 'POST' && pathname === `/api/users/${userId}/ratings`) {
      posted = true
      return (
        postResponse ??
        jsonResponse({ message: 'Review saved successfully', ratingId: 9, bookId, userId, stars: 4 })
      )
    }

    return emptyResponse(404)
  })
}

function renderPage(bookId: number) {
  const user = userEvent.setup()

  render(
    <MemoryRouter initialEntries={[`/books/${bookId}/reviews`]}>
      <Routes>
        <Route path="/books/:bookId/reviews" element={<BookReviewsPage />} />
        <Route path="/login" element={<p>Login page</p>} />
      </Routes>
    </MemoryRouter>,
  )

  return { user }
}

const postCalls = (fetchMock: jest.Mock) => fetchMock.mock.calls.filter(([, init]) => init?.method === 'POST')

describe('BookReviewsPage (integration)', () => {
  it('loads the book, the average rating and the reviews', async () => {
    const bookId = nextBookId++
    mockApi(bookId)

    renderPage(bookId)

    expect(await screen.findByRole('heading', { level: 1, name: `Book ${bookId} - Reviews` })).toBeInTheDocument()

    const reviews = await screen.findByRole('region', { name: 'Reviews' })
    expect(within(reviews).getByText('Ana Torres')).toBeInTheDocument()
    expect(within(reviews).getByText('Loved it')).toBeInTheDocument()
    expect(within(reviews).getByText('Luis Pérez')).toBeInTheDocument()
    expect(within(reviews).getAllByRole('article')).toHaveLength(2)

    expect(screen.getByText('4.5')).toBeInTheDocument()
    expect(screen.getByText('rate counter: 2')).toBeInTheDocument()
    expect(screen.getAllByText('14-06-2025')).toHaveLength(2)
  })

  it('shows an error with a link home when the book does not exist', async () => {
    const bookId = nextBookId++
    mockFetch(() => emptyResponse(404))

    renderPage(bookId)

    expect(await screen.findByRole('alert')).toHaveTextContent("We couldn't load this book")
    expect(screen.getByRole('link', { name: 'Back to home' })).toHaveAttribute('href', '/')
  })

  it('shows a message when the book has no reviews yet', async () => {
    const bookId = nextBookId++
    const fetchMock = mockApi(bookId)
    const original = fetchMock.getMockImplementation()!
    fetchMock.mockImplementation(async (input, init) =>
      String(input).includes('/ratings?')
        ? jsonResponse({ averageRating: 0, numberOfRatings: 0, users: [] })
        : original(input, init),
    )

    renderPage(bookId)

    expect(await screen.findByText('No reviews yet. Be the first to share your thoughts.')).toBeInTheDocument()
  })

  it('shows an alert when the reviews cannot be loaded', async () => {
    const bookId = nextBookId++
    const fetchMock = mockApi(bookId)
    const original = fetchMock.getMockImplementation()!
    fetchMock.mockImplementation(async (input, init) =>
      String(input).includes('/ratings?') ? emptyResponse(500) : original(input, init),
    )

    renderPage(bookId)

    expect(await screen.findByText("We couldn't load the reviews. Please try again.")).toBeInTheDocument()
  })

  it('sends a logged-out user to /login when pressing "Add Review"', async () => {
    const bookId = nextBookId++
    mockApi(bookId, { userId: null })
    const { user } = renderPage(bookId)

    const addButton = await screen.findByRole('button', { name: 'Add Review' })
    await waitFor(() => expect(addButton).toBeEnabled())
    await user.click(addButton)

    expect(await screen.findByText('Login page')).toBeInTheDocument()
  })

  it('lets a logged-in user publish a review and then refreshes the list', async () => {
    const bookId = nextBookId++
    localStorage.setItem('lectio:accessToken', 'test-token')
    const fetchMock = mockApi(bookId, {
      userId: 5,
      ratingsAfterPost: {
        averageRating: 4.3,
        numberOfRatings: 3,
        users: [review(5, 'Steven', 4, 'Very useful'), ...ratingsPage.users],
      },
    })
    const { user } = renderPage(bookId)

    const addButton = await screen.findByRole('button', { name: 'Add Review' })
    await waitFor(() => expect(addButton).toBeEnabled())
    await user.click(addButton)

    await user.click(screen.getByRole('radio', { name: '4 de 5 estrellas' }))
    await user.type(screen.getByLabelText('Comment (optional)'), 'Very useful')
    await user.click(screen.getByRole('button', { name: 'Submit' }))

    // El POST llevó el cuerpo y el JWT correctos
    await waitFor(() => expect(postCalls(fetchMock)).toHaveLength(1))
    const [url, init] = postCalls(fetchMock)[0]
    expect(url).toBe('http://api.test/api/users/5/ratings')
    expect(JSON.parse(init.body)).toEqual({ bookId, stars: 4, comment: 'Very useful' })
    expect(init.headers).toMatchObject({ Authorization: 'Bearer test-token', 'Content-Type': 'application/json' })

    // La UI cierra el formulario, avisa y recarga reseñas y promedio
    expect(await screen.findByRole('status')).toHaveTextContent('Review saved successfully')
    expect(screen.queryByText('Your review')).not.toBeInTheDocument()
    expect(await screen.findByText('Very useful', { selector: 'p' })).toBeInTheDocument()
    expect(screen.getByText('rate counter: 3')).toBeInTheDocument()
  })

  it('sends a null comment when the user leaves it empty', async () => {
    const bookId = nextBookId++
    const fetchMock = mockApi(bookId)
    const { user } = renderPage(bookId)

    const addButton = await screen.findByRole('button', { name: 'Add Review' })
    await waitFor(() => expect(addButton).toBeEnabled())
    await user.click(addButton)
    await user.click(screen.getByRole('radio', { name: '5 de 5 estrellas' }))
    await user.click(screen.getByRole('button', { name: 'Submit' }))

    await waitFor(() => expect(postCalls(fetchMock)).toHaveLength(1))
    expect(JSON.parse(postCalls(fetchMock)[0][1].body)).toEqual({ bookId, stars: 5, comment: null })
  })

  it('shows the server message inside the form when the backend rejects the review', async () => {
    const bookId = nextBookId++
    mockApi(bookId, {
      postResponse: jsonResponse({ detail: 'You can only rate books you have borrowed.' }, 400),
    })
    const { user } = renderPage(bookId)

    const addButton = await screen.findByRole('button', { name: 'Add Review' })
    await waitFor(() => expect(addButton).toBeEnabled())
    await user.click(addButton)
    await user.click(screen.getByRole('radio', { name: '3 de 5 estrellas' }))
    await user.click(screen.getByRole('button', { name: 'Submit' }))

    expect(await screen.findByText('You can only rate books you have borrowed.')).toBeInTheDocument()
    // el formulario sigue abierto para que el usuario pueda reintentar
    expect(screen.getByRole('button', { name: 'Submit' })).toBeEnabled()
    expect(screen.queryByRole('status')).not.toBeInTheDocument()
  })

  it('asks the user to log in again when the session expired (401)', async () => {
    const bookId = nextBookId++
    mockApi(bookId, { postResponse: emptyResponse(401) })
    const { user } = renderPage(bookId)

    const addButton = await screen.findByRole('button', { name: 'Add Review' })
    await waitFor(() => expect(addButton).toBeEnabled())
    await user.click(addButton)
    await user.click(screen.getByRole('radio', { name: '2 de 5 estrellas' }))
    await user.click(screen.getByRole('button', { name: 'Submit' }))

    expect(await screen.findByText('Your session has expired. Please log in again.')).toBeInTheDocument()
  })

  it('closes the form without sending anything when pressing Cancel', async () => {
    const bookId = nextBookId++
    const fetchMock = mockApi(bookId)
    const { user } = renderPage(bookId)

    const addButton = await screen.findByRole('button', { name: 'Add Review' })
    await waitFor(() => expect(addButton).toBeEnabled())
    await user.click(addButton)
    expect(screen.getByText('Your review')).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: 'Cancel' }))

    expect(screen.queryByText('Your review')).not.toBeInTheDocument()
    expect(postCalls(fetchMock)).toHaveLength(0)
  })
})
