import { useCallback, useState } from 'react'
import { logout } from '../../api/endpoints/auth'
import { clearSession, getRefreshToken } from '../../api/token'

export function useLogout() {
  const [loggingOut, setLoggingOut] = useState(false)

  const handleLogout = useCallback(async () => {
    setLoggingOut(true)

    const refreshToken = getRefreshToken()
    try {
      if (refreshToken) await logout(refreshToken)
    } catch {
      // Sin red o con error del backend igual se cierra la sesión local
    } finally {
      clearSession() 
    }
  }, [])

  return { loggingOut, logout: handleLogout }
}