import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router'
import { emptyResponse, jsonResponse, mockFetch } from '../../../test/mockFetch'
import { LoginPage } from './LoginPage'

const AUTH_RESPONSE = {
  accessToken: 'acc-token-123',
  refreshToken: 'ref-token-456',
  tokenType: 'Bearer',
  expiresIn: 900,
  user: { id: 7, fullName: 'Steven Test', email: 'steven@test.com', phone: null, role: 'user' as const },
}

function mockLoginApi(response: Response) {
  return mockFetch((url, init) => {
    const { pathname } = new URL(url)
    if (pathname === '/api/auth/login' && init.method === 'POST') return response
    return emptyResponse(404)
  })
}

function renderPage(initialPath = '/login') {
  const user = userEvent.setup()

  render(
    <MemoryRouter initialEntries={[initialPath]}>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/" element={<p>Home page</p>} />
        <Route path="/register" element={<p>Register page</p>} />
      </Routes>
    </MemoryRouter>,
  )

  return { user }
}

describe('LoginPage (integration)', () => {
  it('renders the login form with Email and Password fields', () => {
    mockLoginApi(emptyResponse(404))
    renderPage()

    expect(screen.getByLabelText('Email')).toBeInTheDocument()
    expect(screen.getByLabelText('Password')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Log in' })).toBeInTheDocument()
  })

  it('shows inline validation errors when submitting with empty fields', async () => {
    mockLoginApi(emptyResponse(404))
    const { user } = renderPage()

    await user.click(screen.getByRole('button', { name: 'Log in' }))

    expect(await screen.findByText('Email is required.')).toBeInTheDocument()
    expect(screen.getByText('Password is required.')).toBeInTheDocument()
  })

  it('shows an "invalid email" error for a malformed email', async () => {
    mockLoginApi(emptyResponse(404))
    const { user } = renderPage()

    await user.type(screen.getByLabelText('Email'), 'notanemail')
    await user.type(screen.getByLabelText('Password'), 'secret')
    await user.click(screen.getByRole('button', { name: 'Log in' }))

    expect(await screen.findByText('Enter a valid email address.')).toBeInTheDocument()
  })

  it('shows a server error message when the backend responds with 401', async () => {
    mockLoginApi(jsonResponse({ detail: 'Invalid email or password.' }, 401))
    const { user } = renderPage()

    await user.type(screen.getByLabelText('Email'), 'wrong@email.com')
    await user.type(screen.getByLabelText('Password'), 'wrongpass')
    await user.click(screen.getByRole('button', { name: 'Log in' }))

    expect(await screen.findByRole('alert')).toHaveTextContent('Invalid email or password.')
  })

  it('redirects to / after a successful login', async () => {
    mockLoginApi(jsonResponse(AUTH_RESPONSE))
    const { user } = renderPage()

    await user.type(screen.getByLabelText('Email'), 'steven@test.com')
    await user.type(screen.getByLabelText('Password'), 'correct-password')
    await user.click(screen.getByRole('button', { name: 'Log in' }))

    expect(await screen.findByText('Home page')).toBeInTheDocument()
  })

  it('shows a fallback error when the backend responds with a 500 and no detail', async () => {
    mockLoginApi(emptyResponse(500))
    const { user } = renderPage()

    await user.type(screen.getByLabelText('Email'), 'user@test.com')
    await user.type(screen.getByLabelText('Password'), 'pass123')
    await user.click(screen.getByRole('button', { name: 'Log in' }))

    expect(await screen.findByRole('alert')).toHaveTextContent('Something went wrong. Please try again.')
  })

  it('links to the register page', () => {
    mockLoginApi(emptyResponse(404))
    renderPage()

    expect(screen.getByRole('link', { name: 'Sign up' })).toHaveAttribute('href', '/register')
  })

  it('shows "Logging in…" while the request is in flight', async () => {
    let resolve!: (r: Response) => void
    const pending = new Promise<Response>((res) => { resolve = res })
    mockFetch(() => pending)

    const { user } = renderPage()

    await user.type(screen.getByLabelText('Email'), 'steven@test.com')
    await user.type(screen.getByLabelText('Password'), 'pass')
    await user.click(screen.getByRole('button', { name: 'Log in' }))

    expect(await screen.findByRole('button', { name: 'Logging in…' })).toBeDisabled()

    resolve(jsonResponse(AUTH_RESPONSE))
    await waitFor(() => expect(screen.queryByRole('button', { name: 'Logging in…' })).not.toBeInTheDocument())
  })
})
