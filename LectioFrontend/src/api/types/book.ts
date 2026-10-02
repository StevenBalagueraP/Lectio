import type { Category } from './category'

export interface Author {
  id: number
  name: string
}

/** Lo mínimo que necesita una card (es lo que devuelve el buscador). */
export interface BookSummary {
  id: number
  title: string
  imageUrl: string | null
  rating: { stars: number } | null
  authors: Author[]
  categories: Category[]
}

export interface Book extends BookSummary {
  description: string | null
}