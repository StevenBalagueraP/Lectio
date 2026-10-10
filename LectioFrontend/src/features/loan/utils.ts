import { ApiError } from '../../api/http'

// 401/403 come without body; for 4xx the backend `detail` is safe to show (5xx are not)
export function getLoanErrorMessage(error: unknown): string {
  if (error instanceof ApiError) {
    if (error.status === 401 || error.status === 403)
      return 'Your session has expired. Please log in again.'
    if (error.status < 500 && error.serverMessage) return error.serverMessage
  }
  return 'Something went wrong. Please try again.'
}
