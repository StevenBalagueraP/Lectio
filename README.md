# Testing with Jest + React Testing Library (LectioFrontend)

## 1. Install the dev dependencies

```bash
npm install --save-dev jest jest-environment-jsdom @types/jest \
  @testing-library/react @testing-library/dom @testing-library/jest-dom @testing-library/user-event \
  babel-jest @babel/core @babel/preset-env @babel/preset-react @babel/preset-typescript identity-obj-proxy
```

## 2. Copy the files from the zip (keeping the same paths)

- `jest.config.cjs` and `jest.babel-import-meta.cjs` → project root
- `src/test/*` → test utilities (polyfills, mocks, `mockFetch`)
- `*.test.ts(x)` → next to each component, hook, or page they test

## 3. Small changes to existing files

**package.json** → add these to `"scripts"`:

```json
"test": "jest",
"test:watch": "jest --watch",
"test:coverage": "jest --coverage"
```

**tsconfig.app.json** → in `compilerOptions`, replace `"types"` with:

```json
"types": ["vite/client", "node", "jest", "@testing-library/jest-dom"],
```

**eslint.config.js** → `globalIgnores(['dist', 'coverage'])`

**.gitignore** → add a `coverage` line

## 4. Run the tests

```bash
npm test
npm run test:coverage
```

## What is tested

| File | Type | What it covers |
|---|---|---|
| `LoanButton.test.tsx` | Unit (component) | Label and state for each LoanStatus, loading, error, click |
| `BookGrid.test.tsx` | Unit (component) | Error, skeletons, empty state, cards, https covers, onOpen, "stale" state |
| `ReviewForm.test.tsx` | Unit (component) | Stars, validation, trimmed comment, counter, submit, cancel |
| `useFetch.test.ts` | Unit (hook) | States, abort signal, de-duplication, TTL, expired data, `keepPrevious` |
| `BookReviewsPage.test.tsx` | Integration | Page + router + hooks + cache + `http`, with only `fetch` mocked |

---

# Test documentation

This section describes what the test suite covers, how it is set up, and the latest results.

## Overview

The frontend tests use **Jest** as the test runner and **React Testing Library** to render components and interact with them the way a user would. There are five test suites with 50 tests in total:

| Suite | Tests | Level |
|---|---|---|
| `LoanButton.test.tsx` | 9 | Component |
| `BookGrid.test.tsx` | 9 | Component |
| `ReviewForm.test.tsx` | 12 | Component |
| `useFetch.test.ts` | 10 | Hook |
| `BookReviewsPage.test.tsx` | 10 | Integration |

The component and hook tests check one piece in isolation. The `BookReviewsPage` suite is different: it renders the real page with the router, hooks, cache, and HTTP layer all wired together, and only `fetch` is mocked. That makes it the closest thing to an end-to-end check in the suite without needing a browser or a running backend.

## What each suite covers

### LoanButton

- Shows the correct label and enabled/disabled state for each loan status:
  - `LoginRequired` → "Log in to borrow" (enabled)
  - `Available` → "Borrow" (enabled)
  - `BorrowedByYou` → "Deliver" (enabled)
  - `Unavailable` → "Unavailable" (disabled)
- Is disabled and marked as busy while the status is loading.
- Shows "Status unavailable" when the request failed or when there is no status yet.
- Calls `onAction` with the current status when clicked.
- Does not call `onAction` when the button is disabled.

### BookGrid

- Shows only the error message, with no books, when there is an error.
- Shows 4 skeleton cards by default while loading without data, and respects a custom `skeletonCount`.
- Shows the default empty message when there are no books, and a custom message when one is provided.
- Renders one card per book.
- Upgrades cover URLs from `http` to `https`.
- Calls `onOpen` with the book id when "See More" is pressed.
- Keeps showing the previous books (dimmed) while a new page is loading.

### ReviewForm

- Starts with no rating selected and the Submit button disabled.
- Selecting a rating enables Submit.
- Submits the star rating together with the trimmed comment.
- Allows an empty comment, since it is optional.
- Does not submit without a rating, including when a submit event is fired directly to bypass the disabled button.
- Ignores a submit event while a request is already in progress.
- Updates the character counter and limits the comment to 1000 characters.
- Disables the buttons and shows "Sending…" while submitting.
- Shows the error message passed in through props, and renders no alert when there is none.
- Calls `onCancel` when Cancel is pressed.

### useFetch

- Goes from loading to data.
- Passes an `AbortSignal` to the fetcher.
- Exposes the message of an error thrown by the fetcher, and falls back to a generic message when the rejection is not an `Error`.
- Does not run the fetcher when `enabled` is `false`.
- Shares a single in-flight request between hooks that use the same key (when `ttl > 0`).
- Returns cached data while the TTL has not expired.
- Falls back to stale data when a refetch fails.
- Refetches when the key changes and hides the old data, unless `keepPrevious` is set.
- Keeps the previous data while the next key loads when `keepPrevious` is `true`.

### BookReviewsPage (integration)

- Loads the book, the average rating, and the list of reviews.
- Shows an error with a link back home when the book does not exist.
- Shows a message when the book has no reviews yet.
- Shows an alert when the reviews cannot be loaded.
- Sends a logged-out user to `/login` when they press "Add Review".
- Lets a logged-in user publish a review, then refreshes the list.
- Sends a `null` comment when the user leaves the comment empty.
- Shows the server's message inside the form when the backend rejects the review.
- Asks the user to log in again when the session has expired (401).
- Closes the form without sending anything when Cancel is pressed.

## Test setup

### Jest configuration (`jest.config.cjs`)

- **Environment:** `jsdom`, so components can render as they would in a browser.
- **Transform:** `babel-jest` with `@babel/preset-env`, `@babel/preset-react` (automatic runtime), and `@babel/preset-typescript`. The same transform also handles `react-router` and `@remix-run/*`, which are published as ESM only, so `transformIgnorePatterns` lets those packages through.
- **Setup files:** `src/test/polyfills.ts` runs before each suite, and `src/test/setupTests.ts` runs after the test framework is installed.
- **Module mapping:** CSS modules are mapped to `identity-obj-proxy`, plain CSS to a stub, and a few modules to mocks (see below).
- **`clearMocks: true`**, so mock call history is reset between tests.

### Test utilities (`src/test/`)

| File | Purpose |
|---|---|
| `polyfills.ts` | Adds `TextEncoder` and `TextDecoder` to the global scope, which jsdom does not provide. |
| `setupTests.ts` | Loads `@testing-library/jest-dom` for matchers like `toBeDisabled()` and `toBeInTheDocument()`. |
| `styleMock.ts` | Empty stand-in for imported `.css` files. |
| `cookieEsMock.ts` | Stand-in for `cookie-es`, which `react-router` imports but which only ships as `.mjs`. |
| `envMock.ts` | Replaces `config/env.ts`, because Jest does not understand Vite's `import.meta.env`. |
| `mockFetch.ts` | Helpers to fake `fetch`: `mockFetch(handler)`, `jsonResponse(body, status)`, and `emptyResponse(status)`. |

`jest.babel-import-meta.cjs`, in the project root, is a small Babel plugin that rewrites `import.meta` to an object backed by `process.env`, so code written for Vite can run under Jest.

## Results

### Test run

```text
> lectiofrontend@0.0.0 test
> jest

 PASS  src/features/book-detail/components/LoanButton/Loanbutton.test.tsx
 PASS  src/shared/components/BookGrid/Bookgrid.test.tsx
 PASS  src/features/book-reviews/components/ReviewForm/Reviewform.test.tsx
 PASS  src/shared/hooks/useFetch.test.ts
 PASS  src/features/book-reviews/pages/BookReviewsPage.test.tsx

Test Suites: 5 passed, 5 total
Tests:       50 passed, 50 total
Snapshots:   0 total
Time:        3.363 s
Ran all test suites.
```

All 5 suites and all 50 tests pass.

### Coverage

Overall coverage from `npm run test:coverage`:

| Statements | Branches | Functions | Lines |
|---|---|---|---|
| 69.29% | 68.46% | 73.52% | 72.84% |

Coverage for the code the suites target directly:

| File | Statements | Branches | Functions | Lines |
|---|---|---|---|---|
| `LoanButton.tsx` | 100% | 91.66% | 100% | 100% |
| `BookGrid.tsx` | 100% | 100% | 100% | 100% |
| `ReviewForm.tsx` | 100% | 100% | 100% | 100% |
| `useFetch.ts` | 95.83% | 94.28% | 100% | 100% |
| `BookReviewsPage.tsx` | 91.37% | 92.1% | 83.33% | 92.85% |

Because the page test runs the real components underneath it, several other files are covered as a side effect, including `RatingSummary`, `ReviewCard`, `ReviewList`, `BookCard`, `Button`, `RatingStars`, and `useSession` (all at 100%).

## Known gaps

These areas have no dedicated tests yet and are only partly covered through the page test:

- `Pagination.tsx`: 13.33% statements
- `api/endpoints/books.ts`: 31.25% statements
- `api/token.ts`: 43.47% statements
- `api/http.ts`: 45.9% statements
- `useGoBack.ts`: 40% statements
- `shared/utils/cache.ts`: 67.64% statements

The next tests worth adding would be for `Pagination`, the token handling in `api/token.ts`, and the error paths in `api/http.ts`.