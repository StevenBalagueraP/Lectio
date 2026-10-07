import { useCallback } from 'react'
import { useNavigate } from 'react-router'

/** Vuelve a la pantalla anterior; si se entró directo por URL (sin historial), va a `fallback`. */
export function useGoBack(fallback: string) {
  const navigate = useNavigate()

  return useCallback(() => {
    if (window.history.state?.idx > 0) navigate(-1)
    else navigate(fallback)
  }, [navigate, fallback])
}