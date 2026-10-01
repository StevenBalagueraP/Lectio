import { useEffect, useState } from 'react'

interface State<T> {
  key: string
  data?: T
  error?: string
}

export function useAsync<T>(
  key: string,
  fetcher: (signal: AbortSignal) => Promise<T>,
  enabled = true,
) {
  const [state, setState] = useState<State<T>>({ key: '' })

  useEffect(() => {
    if (!enabled) return
    const controller = new AbortController()

    fetcher(controller.signal)
      .then((data) => setState({ key, data }))
      .catch((err: unknown) => {
        if (controller.signal.aborted) return
        setState({ key, error: err instanceof Error ? err.message : 'Error desconocido' })
      })

    return () => controller.abort()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [key, enabled])

  const isCurrent = state.key === key

  return {
    data: isCurrent ? state.data : undefined,
    error: isCurrent ? state.error : undefined,
    loading: enabled && !isCurrent,
  }
}