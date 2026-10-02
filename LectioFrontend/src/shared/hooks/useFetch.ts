import { useEffect, useState } from 'react'
import { fetchCached, readCache, readStale } from '../utils/cache'

interface State<T> {
  key: string
  data?: T
  error?: string
}

interface UseFetchOptions {
  enabled?: boolean
  ttl?: number
  persist?: boolean
}

export function useFetch<T>(
  key: string,
  fetcher: (signal: AbortSignal) => Promise<T>,
  { enabled = true, ttl = 0, persist = false }: UseFetchOptions = {},
) {
  const [state, setState] = useState<State<T>>({ key: '' })

  const cached = enabled && ttl > 0 ? readCache<T>(key, persist) : undefined

  useEffect(() => {
    if (!enabled || cached !== undefined) return

    const controller = new AbortController()

    const request =
      ttl > 0
        ? fetchCached(key, () => fetcher(new AbortController().signal), ttl, persist)
        : fetcher(controller.signal)

    request
      .then((data) => {
        if (!controller.signal.aborted) setState({ key, data })
      })
      .catch((err: unknown) => {
        if (controller.signal.aborted) return

        const stale = ttl > 0 ? readStale<T>(key, persist) : undefined
        if (stale !== undefined) {
          setState({ key, data: stale })
          return
        }

        setState({ key, error: err instanceof Error ? err.message : 'Error desconocido' })
      })

    return () => controller.abort()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [key, enabled])

  if (cached !== undefined) {
    return { data: cached, error: undefined, loading: false }
  }

  const isCurrent = state.key === key

  return {
    data: isCurrent ? state.data : undefined,
    error: isCurrent ? state.error : undefined,
    loading: enabled && !isCurrent,
  }
}