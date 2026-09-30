import { getCategories } from '../../api/endpoints/categories'
import { getTopRatedBooks } from '../../api/endpoints/books'
import { useAsync } from '../../shared/hooks/useAsync'

export function useCategories() {
  return useAsync('categories', (signal) => getCategories(signal))
}

export function useTopRatedBooks(categoryId?: number, enabled = true) {
  return useAsync(
    `top-rated:${categoryId ?? 'all'}`,
    (signal) => getTopRatedBooks(categoryId, signal),
    enabled,
  )
}