import { getBookById, getBookLoanStatus } from '../../api/endpoints/books'
import { getAccessToken } from '../../api/token'
import { useFetch } from '../../shared/hooks/useFetch'

const BOOK_DETAIL_TTL = 5 * 60 * 1000 // 5 minutos, solo en memoria

export function useBookDetail(bookId: number, enabled = true) {
  return useFetch(`book:${bookId}`, (signal) => getBookById(bookId, signal), {
    enabled,
    ttl: BOOK_DETAIL_TTL,
  })
}

export function useBookLoanStatus(bookId: number, enabled = true) {
  const session = getAccessToken() ? 'auth' : 'anon'

  return useFetch(
    `loan-status:${bookId}:${session}`,
    (signal) => getBookLoanStatus(bookId, signal),
    { enabled },
  )
}