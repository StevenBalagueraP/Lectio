import { ApiError } from '../../api/http'

export function getLoanErrorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    if (error.status === 401 || error.status === 403)
      return 'Your session has expired. Please log in again.'
    if (error.status < 500 && error.serverMessage) return error.serverMessage
  }
  return 'Something went wrong. Please try again.'
}
