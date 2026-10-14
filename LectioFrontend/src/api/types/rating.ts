export interface BookRatingUser {
  idUser: number
  fullName: string
  comment: string | null
  rating: number
  createdAt: string
  updatedAt: string
}

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