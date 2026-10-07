import { getBookLoanStatus } from '../../api/endpoints/books'
import { getAccessToken } from '../../api/token'
import { useFetch } from './useFetch'

// Sin cache: el estado cambia al prestar/devolver. La key distingue con/sin sesión
// porque el resultado del endpoint depende del JWT.
// También sirve para saber si hay sesión: `data.userId !== null` (sin JWT llega `LoginRequired` y `userId: null`).
export function useBookLoanStatus(bookId: number, enabled = true) {
  const session = getAccessToken() ? 'auth' : 'anon'

  return useFetch(
    `loan-status:${bookId}:${session}`,
    (signal) => getBookLoanStatus(bookId, signal),
    { enabled },
  )
}