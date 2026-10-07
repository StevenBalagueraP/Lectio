import { getBookLoanStatus } from '../../api/endpoints/books'
import { getAccessToken } from '../../api/token'
import { useFetch } from './useFetch'

export function useBookLoanStatus(bookId: number, enabled = true) {
  const session = getAccessToken() ? 'auth' : 'anon'

  return useFetch(
    `loan-status:${bookId}:${session}`,
    (signal) => getBookLoanStatus(bookId, signal),
    { enabled },
  )
}