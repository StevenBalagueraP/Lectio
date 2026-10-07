/** Una reseña dentro de GET /api/books/{id}/ratings. El backend no devuelve el nombre del autor, solo `idUser`. */
export interface BookRatingUser {
  idUser: number
  fullName: string
  comment: string | null
  rating: number
  createdAt: string
  updatedAt: string
}

/** Respuesta de GET /api/books/{id}/ratings (sin envoltorio `data` ni objeto `pagination`). */
export interface BookRatings {
  averageRating: number
  numberOfRatings: number
  users: BookRatingUser[]
}

export interface CreateRatingBody {
  bookId: number
  stars: number
  comment: string | null
}

/** Respuesta de POST /api/users/{userId}/ratings (200 tanto al crear como al actualizar). */
export interface RatingResponse {
  message: string
  ratingId: number
  bookId: number
  userId: number
  stars: number
  comment: string | null
  createdAt: string
  updatedAt: string
}