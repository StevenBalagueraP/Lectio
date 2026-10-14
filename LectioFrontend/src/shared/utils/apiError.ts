import { ApiError } from '../../api/http'

export function getApiErrorMessage(error: unknown, fallback: string) {
  if (error instanceof ApiError && error.status < 500 && error.serverMessage) {
    return error.serverMessage
  }
  return fallback
}