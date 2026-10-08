import { ApiError } from '../../api/http'

// En 4xx el `detail` del backend es seguro de mostrar; en 5xx o sin red se usa el texto genérico
export function getApiErrorMessage(error: unknown, fallback: string) {
  if (error instanceof ApiError && error.status < 500 && error.serverMessage) {
    return error.serverMessage
  }
  return fallback
}