import { ApiError } from '../../api/http'

export function formatReviewDate(iso: string) {
  const date = new Date(iso)
  if (Number.isNaN(date.getTime())) return ''

  const day = String(date.getUTCDate()).padStart(2, '0')
  const month = String(date.getUTCMonth() + 1).padStart(2, '0')
  return `${day}-${month}-${date.getUTCFullYear()}`
}

export function getSubmitErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    if (error.status === 401 || error.status === 403) {
      return 'Your session has expired. Please log in again.'
    }
    if (error.status < 500 && error.serverMessage) return error.serverMessage
  }
  return 'Something went wrong while saving your review. Please try again.'
}