import { getCategories } from '../../api/endpoints/categories'
import { useFetch } from './useFetch'

const CATEGORIES_TTL = 10 * 60 * 1000

export function useCategories() {
  return useFetch('categories', (signal) => getCategories(signal), {
    ttl: CATEGORIES_TTL,
    persist: true,
  })
}