import { useSyncExternalStore } from 'react'
import { getStoredUser, subscribeSession } from '../../api/token'
import type { AuthUser } from '../../api/types/auth'

/** Usuario con sesión iniciada (o `null`). Se actualiza al hacer login, logout o refresh, también entre pestañas. */
export function useSession(): AuthUser | null {
  return useSyncExternalStore(subscribeSession, getStoredUser)
}