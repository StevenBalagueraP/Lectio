import type { Category } from './category'

export interface Author {
  id: number
  name: string
}

export interface Book {
  id: number
  title: string
  description: string | null
  imageUrl: string | null
  rating: { stars: number } | null
  authors: Author[]
  categories: Category[]
}