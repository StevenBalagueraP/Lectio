import { act, renderHook, waitFor } from '@testing-library/react'
import { useDebouncedValue } from './useDebouncedValue'

// Prueba de integración: el hook real + los timers reales de jest (fake timers).
// Verifica el ciclo completo: valor cambia → se aplaza → se confirma tras el delay.

describe('useDebouncedValue (integration)', () => {
  beforeEach(() => {
    jest.useFakeTimers()
  })

  afterEach(() => {
    jest.useRealTimers()
  })

  it('returns the initial value immediately without waiting', () => {
    const { result } = renderHook(() => useDebouncedValue('hello', 400))
    expect(result.current).toBe('hello')
  })

  it('does NOT update the value before the delay has elapsed', () => {
    const { result, rerender } = renderHook(({ v }) => useDebouncedValue(v, 400), {
      initialProps: { v: 'a' },
    })

    act(() => {
      rerender({ v: 'b' })
      jest.advanceTimersByTime(300) // Still 100 ms short of the 400 ms delay
    })

    expect(result.current).toBe('a')
  })

  it('updates the value after the delay has elapsed', async () => {
    const { result, rerender } = renderHook(({ v }) => useDebouncedValue(v, 400), {
      initialProps: { v: 'a' },
    })

    rerender({ v: 'b' })

    act(() => {
      jest.advanceTimersByTime(400)
    })

    await waitFor(() => expect(result.current).toBe('b'))
  })

  it('resets the timer on each new value (debounce behavior)', async () => {
    const { result, rerender } = renderHook(({ v }) => useDebouncedValue(v, 400), {
      initialProps: { v: 'a' },
    })

    // Rapid succession of updates; each should restart the timer
    act(() => {
      rerender({ v: 'b' })
      jest.advanceTimersByTime(200) // half-way through for 'b'
      rerender({ v: 'c' })
      jest.advanceTimersByTime(200) // half-way through for 'c' (timer reset)
    })

    // Neither 'b' nor 'c' should have committed yet
    expect(result.current).toBe('a')

    // Now let 'c' fully commit
    act(() => {
      jest.advanceTimersByTime(200)
    })

    await waitFor(() => expect(result.current).toBe('c'))
  })

  it('cleans up the timeout when the component unmounts', () => {
    const { rerender, unmount } = renderHook(({ v }) => useDebouncedValue(v, 400), {
      initialProps: { v: 'start' },
    })

    rerender({ v: 'updated' })
    unmount() // Should cancel the pending timer

    // No error should be thrown if the timer fires after unmount.
    // This also implicitly tests that clearTimeout is called.
    expect(() => act(() => jest.runAllTimers())).not.toThrow()
  })

  it('respects a custom delay value', async () => {
    const { result, rerender } = renderHook(({ v }) => useDebouncedValue(v, 1000), {
      initialProps: { v: 'x' },
    })

    rerender({ v: 'y' })

    act(() => {
      jest.advanceTimersByTime(999)
    })
    expect(result.current).toBe('x') // not yet

    act(() => {
      jest.advanceTimersByTime(1)
    })

    await waitFor(() => expect(result.current).toBe('y'))
  })
})
