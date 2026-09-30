import { http } from '../http'
import type { ApiResponse } from '../types/common'
import type { Category } from '../types/category'

export async function getCategories(signal?: AbortSignal) {
  const res = await http<ApiResponse<Category[]>>('/api/categories', signal)
  return res.data
}