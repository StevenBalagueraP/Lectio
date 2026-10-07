import { http } from '../http'
import type { CreateRatingBody, RatingResponse } from '../types/rating'

const USERS_PATH = '/api/users'

// Requiere JWT y que {userId} sea el del token. Crea o actualiza (una reseña por usuario y libro);
// el backend rechaza con 400 si el usuario nunca prestó el libro.
export function createRating(userId: number, body: CreateRatingBody, signal?: AbortSignal) {
  return http<RatingResponse>(`${USERS_PATH}/${userId}/ratings`, signal, {
    auth: true,
    method: 'POST',
    body,
  })
}