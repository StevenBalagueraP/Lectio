import { getBookLoanStatus } from '../../api/endpoints/books'
import { useFetch } from './useFetch'
import { useSession } from './useSession'

export function useBookLoanStatus(bookId: number, enabled = true, version = 0) {
  const user = useSession()

  return useFetch(
    `loan-status:${bookId}:${user?.id ?? 'anon'}:v${version}`,
    (signal) => getBookLoanStatus(bookId, signal),
    { enabled },
  )
}