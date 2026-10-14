import { useSyncExternalStore } from 'react'
import { getStoredUser, subscribeSession } from '../../api/token'
import type { AuthUser } from '../../api/types/auth'

export function useSession(): AuthUser | null {
  return useSyncExternalStore(subscribeSession, getStoredUser)
}