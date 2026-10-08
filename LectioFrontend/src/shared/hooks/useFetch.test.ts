import { act, renderHook, waitFor } from '@testing-library/react'
import { useFetch } from './useFetch'

// Los keys son únicos por prueba porque el cache vive a nivel de módulo.
describe('useFetch', () => {
  it('goes from loading to data', async () => {
    const fetcher = jest.fn().mockResolvedValue(['a', 'b'])

    const { result } = renderHook(() => useFetch('uf:basic', fetcher))

    expect(result.current).toEqual({ data: undefined, error: undefined, loading: true })
    await waitFor(() => expect(result.current.loading).toBe(false))

    expect(result.current.data).toEqual(['a', 'b'])
    expect(fetcher).toHaveBeenCalledTimes(1)
  })

  it('passes an AbortSignal to the fetcher', async () => {
    const fetcher = jest.fn().mockResolvedValue('ok')

    renderHook(() => useFetch('uf:signal', fetcher))

    await waitFor(() => expect(fetcher).toHaveBeenCalled())
    expect(fetcher.mock.calls[0][0]).toBeInstanceOf(AbortSignal)
  })

  it('exposes the message of the error thrown by the fetcher', async () => {
    const fetcher = jest.fn().mockRejectedValue(new Error('boom'))

    const { result } = renderHook(() => useFetch('uf:error', fetcher))

    await waitFor(() => expect(result.current.error).toBe('boom'))
    expect(result.current.loading).toBe(false)
    expect(result.current.data).toBeUndefined()
  })

  it('uses a generic message when the rejection is not an Error', async () => {
    const fetcher = jest.fn().mockRejectedValue('nope')

    const { result } = renderHook(() => useFetch('uf:error-string', fetcher))

    await waitFor(() => expect(result.current.error).toBe('Error desconocido'))
  })

  it('does not run the fetcher when enabled is false', () => {
    const fetcher = jest.fn().mockResolvedValue('x')

    const { result } = renderHook(() => useFetch('uf:disabled', fetcher, { enabled: false }))

    expect(result.current.loading).toBe(false)
    expect(fetcher).not.toHaveBeenCalled()
  })

  it('shares one in-flight request between hooks with the same key (ttl > 0)', async () => {
    const fetcher = jest.fn().mockResolvedValue('shared')

    const a = renderHook(() => useFetch('uf:dedupe', fetcher, { ttl: 1000 }))
    const b = renderHook(() => useFetch('uf:dedupe', fetcher, { ttl: 1000 }))

    await waitFor(() => expect(a.result.current.data).toBe('shared'))
    await waitFor(() => expect(b.result.current.data).toBe('shared'))
    expect(fetcher).toHaveBeenCalledTimes(1)
  })

  it('returns cached data while the ttl has not expired', async () => {
    const fetcher = jest.fn().mockResolvedValue('cached')

    const first = renderHook(() => useFetch('uf:ttl', fetcher, { ttl: 60_000 }))
    await waitFor(() => expect(first.result.current.data).toBe('cached'))
    first.unmount()

    const second = renderHook(() => useFetch('uf:ttl', fetcher, { ttl: 60_000 }))

    expect(second.result.current).toEqual({ data: 'cached', error: undefined, loading: false })
    expect(fetcher).toHaveBeenCalledTimes(1)
  })

  it('falls back to stale data when the refetch fails', async () => {
    jest.useFakeTimers()
    try {
      const fetcher = jest.fn().mockResolvedValueOnce('old').mockRejectedValueOnce(new Error('offline'))

      const first = renderHook(() => useFetch('uf:stale', fetcher, { ttl: 1000 }))
      await act(async () => {})
      expect(first.result.current.data).toBe('old')
      first.unmount()

      jest.advanceTimersByTime(2000) // el cache vence

      const second = renderHook(() => useFetch('uf:stale', fetcher, { ttl: 1000 }))
      await act(async () => {})

      expect(second.result.current.data).toBe('old')
      expect(second.result.current.error).toBeUndefined()
      expect(fetcher).toHaveBeenCalledTimes(2)
    } finally {
      jest.useRealTimers()
    }
  })

  it('refetches when the key changes and hides old data unless keepPrevious is set', async () => {
    const fetcher = jest.fn().mockResolvedValueOnce('data-1').mockResolvedValueOnce('data-2')

    const { result, rerender } = renderHook(({ k }) => useFetch(k, fetcher), { initialProps: { k: 'uf:key-1' } })
    await waitFor(() => expect(result.current.data).toBe('data-1'))

    rerender({ k: 'uf:key-2' })

    expect(result.current.data).toBeUndefined()
    expect(result.current.loading).toBe(true)
    await waitFor(() => expect(result.current.data).toBe('data-2'))
  })

  it('keeps the previous data while loading the next key when keepPrevious is true', async () => {
    const fetcher = jest.fn().mockResolvedValueOnce('page 1').mockResolvedValueOnce('page 2')

    const { result, rerender } = renderHook(({ k }) => useFetch(k, fetcher, { keepPrevious: true }), {
      initialProps: { k: 'uf:keep-1' },
    })
    await waitFor(() => expect(result.current.data).toBe('page 1'))

    rerender({ k: 'uf:keep-2' })

    expect(result.current.data).toBe('page 1')
    expect(result.current.loading).toBe(true)
    await waitFor(() => expect(result.current.data).toBe('page 2'))
  })
})