import { http } from '../http'
import type { ApiResponse } from '../types/common'
import type { Book } from '../types/book'

export async function getTopRatedBooks(categoryId?: number, signal?: AbortSignal) {
  const query = categoryId !== undefined ? `?categoryId=${categoryId}` : ''
  const res = await http<ApiResponse<Book[]>>(`/api/books/top-rated${query}`, signal)
  return res.data
}