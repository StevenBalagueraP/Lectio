import { act, renderHook, waitFor } from '@testing-library/react'
import { useDebouncedValue } from './useDebouncedValue'


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
      jest.advanceTimersByTime(300)
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

    act(() => {
      rerender({ v: 'b' })
      jest.advanceTimersByTime(200)
      rerender({ v: 'c' })
      jest.advanceTimersByTime(200) 
    })

    expect(result.current).toBe('a')

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
    unmount() 
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
    expect(result.current).toBe('x') 

    act(() => {
      jest.advanceTimersByTime(1)
    })

    await waitFor(() => expect(result.current).toBe('y'))
  })
})
