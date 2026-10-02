import { useEffect, useState } from 'react'
import { fetchCached, readCache, readStale } from '../utils/cache'

interface State<T> {
  key: string
  data?: T
  error?: string
}

interface UseFetchOptions {
  enabled?: boolean
  /** Milisegundos que se guarda la respuesta en cache. 0 = sin cache. */
  ttl?: number
  /** Además de memoria, guarda en localStorage (sobrevive a recargas). */
  persist?: boolean
  /** Mientras carga una key nueva, sigue devolviendo la data anterior. */
  keepPrevious?: boolean
}

export function useFetch<T>(
  key: string,
  fetcher: (signal: AbortSignal) => Promise<T>,
  { enabled = true, ttl = 0, persist = false, keepPrevious = false }: UseFetchOptions = {},
) {
  const [state, setState] = useState<State<T>>({ key: '' })

  const cached = enabled && ttl > 0 ? readCache<T>(key, persist) : undefined

  useEffect(() => {
    if (!enabled || cached !== undefined) return

    const controller = new AbortController()

    // Con cache la petición es compartida, así que no se cancela con el componente:
    // `controller` solo sirve para ignorar el resultado si ya se desmontó.
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

        // Si el backend falla pero hay una copia guardada (aunque vencida), la mostramos
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
    data: isCurrent || keepPrevious ? state.data : undefined,
    error: isCurrent ? state.error : undefined,
    loading: enabled && !isCurrent,
  }
}