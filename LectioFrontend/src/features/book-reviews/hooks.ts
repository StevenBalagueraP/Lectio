import { getBookRatings } from '../../api/endpoints/books'
import { useFetch } from '../../shared/hooks/useFetch'
import { RATINGS_PAGE_SIZE } from './constants'

const RATINGS_TTL = 60 * 1000 // 1 minuto, solo en memoria

// `version` se incrementa al publicar una reseña: cambia la key y fuerza una consulta nueva.
// `keepPrevious` deja la página anterior visible mientras llega la siguiente.
export function useBookRatings(bookId: number, page: number, version: number, enabled = true) {
  return useFetch(
    `ratings:${bookId}:${page}:${RATINGS_PAGE_SIZE}:v${version}`,
    (signal) => getBookRatings(bookId, { page, pageSize: RATINGS_PAGE_SIZE }, signal),
    { enabled, ttl: RATINGS_TTL, keepPrevious: true },
  )
}