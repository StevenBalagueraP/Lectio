import type { BookFilter } from '../../api/types/bookFilter'

export const BOOK_FILTER_OPTIONS: { value: BookFilter; label: string }[] = [
  { value: 'TopRated', label: 'Top rated' },
  { value: 'Available', label: 'Available only' },
  { value: 'Newest', label: 'Newest first' },
  { value: 'Oldest', label: 'Oldest first' },
  { value: 'Longest', label: 'Longest first' },
  { value: 'Shortest', label: 'Shortest first' },
]

// El backend responde 400 con estas parejas, así que elegir una quita la contraria
const OPPOSITES: Partial<Record<BookFilter, BookFilter>> = {
  Newest: 'Oldest',
  Oldest: 'Newest',
  Longest: 'Shortest',
  Shortest: 'Longest',
}

const VALID_VALUES = new Set<string>(BOOK_FILTER_OPTIONS.map((option) => option.value))

export function getBookFilterLabel(value: BookFilter) {
  return BOOK_FILTER_OPTIONS.find((option) => option.value === value)?.label ?? value
}

// No se ordena: el orden de la URL es la prioridad que se manda al backend
export function parseBookFilters(value: string | null): BookFilter[] {
  if (!value) return []

  const unique = [...new Set(value.split(',').filter((v): v is BookFilter => VALID_VALUES.has(v)))]
  const result: BookFilter[] = []

  for (const filter of unique) {
    const opposite = OPPOSITES[filter]
    if (!opposite || !result.includes(opposite)) result.push(filter)
  }

  return result
}

export function toggleBookFilter(current: BookFilter[], filter: BookFilter): BookFilter[] {
  if (current.includes(filter)) return current.filter((f) => f !== filter)

  const opposite = OPPOSITES[filter]
  return [...current.filter((f) => f !== opposite), filter]
}