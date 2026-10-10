import { getUserLoans } from '../../api/endpoints/loans'
import { useFetch } from '../../shared/hooks/useFetch'
import { PAGE_SIZE } from './constants'

// No cache (ttl 0) so the list is fresh after returning a book.
// keepPrevious: true keeps the previous page visible while the next one loads.
// The key includes userId, page and pageSize so the effect reruns on any change.
export function useUserLoans(userId: number, page: number, enabled: boolean) {
  return useFetch(
    `user-loans:${userId}:${page}:${PAGE_SIZE}`,
    (signal) => getUserLoans(userId, { page, pageSize: PAGE_SIZE }, signal),
    { enabled, ttl: 0, keepPrevious: true },
  )
}
