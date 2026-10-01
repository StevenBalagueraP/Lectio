import type { Author } from '../../api/types/book'

export const toHttps = (url: string) => url.replace(/^http:\/\//, 'https://')

export function formatAuthors(authors: Author[]) {
  if (authors.length === 0) return 'Autor desconocido'
  const [first, ...others] = authors
  return others.length > 0 ? `${first.name} +${others.length}` : first.name
}