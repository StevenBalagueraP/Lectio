import { http } from '../http'
import type { Book, BookDetail, BookSummary } from '../types/book'
import type { ApiResponse, PagedResponse } from '../types/common'
import type { BookLoanStatus } from '../types/loan'

const BOOKS_PATH = '/api/books'

export interface BookSearchParams {
  search: string
  categoryIds: number[]
  page: number
  pageSize: number
}

export async function getTopRatedBooks(categoryId?: number, signal?: AbortSignal) {
  const query = categoryId !== undefined ? `?categoryId=${categoryId}` : ''
  const res = await http<ApiResponse<Book[]>>(`${BOOKS_PATH}/top-rated${query}`, signal)
  return res.data
}

export function searchBooks(
  { search, categoryIds, page, pageSize }: BookSearchParams,
  signal?: AbortSignal,
) {
  const query = new URLSearchParams()
  if (search) query.set('search', search)
  categoryIds.forEach((id) => query.append('categoryIds', String(id)))
  query.set('page', String(page))
  query.set('pageSize', String(pageSize))

  return http<PagedResponse<BookSummary>>(`${BOOKS_PATH}?${query.toString()}`, signal)
}

export function getBookById(bookId: number, signal?: AbortSignal) {
  return http<BookDetail>(`${BOOKS_PATH}/${bookId}`, signal)
}

export function getBookLoanStatus(bookId: number, signal?: AbortSignal) {
  return http<BookLoanStatus>(`${BOOKS_PATH}/${bookId}/loan-status`, signal, { auth: true })
}