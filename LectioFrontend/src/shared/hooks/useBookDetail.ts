import { getBookById } from '../../api/endpoints/books'
import { useFetch } from './useFetch'

const BOOK_DETAIL_TTL = 5 * 60 * 1000 

export function useBookDetail(bookId: number, enabled = true) {
  return useFetch(`book:${bookId}`, (signal) => getBookById(bookId, signal), {
    enabled,
    ttl: BOOK_DETAIL_TTL,
  })
}