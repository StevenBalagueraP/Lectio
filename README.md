# Lectio

> A university library app: browse the catalogue, borrow and return books, follow your loan history and publish reviews.

**Lectio** is the final project of the university course **Desarrollo Web** (Jala University). It is made of two parts that live in the same repository (`https://github.com/StevenBalagueraP/Lectio`):

| Part | What it is | Stack |
| --- | --- | --- |
| **LectioBackend** | REST API: catalogue, authentication, loans and ratings | ASP.NET Core (`net10.0`), EF Core, PostgreSQL on Supabase |
| **LectioFrontend** | Single-page web client that consumes the API | React 19, TypeScript, Vite |

Every screen of the frontend is backed by a real endpoint of the API: the catalogue, the book detail, the loan status, the loans and the ratings never come from static data.

## Table of Contents

- [Design (Figma)](#design-figma)
- [Features](#features)
- [Repository Structure](#repository-structure)
- [Technologies](#technologies)
- [Installation and Running](#installation-and-running)
- [Architecture](#architecture)
- [Authentication](#authentication)
- [Database Model](#database-model)
- [Testing](#testing)
- [How to Contribute](#how-to-contribute)
- [License](#license)

## Design (Figma)

The UI mock-ups live in Figma:

https://www.figma.com/design/VWf8Ie1JsaLGxFNFHw4pIY/Web-Development---Book-store-Mock-ups?node-id=0-1&p=f&t=aDfMx0cy5LG7kPe2-0

## Features

A reader can:

- See the top-rated books and browse the catalogue by title and category, with filters and pagination kept in the URL.
- Open the detail of a book and read its summary, basic info and reviews.
- Borrow and return books, and follow the loan history in "My Library".
- Read and publish reviews (rating from 1 to 5 and an optional comment).
- Register, log in, see the profile and log out.

| Path | Screen | Needs a session |
| --- | --- | --- |
| `/` | Home: hero carousel, category shortcuts and top-rated books | No |
| `/categories` | Catalogue: search, category and book filters, pagination | No |
| `/books/:bookId` | Book detail | Only to borrow or return |
| `/books/:bookId/loan` | Borrow: review the checkout and due dates and confirm | Yes |
| `/books/:bookId/reviews` | Reviews: average rating, review list and review form | Only to publish |
| `/history` | "My Library": loans with their status; return or rate a book | Yes |
| `/login` | Sign in with email and password | No |
| `/register` | Create an account | No |
| `/profile` | User name, menu and log out | Yes |

When a signed-out user tries a protected action, the app sends them to `/login` and returns them to the original page after signing in.

## Repository Structure

```text
Lectio/
├── LectioBackend/
│   ├── Application/       Use cases: business rules, services, DTOs and validation
│   ├── Controllers/       HTTP layer (thin): /api/auth, /api/books, /api/categories, /api/users, /api/seed
│   ├── Domain/            Entities and business-rule exceptions
│   ├── Infrastructure/    AppDbContext, EF Core mappings, external clients and migrations
│   ├── Middlewares/       Exception middleware (application/problem+json)
│   └── Program.cs         DI, authentication, CORS, rate limiting and endpoints
└── LectioFrontend/
    ├── src/api/           http wrapper, endpoint modules per resource and API types
    ├── src/config/        env.ts, the typed environment variables
    ├── src/features/      One folder per screen (home, categories, book-detail, book-reviews, loan, history, login, register, profile)
    ├── src/routes/        The router and AppLayout
    ├── src/shared/        Components, hooks and utils reused across features
    └── src/test/          Jest setup, the fetch mock and the module mocks
```

## Technologies

### Backend

| Technology | Purpose |
| --- | --- |
| .NET / ASP.NET Core Web API (`net10.0`) | Runtime, web framework and controllers |
| Entity Framework Core + Npgsql | Data access (Code First) on PostgreSQL |
| PostgreSQL on Supabase | Database, with Row Level Security enabled in the migrations |
| JwtBearer + BCrypt.Net-Next | JWT authentication and password hashing |
| Swashbuckle (Swagger UI) | Interactive API documentation at `/swagger` in Development |
| Google Books API | Book data source, used only by the seeders |
| Render | Backend deployment |

### Frontend

| Technology | Purpose |
| --- | --- |
| React 19 + TypeScript (strict) | UI with function components and hooks |
| Vite 8 | Dev server and production build |
| react-router 8 | Client-side routing with a data router |
| CSS Modules + design tokens | Styling, with no CSS framework |
| Own `fetch` wrapper + `useFetch` hook | Data fetching, with no Axios or React Query |
| Jest + React Testing Library | Unit and integration tests |
| Vercel | Frontend deployment (SPA fallback in `vercel.json`) |

The frontend deliberately has no global state library (no Redux, Zustand or React Query), no Axios and no UI kit: the state lives in the URL, in local component state and in `useSession()`.

## Installation and Running

### Prerequisites

- **Git**.
- **.NET SDK 10** and a **PostgreSQL** database (the project is set up for Supabase) for the backend.
- **Node.js** `^20.19.0 || >=22.12.0` and **npm** for the frontend.
- A Google Books API key, only if you want to run the seeders.

```bash
git clone https://github.com/StevenBalagueraP/Lectio
cd Lectio
```

### 1. Backend

Set the required configuration with user-secrets (never commit real values):

```bash
cd LectioBackend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
dotnet user-secrets set "Jwt:Secret" "<your-random-secret-32-chars>"
dotnet user-secrets set "GoogleBooks:ApiKey" "<your-google-books-key>"
```

Create the schema (the migrations also enable Row Level Security) and run the API:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
dotnet run
```

The API listens on `http://localhost:5031` (and `https://localhost:7026`). In Development, Swagger UI is available at `/swagger`.

Optional keys: `Jwt:Issuer`, `Jwt:Audience`, `Jwt:AccessTokenMinutes` (default 15), `Jwt:RefreshTokenDays` (default 7) and `Cors:Origins` (default `http://localhost:5173`).

### 2. Frontend

```bash
cd LectioFrontend
npm install
```

Create a `.env` file in the frontend root (it is ignored by Git):

```bash
VITE_API_URL=http://localhost:5031
```

| Variable | Purpose | Fallback when missing |
| --- | --- | --- |
| `VITE_API_URL` | Base URL of the Lectio REST API | `http://localhost:5031` |

```bash
npm run dev        # dev server on http://localhost:5173
npm run build      # tsc -b + vite build, output in dist/
npm run preview    # serve the production bundle locally
npm run lint
```

The backend only accepts requests from the origins listed in `Cors:Origins`, so keep the frontend on `http://localhost:5173` (the Vite default) or update that setting.

## Architecture

```text
Browser (React SPA)
   │  Page -> feature hook -> useFetch (cache + TTL + AbortController) -> endpoint -> http.ts
   │  HTTPS + JSON
   ▼
ASP.NET Core pipeline  (CORS, rate limiter, exception middleware, JWT bearer, authorization)
   ▼
Controller  ->  Service (Application/)  ->  AppDbContext (EF Core)  ->  PostgreSQL (Supabase)
```

- **Backend:** layered API. Controllers stay thin, business rules live in `Application/` services, and services use `AppDbContext` directly (no Repository or Unit of Work). A single exception middleware answers `application/problem+json`. Rate limiting is global, with a stricter policy for auth endpoints.
- **Frontend:** client-side SPA. Every call goes through `http<T>()` in `src/api/`; no component calls `fetch` directly. `shared/` never imports from `features/`.

## Authentication

Authentication is hand-made (no ASP.NET Core Identity): a custom `User` table with BCrypt hashing, a JWT (HS256) access token and an opaque, single-use, rotating refresh token stored hashed in the database. The access token lasts **15 minutes** and the refresh token **7 days** (both configurable).

| Method | Path | Request | Response |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | `fullName`, `email`, `password`, `phone` (optional) | Token pair and user |
| POST | `/api/auth/login` | `email`, `password` | Token pair and user |
| POST | `/api/auth/refresh` | `refreshToken` | New token pair |
| POST | `/api/auth/logout` | `refreshToken` | `204` |

On the frontend, the tokens and the user are kept in `localStorage`. `http.ts` sends the access token on protected requests, renews the pair automatically when it is about to expire (30-second margin) or when a request fails with `401`, and retries the original request once. The renewal is serialized across tabs with the `lectio:refresh` lock, and logout always clears the local session, even if the request fails.

Public endpoints: `/api/auth/*`, `/api/books/*`, `/api/categories` and `/health`. The `/api/users/*` endpoints (loans and ratings) require a valid JWT, and you can only act on your own user unless you hold the `admin` role.

## Database Model

| Table | Purpose |
| --- | --- |
| `Book`, `Author`, `BookAuthor` | Catalogue and its authors (many-to-many) |
| `Category`, `BookCategory` | Categories and their books (many-to-many) |
| `User` | Accounts with a role (`user` by default) |
| `Loan` | Borrow and return of a book; only one active loan per book (unique index where `ReturnDate` is null) |
| `Rating` | One rating per user and book, linked to a loan; keeps `Stars` and an optional `Comment` |
| `RefreshToken` | Hashed, rotating refresh tokens per user |

## Testing

The frontend tests are **unit** and **integration** tests built with **Jest** and **React Testing Library**. Integration suites render the real page with the router, hooks, cache and HTTP layer wired together; only `fetch` is mocked, so no browser or running backend is needed.

### How to run

From the `LectioFrontend` folder:

```bash
npm test                 # run all suites
npm run test:watch       # watch mode
npm run test:coverage    # run with the coverage report
```

To filter by file or test name:

```bash
npm test -- Pagination
npm test -- --testNamePattern="shows an error"
```

### Suites

| Suite | Type | What it covers | Tests |
| --- | --- | --- | --- |
| `Bookgrid.test.tsx` | Unit | `BookGrid`: error, empty and loading states, one card per book, cover URL, `onOpen` | 9 |
| `Pagination.test.tsx` | Unit | `Pagination`: hidden for one page, disabled arrows, `aria-current`, `onChange`, ellipsis | 11 |
| `RatingStars.test.tsx` | Unit | `RatingStars`: accessible label, clamping to 0–5, decimals and the large size | 8 |
| `useFetch.test.ts` | Unit | `useFetch`: loading to data, abort signal, error message, `enabled`, in-flight de-duplication, TTL and stale fallback | 10 |
| `useDebouncedValue.test.ts` | Unit | `useDebouncedValue` with fake timers: initial value, delay, reset and cleanup | 6 |
| `Loanbutton.test.tsx` | Unit | `LoanButton`: label and state per loan status, disabled while loading, error state and `onAction` | 9 |
| `Reviewform.test.tsx` | Unit | `ReviewForm`: rating required, submit with stars and comment, character limit and `onCancel` | 12 |
| `validation.test.ts` | Unit | `isValidEmail` and `validateLogin` | 14 |
| `BookReviewsPage.test.tsx` | Integration | The reviews page with the real router and hooks; only `fetch` is mocked | 10 |
| `CategoriesPage.test.tsx` | Integration | The catalogue page: list, count, error, empty state, filters and navigation | 10 |
| `LoginPage.test.tsx` | Integration | The login form: fields, validation, 401, 500, success redirect and in-flight state | 8 |

### Results

A real run of `npm test` (2026-10):

```text
Test Suites: 11 passed, 11 total
Tests:       107 passed, 107 total
```

Coverage from `npm run test:coverage`:

| Statements | Branches | Functions | Lines |
| --- | --- | --- | --- |
| 80.35% | 72.68% | 81.03% | 84.26% |

Coverage only measures the files that the tests load, because `jest.config.cjs` does not set `collectCoverageFrom`. The least covered part is the session renewal in `src/api/http.ts` (45.9% statements), followed by `src/api/token.ts` (78.26%).

### Setup notes

- **Environment:** `jsdom`, with `babel-jest` (`preset-env`, `preset-react` and `preset-typescript`).
- **`fetch` is mocked** with the helpers in `src/test/mockFetch.ts` (`mockFetch`, `jsonResponse`, `emptyResponse`); `clearMocks: true` resets it between tests.
- **`localStorage` is cleaned** after each test in `src/test/setupTests.ts`.
- **CSS Modules** are mocked with `identity-obj-proxy`, `import.meta.env` is mapped to `src/test/envMock.ts` and `cookie-es` to `src/test/cookieEsMock.ts`, all in `jest.config.cjs`.
- **Cache isolation:** `src/shared/utils/cache.ts` keeps a module-level `Map`, so integration suites use a unique cache key per test.

## How to Contribute

1. Fork the repository, clone your fork and create a branch (`feature/...` or `fix/...`).
2. Make the change following the existing conventions.
3. Run the checks before opening a pull request:

   ```bash
   # Backend
   dotnet build --nologo

   # Frontend
   npm run build
   npm run lint
   npm test
   ```

4. Never commit secrets, `.env` values, tokens or passwords.
5. Open a pull request explaining what changes and why. Discuss any new package (NuGet or npm) in the pull request, because both projects keep their dependency lists small.

Conventions:

- **Backend:** thin controllers, business rules in `Application/` services, DTOs validated with DataAnnotations, `async`/`await` with the `Async` suffix, nullable reference types enabled, UTC dates. Schema changes are new EF Core migrations. Identity, Repository/Unit of Work, MediatR and AutoMapper are deliberately not used.
- **Frontend:** named exports with a local `interface NameProps`, one folder per component (`Name.tsx` + `Name.module.css`), `import type` for type-only imports, no `enum` or `namespace`, design tokens (`var(--...)`) instead of hard-coded colors, and API access only through `src/api/`.

## License

This project is licensed under the MIT license.