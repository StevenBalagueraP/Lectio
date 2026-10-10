import { getBookLoanStatus } from '../../api/endpoints/books'
import { useFetch } from './useFetch'
import { useSession } from './useSession'

// Sin cache: el estado cambia al prestar/devolver. La key incluye al usuario porque el
// resultado del endpoint depende del JWT (y así se recarga al iniciar o cerrar sesión).
// También sirve para saber si hay sesión: `data.userId !== null` (sin JWT llega `LoginRequired` y `userId: null`).
export function useBookLoanStatus(bookId: number, enabled = true, version = 0) {
  const user = useSession()

  return useFetch(
    `loan-status:${bookId}:${user?.id ?? 'anon'}:v${version}`,
    (signal) => getBookLoanStatus(bookId, signal),
    { enabled },
  )
}