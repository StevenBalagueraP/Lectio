import '@testing-library/jest-dom'

beforeEach(() => {
  window.scrollTo = jest.fn()
})

afterEach(() => {
  localStorage.clear()
})
