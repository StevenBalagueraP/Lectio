import type { Category } from './category'

export interface Author {
  id: number
  name: string
}

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

export interface BookDetail {
  id: number
  title: string
  imageUrl: string | null
  description: string | null
  pageCount: number | null
  publishedYear: number | null
  publisher: string | null
  language: string | null
  authors: Author[]
  categories: Category[]
}