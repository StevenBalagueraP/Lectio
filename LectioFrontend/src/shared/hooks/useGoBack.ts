import { useCallback } from 'react'
import { useNavigate } from 'react-router'

export function useGoBack(fallback: string) {
  const navigate = useNavigate()

  return useCallback(() => {
    if (window.history.state?.idx > 0) navigate(-1)
    else navigate(fallback)
  }, [navigate, fallback])
}