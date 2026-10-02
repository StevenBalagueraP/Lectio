import { searchBooks, type BookSearchParams } from '../../api/endpoints/books'
import { useFetch } from '../../shared/hooks/useFetch'

const SEARCH_TTL = 2 * 60 * 1000 // 2 minutos, solo en memoria

export function useBookSearch(params: BookSearchParams) {
  const key = `books:${params.search}|${params.categoryIds.join(',')}|${params.page}|${params.pageSize}`

  return useFetch(key, (signal) => searchBooks(params, signal), {
    ttl: SEARCH_TTL,
    keepPrevious: true,
  })
}