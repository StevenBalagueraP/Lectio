import '@testing-library/jest-dom'

beforeEach(() => {
  // jsdom no implementa scrollTo (lo usan CategoriesPage y BookReviewsPage).
  window.scrollTo = jest.fn()
})

afterEach(() => {
  localStorage.clear()
})
