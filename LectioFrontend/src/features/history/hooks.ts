import { getUserLoans } from '../../api/endpoints/loans'
import { useFetch } from '../../shared/hooks/useFetch'
import { PAGE_SIZE } from './constants'

export function useUserLoans(userId: number, page: number, enabled: boolean) {
  return useFetch(
    `user-loans:${userId}:${page}:${PAGE_SIZE}`,
    (signal) => getUserLoans(userId, { page, pageSize: PAGE_SIZE }, signal),
    { enabled, ttl: 0, keepPrevious: true },
  )
}
