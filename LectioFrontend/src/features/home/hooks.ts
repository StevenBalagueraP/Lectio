import { getTopRatedBooks } from '../../api/endpoints/books'
import { useFetch } from '../../shared/hooks/useFetch'

const TOP_RATED_TTL = 2 * 60 * 1000 // 2 minutos

export function useTopRatedBooks(categoryId?: number, enabled = true) {
  return useFetch(
    `top-rated:${categoryId ?? 'all'}`,
    (signal) => getTopRatedBooks(categoryId, signal),
    { enabled, ttl: TOP_RATED_TTL, persist: true },
  )
}