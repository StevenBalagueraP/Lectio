import { http } from '../http'
import type { CreateRatingBody, RatingResponse } from '../types/rating'

const USERS_PATH = '/api/users'

export function createRating(userId: number, body: CreateRatingBody, signal?: AbortSignal) {
  return http<RatingResponse>(`${USERS_PATH}/${userId}/ratings`, signal, {
    auth: true,
    method: 'POST',
    body,
  })
}