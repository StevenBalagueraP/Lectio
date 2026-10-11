# FRONTEND_FILES.md — Código fuente completo de LectioFrontend

> Snapshot del código actual del frontend (actualizado el 2026-10-11: **My Library (History) — bug fixes** — formato de fechas consistente sin año en `formatShortDate` y línea "Returned:" en tarjetas devueltas; también se completan las entradas faltantes del snapshot anterior: `HistoryPage.tsx`, `LoanCard.tsx`, `LoanCard.module.css`, `date.ts`, `src/api/endpoints/loans.ts` y archivos de `history/`). Cada archivo está copiado **literalmente** desde el repositorio, sin resumir ni modificar.
> Complemento de `FRONTEND_CONTEXT.md` (explicación y convenciones). Este archivo contiene solo el código.

## Notas de alcance

- **Excluidos a propósito:** `node_modules/`, `dist/`, `package-lock.json` (generado), `.gitkeep` (vacíos en `src/features`, `src/routes`, `src/shared`) y `README.md` (documentación obsoleta que no refleja el código real).
- **`.env`:** existe en el proyecto; no se copia su contenido. Variable usada: `VITE_API_URL` (ver `src/config/env.ts`).
- **Imágenes / binarios:** no hay en el zip entregado. No existe `public/`; `index.html` referencia `/favicon.svg` `[UNKNOWN / NEEDS VERIFICATION]` (el archivo no estaba en el zip).
- **Contextos / Providers:** no existen en el código (no hay Context API, ni store). La sesión se comparte con `useSyncExternalStore` (`src/api/token.ts` + `src/shared/hooks/useSession.ts`). La sección 10 queda vacía a propósito.
- **Código muerto (incluido por fidelidad):** solo `src/features/home/components/BookList/*` (nadie lo importa). `src/App.tsx` y `src/App.css` **ya no existen** en el repo (se eliminaron en una iteración anterior).
- **Marcas `(nuevo)` / `(modificado)`:** se calculan comparando contra el snapshot anterior (iteración de autenticación). Los marcadores anteriores se reemplazaron por los de esta iteración.

## Cambios de esta iteración (respecto al snapshot anterior)

Esta iteración corrige dos bugs en la página **My Library** (`/history`) y completa las entradas de código que faltaban en el snapshot anterior.

**Modificados (2):**

- `src/shared/utils/date.ts` — `formatShortDate` ahora acepta `string | null | undefined` y **siempre** devuelve `"Mes Día"` sin año (se elimina la lógica condicional del año).
- `src/features/history/components/LoanCard/LoanCard.tsx` — agrega línea `Returned: <fecha>` en el bloque `.dates` para préstamos con `status === "Returned"`, usando el mismo estilo `.date` que `Borrowed` y `Due Date`.

**Entradas de código añadidas al documento (faltaban en el snapshot anterior):**

- `src/features/history/pages/HistoryPage.tsx` — se reemplaza el stub por el código real.
- `src/features/history/pages/HistoryPage.module.css` — nuevo en el documento.
- `src/features/history/components/LoanCard/LoanCard.tsx` — nuevo en el documento.
- `src/features/history/components/LoanCard/LoanCard.module.css` — nuevo en el documento.
- `src/features/history/hooks.ts` — nuevo en el documento.
- `src/features/history/constants.ts` — nuevo en el documento.
- `src/api/endpoints/loans.ts` — nuevo en el documento.
- `src/shared/utils/date.ts` — nuevo en el documento.

**Sin cambios:**

## Árbol real del proyecto

```text
LectioFrontend/
├── src
│   ├── api
│   │   ├── endpoints
│   │   │   ├── auth.ts   (nuevo)
│   │   │   ├── books.ts
│   │   │   ├── categories.ts
│   │   │   └── ratings.ts
│   │   ├── types
│   │   │   ├── auth.ts   (nuevo)
│   │   │   ├── book.ts
│   │   │   ├── category.ts
│   │   │   ├── common.ts
│   │   │   ├── loan.ts
│   │   │   └── rating.ts
│   │   ├── http.ts   (modificado)
│   │   └── token.ts   (modificado)
│   ├── config
│   │   └── env.ts
│   ├── features
│   │   ├── book-detail
│   │   │   ├── components
│   │   │   │   ├── BookInfoList
│   │   │   │   │   ├── BookInfoList.module.css
│   │   │   │   │   └── BookInfoList.tsx
│   │   │   │   ├── BookTabs
│   │   │   │   │   ├── BookTabs.module.css
│   │   │   │   │   └── BookTabs.tsx
│   │   │   │   ├── ExpandableText
│   │   │   │   │   ├── ExpandableText.module.css
│   │   │   │   │   └── ExpandableText.tsx
│   │   │   │   └── LoanButton
│   │   │   │       └── LoanButton.tsx
│   │   │   ├── pages
│   │   │   │   ├── BookDetailPage.module.css
│   │   │   │   └── BookDetailPage.tsx   (modificado)
│   │   │   └── constants.ts
│   │   ├── book-reviews
│   │   │   ├── components
│   │   │   │   ├── RatingSummary
│   │   │   │   │   ├── RatingSummary.module.css
│   │   │   │   │   └── RatingSummary.tsx
│   │   │   │   ├── ReviewCard
│   │   │   │   │   ├── ReviewCard.module.css
│   │   │   │   │   └── ReviewCard.tsx
│   │   │   │   ├── ReviewForm
│   │   │   │   │   ├── ReviewForm.module.css
│   │   │   │   │   └── ReviewForm.tsx
│   │   │   │   └── ReviewList
│   │   │   │       ├── ReviewList.module.css
│   │   │   │       └── ReviewList.tsx
│   │   │   ├── pages
│   │   │   │   ├── BookReviewsPage.module.css
│   │   │   │   └── BookReviewsPage.tsx   (modificado)
│   │   │   ├── constants.ts
│   │   │   ├── hooks.ts
│   │   │   └── utils.ts
│   │   ├── categories
│   │   │   ├── components
│   │   │   │   └── CategoryFilter
│   │   │   │       ├── CategoryFilter.module.css
│   │   │   │       └── CategoryFilter.tsx
│   │   │   ├── pages
│   │   │   │   ├── CategoriesPage.module.css
│   │   │   │   └── CategoriesPage.tsx
│   │   │   └── hooks.ts
│   │   ├── history
│   │   │   └── pages
│   │   │       └── HistoryPage.tsx
│   │   ├── home
│   │   │   ├── components
│   │   │   │   ├── BookList
│   │   │   │   │   ├── BookList.module.css   (código muerto: nadie lo importa)
│   │   │   │   │   └── BookList.tsx   (código muerto: nadie lo importa)
│   │   │   │   ├── CategoryShortcuts
│   │   │   │   │   ├── CategoryShortcuts.module.css
│   │   │   │   │   └── CategoryShortcuts.tsx
│   │   │   │   └── HeroCarousel
│   │   │   │       ├── HeroCarousel.module.css
│   │   │   │       └── HeroCarousel.tsx
│   │   │   ├── pages
│   │   │   │   ├── HomePage.module.css
│   │   │   │   └── HomePage.tsx
│   │   │   ├── constants.ts
│   │   │   └── hooks.ts
│   │   ├── login
│   │   │   ├── pages
│   │   │   │   └── LoginPage.tsx   (modificado)
│   │   │   └── utils.ts   (nuevo)
│   │   ├── profile   (nuevo)
│   │   │   ├── pages
│   │   │   │   ├── ProfilePage.module.css   (nuevo)
│   │   │   │   └── ProfilePage.tsx   (nuevo)
│   │   │   ├── constants.ts   (nuevo)
│   │   │   └── hooks.ts   (nuevo)
│   │   ├── register   (nuevo)
│   │   │   ├── pages
│   │   │   │   └── RegisterPage.tsx   (nuevo)
│   │   │   └── utils.ts   (nuevo)
│   │   └── .gitkeep   (vacío)
│   ├── routes
│   │   ├── .gitkeep   (vacío)
│   │   ├── AppLayout.tsx   (modificado)
│   │   └── index.tsx   (modificado)
│   ├── shared
│   │   ├── components
│   │   │   ├── AuthForm   (nuevo)
│   │   │   │   ├── AuthForm.module.css   (nuevo)
│   │   │   │   └── AuthForm.tsx   (nuevo)
│   │   │   ├── BookCard
│   │   │   │   ├── BookCard.module.css
│   │   │   │   └── BookCard.tsx
│   │   │   ├── BookGrid
│   │   │   │   ├── BookGrid.module.css
│   │   │   │   └── BookGrid.tsx
│   │   │   ├── BookHero
│   │   │   │   ├── BookHero.module.css
│   │   │   │   └── BookHero.tsx
│   │   │   ├── Button
│   │   │   │   ├── Button.module.css
│   │   │   │   └── Button.tsx
│   │   │   ├── MobileMenu
│   │   │   │   ├── MobileMenu.module.css
│   │   │   │   └── MobileMenu.tsx
│   │   │   ├── Navbar
│   │   │   │   ├── Navbar.module.css   (modificado)
│   │   │   │   ├── Navbar.tsx   (modificado)
│   │   │   │   └── navItems.ts   (modificado)
│   │   │   ├── Pagination
│   │   │   │   ├── Pagination.module.css
│   │   │   │   └── Pagination.tsx
│   │   │   ├── RatingStars
│   │   │   │   ├── RatingStars.module.css
│   │   │   │   └── RatingStars.tsx
│   │   │   ├── Skeleton
│   │   │   │   ├── Skeleton.module.css
│   │   │   │   └── Skeleton.tsx
│   │   │   └── TextField   (nuevo)
│   │   │       ├── TextField.module.css   (nuevo)
│   │   │       └── TextField.tsx   (nuevo)
│   │   ├── hooks
│   │   │   ├── useBookDetail.ts
│   │   │   ├── useBookLoanStatus.ts   (modificado)
│   │   │   ├── useCategories.ts
│   │   │   ├── useDebouncedValue.ts
│   │   │   ├── useFetch.ts
│   │   │   ├── useGoBack.ts
│   │   │   └── useSession.ts   (nuevo)
│   │   ├── utils
│   │   │   ├── apiError.ts   (nuevo)
│   │   │   ├── book.ts
│   │   │   ├── cache.ts
│   │   │   ├── redirect.ts   (nuevo)
│   │   │   └── validation.ts   (nuevo)
│   │   └── .gitkeep   (vacío)
│   ├── index.css
│   └── main.tsx
├── .env   (valores omitidos — solo se documenta el nombre VITE_API_URL)
├── .gitignore
├── eslint.config.js
├── index.html
├── package.json
├── README.md   (no incluido: obsoleto, ver FRONTEND_CONTEXT.md)
├── tsconfig.app.json
├── tsconfig.json
├── tsconfig.node.json
└── vite.config.ts
```

## Índice de archivos (orden del documento)

**1. Configuración y entry points**
- `index.html` (19 líneas)
- `package.json` (32 líneas)
- `tsconfig.json` (7 líneas)
- `tsconfig.app.json` (26 líneas)
- `tsconfig.node.json` (23 líneas)
- `vite.config.ts` (7 líneas)
- `eslint.config.js` (22 líneas)
- `.gitignore` (24 líneas)
- `src/config/env.ts` (3 líneas)
- `src/main.tsx` (11 líneas)

**2. App**
- _(ninguno — `src/App.tsx` y `src/App.css` fueron eliminados)_

**3. Routing**
- `src/routes/index.tsx` (26 líneas) **(modificado)**

**4. Estilos globales**
- `src/index.css` (103 líneas)

**5. Layouts**
- `src/routes/AppLayout.tsx` (16 líneas) **(modificado)**

**6. Páginas**
- `src/features/home/pages/HomePage.tsx` (61 líneas)
- `src/features/home/pages/HomePage.module.css` (47 líneas)
- `src/features/categories/pages/CategoriesPage.tsx` (135 líneas)
- `src/features/categories/pages/CategoriesPage.module.css` (65 líneas)
- `src/features/book-detail/pages/BookDetailPage.tsx` (97 líneas) **(modificado)**
- `src/features/book-detail/pages/BookDetailPage.module.css` (92 líneas)
- `src/features/book-reviews/pages/BookReviewsPage.tsx` (197 líneas) **(modificado)**
- `src/features/book-reviews/pages/BookReviewsPage.module.css` (174 líneas)
- `src/features/login/pages/LoginPage.tsx` (85 líneas) **(modificado)**
- `src/features/register/pages/RegisterPage.tsx` (112 líneas) **(nuevo)**
- `src/features/profile/pages/ProfilePage.tsx` (45 líneas) **(nuevo)**
- `src/features/profile/pages/ProfilePage.module.css` (104 líneas) **(nuevo)**
- `src/features/history/pages/HistoryPage.tsx` (3 líneas)

**7. Componentes compartidos (src/shared/components)**
- `src/shared/components/BookCard/BookCard.tsx` (40 líneas)
- `src/shared/components/BookCard/BookCard.module.css` (112 líneas)
- `src/shared/components/BookGrid/BookGrid.tsx` (47 líneas)
- `src/shared/components/BookGrid/BookGrid.module.css` (45 líneas)
- `src/shared/components/RatingStars/RatingStars.tsx` (36 líneas)
- `src/shared/components/RatingStars/RatingStars.module.css` (31 líneas)
- `src/shared/components/Pagination/Pagination.tsx` (67 líneas)
- `src/shared/components/Pagination/Pagination.module.css` (49 líneas)
- `src/shared/components/Navbar/Navbar.tsx` (71 líneas) **(modificado)**
- `src/shared/components/Navbar/Navbar.module.css` (164 líneas) **(modificado)**
- `src/shared/components/Navbar/navItems.ts` (17 líneas) **(modificado)**
- `src/shared/components/MobileMenu/MobileMenu.tsx` (94 líneas)
- `src/shared/components/MobileMenu/MobileMenu.module.css` (125 líneas)
- `src/shared/components/Skeleton/Skeleton.tsx` (9 líneas)
- `src/shared/components/Skeleton/Skeleton.module.css` (10 líneas)
- `src/shared/components/Button/Button.tsx` (16 líneas)
- `src/shared/components/Button/Button.module.css` (45 líneas)
- `src/shared/components/BookHero/BookHero.tsx` (37 líneas)
- `src/shared/components/BookHero/BookHero.module.css` (96 líneas)
- `src/shared/components/TextField/TextField.tsx` (40 líneas) **(nuevo)**
- `src/shared/components/TextField/TextField.module.css` (44 líneas) **(nuevo)**
- `src/shared/components/AuthForm/AuthForm.tsx` (57 líneas) **(nuevo)**
- `src/shared/components/AuthForm/AuthForm.module.css` (66 líneas) **(nuevo)**

**8. Componentes específicos de features**
- `src/features/home/components/HeroCarousel/HeroCarousel.tsx` (135 líneas)
- `src/features/home/components/HeroCarousel/HeroCarousel.module.css` (238 líneas)
- `src/features/home/components/CategoryShortcuts/CategoryShortcuts.tsx` (74 líneas)
- `src/features/home/components/CategoryShortcuts/CategoryShortcuts.module.css` (70 líneas)
- `src/features/home/components/BookList/BookList.tsx` (36 líneas)
- `src/features/home/components/BookList/BookList.module.css` (38 líneas)
- `src/features/categories/components/CategoryFilter/CategoryFilter.tsx` (107 líneas)
- `src/features/categories/components/CategoryFilter/CategoryFilter.module.css` (123 líneas)
- `src/features/book-detail/components/BookTabs/BookTabs.tsx` (32 líneas)
- `src/features/book-detail/components/BookTabs/BookTabs.module.css` (28 líneas)
- `src/features/book-detail/components/BookInfoList/BookInfoList.tsx` (48 líneas)
- `src/features/book-detail/components/BookInfoList/BookInfoList.module.css` (25 líneas)
- `src/features/book-detail/components/LoanButton/LoanButton.tsx` (44 líneas)
- `src/features/book-detail/components/ExpandableText/ExpandableText.tsx` (51 líneas)
- `src/features/book-detail/components/ExpandableText/ExpandableText.module.css` (33 líneas)
- `src/features/book-reviews/components/RatingSummary/RatingSummary.tsx` (20 líneas)
- `src/features/book-reviews/components/RatingSummary/RatingSummary.module.css` (35 líneas)
- `src/features/book-reviews/components/ReviewCard/ReviewCard.tsx` (32 líneas)
- `src/features/book-reviews/components/ReviewCard/ReviewCard.module.css` (55 líneas)
- `src/features/book-reviews/components/ReviewList/ReviewList.tsx` (20 líneas)
- `src/features/book-reviews/components/ReviewList/ReviewList.module.css` (8 líneas)
- `src/features/book-reviews/components/ReviewForm/ReviewForm.tsx` (89 líneas)
- `src/features/book-reviews/components/ReviewForm/ReviewForm.module.css` (80 líneas)

**9. Hooks**
- `src/shared/hooks/useFetch.ts` (73 líneas)
- `src/shared/hooks/useCategories.ts` (11 líneas)
- `src/shared/hooks/useDebouncedValue.ts` (12 líneas)
- `src/shared/hooks/useBookDetail.ts` (11 líneas)
- `src/shared/hooks/useBookLoanStatus.ts` (16 líneas) **(modificado)**
- `src/shared/hooks/useGoBack.ts` (12 líneas)
- `src/shared/hooks/useSession.ts` (8 líneas) **(nuevo)**
- `src/features/home/hooks.ts` (12 líneas)
- `src/features/categories/hooks.ts` (13 líneas)
- `src/features/book-reviews/hooks.ts` (15 líneas)
- `src/features/profile/hooks.ts` (22 líneas) **(nuevo)**

**10. Contextos / Providers**
- _(ninguno — no existen en el proyecto)_

**11. Servicios / API**
- `src/api/http.ts` (150 líneas) **(modificado)**
- `src/api/endpoints/books.ts` (58 líneas)
- `src/api/endpoints/categories.ts` (8 líneas)
- `src/api/endpoints/ratings.ts` (14 líneas)
- `src/api/endpoints/auth.ts` (19 líneas) **(nuevo)**
- `src/api/token.ts` (96 líneas) **(modificado)**

**12. Tipos / Interfaces**
- `src/api/types/auth.ts` (28 líneas) **(nuevo)**
- `src/api/types/book.ts` (32 líneas)
- `src/api/types/category.ts` (4 líneas)
- `src/api/types/common.ts` (15 líneas)
- `src/api/types/loan.ts` (9 líneas)
- `src/api/types/rating.ts` (34 líneas)

**13. Utilidades y constantes**
- `src/shared/utils/book.ts` (9 líneas)
- `src/shared/utils/cache.ts` (83 líneas)
- `src/shared/utils/validation.ts` (5 líneas) **(nuevo)**
- `src/shared/utils/apiError.ts` (9 líneas) **(nuevo)**
- `src/shared/utils/redirect.ts` (8 líneas) **(nuevo)**
- `src/features/home/constants.ts` (66 líneas)
- `src/features/book-detail/constants.ts` (21 líneas)
- `src/features/book-reviews/constants.ts` (3 líneas)
- `src/features/book-reviews/utils.ts` (22 líneas)
- `src/features/login/utils.ts` (19 líneas) **(nuevo)**
- `src/features/register/utils.ts` (32 líneas) **(nuevo)**
- `src/features/profile/constants.ts` (2 líneas) **(nuevo)**

---

<!-- ===== 1. Configuración y entry points ===== -->

# SECCIÓN 1. Configuración y entry points

# `index.html`

## Código completo

```html
<!doctype html>
<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <link rel="icon" type="image/svg+xml" href="/favicon.svg" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap" rel="stylesheet">

    <title>lectiofrontend</title>
  </head>
  <body>
    <div id="root"></div>
    <script type="module" src="/src/main.tsx"></script>
  </body>
</html>
```

# `package.json`

## Código completo

```json
{
  "name": "lectiofrontend",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc -b && vite build",
    "lint": "eslint .",
    "preview": "vite preview"
  },
  "dependencies": {
    "react": "^19.2.8",
    "react-dom": "^19.2.8",
    "react-icons": "^5.7.0",
    "react-router": "^8.4.0"
  },
  "devDependencies": {
    "@eslint/js": "^10.0.1",
    "@types/node": "^24.13.3",
    "@types/react": "^19.2.18",
    "@types/react-dom": "^19.2.7",
    "@vitejs/plugin-react": "^6.1.1",
    "eslint": "^10.10.0",
    "eslint-plugin-react-hooks": "^7.1.1",
    "eslint-plugin-react-refresh": "^0.5.6",
    "globals": "^17.12.0",
    "typescript": "~6.0.2",
    "typescript-eslint": "^8.69.0",
    "vite": "^8.3.0"
  }
}
```

# `tsconfig.json`

## Código completo

```json
{
  "files": [],
  "references": [
    { "path": "./tsconfig.app.json" },
    { "path": "./tsconfig.node.json" }
  ]
}
```

# `tsconfig.app.json`

## Código completo

```json
{
  "compilerOptions": {
    "tsBuildInfoFile": "./node_modules/.tmp/tsconfig.app.tsbuildinfo",
    "target": "es2023",
    "lib": ["ES2023", "DOM"],
    "module": "esnext",
    "types": ["vite/client"],
    "allowArbitraryExtensions": true,
    "skipLibCheck": true,

    /* Bundler mode */
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "verbatimModuleSyntax": true,
    "moduleDetection": "force",
    "noEmit": true,
    "jsx": "react-jsx",

    /* Linting */
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "erasableSyntaxOnly": true,
    "noFallthroughCasesInSwitch": true
  },
  "include": ["src"]
}
```

# `tsconfig.node.json`

## Código completo

```json
{
  "compilerOptions": {
    "tsBuildInfoFile": "./node_modules/.tmp/tsconfig.node.tsbuildinfo",
    "target": "es2023",
    "lib": ["ES2023"],
    "types": ["node"],
    "skipLibCheck": true,

    /* Bundler mode */
    "module": "nodenext",
    "allowImportingTsExtensions": true,
    "verbatimModuleSyntax": true,
    "moduleDetection": "force",
    "noEmit": true,

    /* Linting */
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "erasableSyntaxOnly": true,
    "noFallthroughCasesInSwitch": true
  },
  "include": ["vite.config.ts"]
}
```

# `vite.config.ts`

## Código completo

```ts
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
})
```

# `eslint.config.js`

## Código completo

```js
import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'
import { defineConfig, globalIgnores } from 'eslint/config'

export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooks.configs.flat.recommended,
      reactRefresh.configs.vite,
    ],
    languageOptions: {
      globals: globals.browser,
    },
  },
])
```

# `.gitignore`

## Código completo

```text
# Logs
logs
*.log
npm-debug.log*
yarn-debug.log*
yarn-error.log*
pnpm-debug.log*
lerna-debug.log*

node_modules
dist
dist-ssr
*.local

# Editor directories and files
.vscode/*
!.vscode/extensions.json
.idea
.DS_Store
*.suo
*.ntvs*
*.njsproj
*.sln
*.sw?
```

# `src/config/env.ts`

## Código completo

```ts
export const env = {
  apiUrl: (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:5031',
}
```

# `src/main.tsx`

## Código completo

```tsx
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider } from 'react-router/dom'
import { router } from './routes'
import './index.css'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
)
```

<!-- ===== 2. App ===== -->

# SECCIÓN 2. App

_`src/App.tsx` y `src/App.css` fueron eliminados del repo en esta iteración (eran código muerto). El punto de entrada real es `src/main.tsx` + `src/routes/index.tsx`._

<!-- ===== 3. Routing ===== -->

# SECCIÓN 3. Routing

# `src/routes/index.tsx`

## Código completo

```tsx
import { createBrowserRouter } from 'react-router'
import { BookDetailPage } from '../features/book-detail/pages/BookDetailPage'
import { BookReviewsPage } from '../features/book-reviews/pages/BookReviewsPage'
import { CategoriesPage } from '../features/categories/pages/CategoriesPage'
import { HistoryPage } from '../features/history/pages/HistoryPage'
import { HomePage } from '../features/home/pages/HomePage'
import { LoginPage } from '../features/login/pages/LoginPage'
import { ProfilePage } from '../features/profile/pages/ProfilePage'
import { RegisterPage } from '../features/register/pages/RegisterPage'
import { AppLayout } from './AppLayout'

export const router = createBrowserRouter([
  {
    element: <AppLayout />,
    children: [
      { path: '/', element: <HomePage /> },
      { path: '/history', element: <HistoryPage /> },
      { path: '/categories', element: <CategoriesPage /> },
      { path: '/books/:bookId', element: <BookDetailPage /> },
      { path: '/books/:bookId/reviews', element: <BookReviewsPage /> },
      { path: '/login', element: <LoginPage /> },
      { path: '/register', element: <RegisterPage /> },
      { path: '/profile', element: <ProfilePage /> },
    ],
  },
])
```

<!-- ===== 4. Estilos globales ===== -->

# SECCIÓN 4. Estilos globales

# `src/index.css`

## Código completo

```css
:root {
  /* === PRIMARIO === */
  --color-primary: #236A34;        /* verde de marca: botones principales, tabs activos, textos destacados */
  --color-primary-hover: #1B4F27;  /* estado :hover de elementos primarios */
  --color-primary-light: #E3F0E6;  /* fondo suave verde (ej: chips o secciones resaltadas) */

  /* === SECUNDARIO === */
  --color-secondary: #F3F02B;        /* amarillo: estrellas de rating */
  --color-secondary-hover: #D6D323;  /* variante hover del amarillo, si aplica */

  /* === NEUTRALES === */
  --color-bg: #FFFFFF;         /* fondo general de la página/body */
  --color-surface: #F9FAFE;    /* fondo de cards y elementos elevados */
  --color-border: #747474;     /* bordes de inputs, líneas divisorias */
  --color-text: #000000;       /* texto principal sobre fondo claro */
  --color-text-muted: #6B7280; /* texto secundario sobre fondo claro (fechas, subtítulos) */
  --color-disabled: #E0E0E0;   /* fondo de elementos inactivos (ej: tab "Returned") */

  /* === ESTADOS === */
  --color-success: #236A34;  /* estado positivo (ej: badge "Active") */
  --color-error: #FF1226;    /* estado negativo (ej: badge "Expired") */
  --color-warning: #F59E0B;  /* advertencias futuras, aún sin uso */
  --color-info: #3B82F6;     /* mensajes informativos futuros, aún sin uso */

  /* === TIPOGRAFÍA === */
  --font-family-base: 'Poppins', sans-serif; /* tipografía global de toda la app */

  /* === PESOS DE FUENTE === */
  --font-weight-regular: 400;   /* párrafos, comentarios de reviews */
  --font-weight-medium: 500;    /* nombres de autor, labels de botones */
  --font-weight-semibold: 600;  /* subtítulos ("My Library"), nombres en cards */
  --font-weight-bold: 700;      /* títulos grandes, números destacados (ej: "4.3") */

  /* === TAMAÑOS DE FUENTE (mobile / base) === */
  --font-size-xs: 13px;   /* fechas, "rate counter: 3" */
  --font-size-sm: 15px;   /* subtítulos, categoría bajo el título, tabs inactivos */
  --font-size-md: 17px;   /* nombre de autor, texto de botones */
  --font-size-lg: 20px;   /* encabezados medianos ("My Library", ítems de menú) */
  --font-size-xl: 24px;   /* títulos de sección ("What do you plan to read today?") */
  --font-size-2xl: 28px;  /* título del libro/lugar en el detalle */
  --font-size-3xl: 36px;  /* número grande protagonista (ej: rating "4.3") */

  /* === RADIOS DE BORDE === */
  --radius-sm: 8px;     /* inputs, badges */
  --radius-md: 12px;    /* cards */
  --radius-lg: 16px;    /* modales, contenedores grandes */
  --radius-full: 999px; /* botones pill, avatares circulares */

  /* === ESPACIADO === */
  --spacing-xs: 4px;   /* separación mínima entre elementos muy cercanos (ej: ícono y label) */
  --spacing-sm: 8px;   /* padding interno chico (badges, chips) */
  --spacing-md: 16px;  /* padding estándar de cards e inputs */
  --spacing-lg: 24px;  /* separación entre secciones */
  --spacing-xl: 32px;  /* separación entre bloques grandes de la página */

  /* === SOMBRAS === */
  --shadow-sm: 0 1px 2px rgba(0,0,0,0.05);  /* elevación sutil (inputs, botones) */
  --shadow-md: 0 4px 6px rgba(0,0,0,0.1);   /* elevación media (cards) */
  --shadow-lg: 0 10px 15px rgba(0,0,0,0.1); /* elevación alta (modales, dropdowns) */

  /* === TEXTO SOBRE FONDO OSCURO / IMÁGENES === */
  --color-text-inverse: #FFFFFF;        /* texto principal blanco sobre imágenes (ej: título del hero) */
  --color-text-inverse-muted: #E5E7EB;  /* texto secundario blanco grisáceo sobre imágenes (ej: categoría) */

  /* === OVERLAY === */
  --overlay-dark: rgba(0, 0, 0, 0.55); /* capa oscura semi-transparente sobre imágenes de fondo */
}

/* === DESKTOP === */
@media (min-width: 1024px) {
  :root {
    --font-size-xs: 14px;
    --font-size-sm: 16px;
    --font-size-md: 20px;
    --font-size-lg: 24px;
    --font-size-xl: 32px;
    --font-size-2xl: 40px;
    --font-size-3xl: 48px;
  }
}

*,
*::before,
*::after {
  box-sizing: border-box;
}

body {
  margin: 0;
  font-family: var(--font-family-base);
  color: var(--color-text);
  background: var(--color-bg);
}

button {
  font: inherit;
  cursor: pointer;
}

img {
  display: block;
  max-width: 100%;
}
```

<!-- ===== 5. Layouts ===== -->

# SECCIÓN 5. Layouts

# `src/routes/AppLayout.tsx`

## Código completo

```tsx
import { Outlet, useMatch } from 'react-router'
import { Navbar } from '../shared/components/Navbar/Navbar'
import { useSession } from '../shared/hooks/useSession'

export function AppLayout() {
  const user = useSession()

  const overlayNavbar = useMatch('/books/:bookId') !== null

  return (
    <>
      <Navbar userName={user?.fullName.split(' ')[0]} overlay={overlayNavbar} />
      <Outlet />
    </>
  )
}
```

<!-- ===== 6. Páginas ===== -->

# SECCIÓN 6. Páginas

# `src/features/home/pages/HomePage.tsx`

## Código completo

```tsx
import { useState } from 'react'
import { useNavigate } from 'react-router'
import { BookGrid } from '../../../shared/components/BookGrid/BookGrid'
import { CategoryShortcuts } from '../components/CategoryShortcuts/CategoryShortcuts'
import { HeroCarousel } from '../components/HeroCarousel/HeroCarousel'
import { useCategories } from '../../../shared/hooks/useCategories'
import { useTopRatedBooks } from '../hooks'
import styles from './HomePage.module.css'

export function HomePage() {
  const navigate = useNavigate()
  const [selectedCategoryId, setSelectedCategoryId] = useState<number>()

  const categories = useCategories()
  const mostVisited = useTopRatedBooks() // sin categoryId
  const byCategory = useTopRatedBooks(selectedCategoryId, selectedCategoryId !== undefined)

  // Sin categoría seleccionada reutilizamos la data del hero (evita un segundo request)
  const list = selectedCategoryId === undefined ? mostVisited : byCategory

  const handleOpen = (bookId: number) => navigate(`/books/${bookId}`)
  const handleSeeMore = () => navigate('/categories')

  return (
    <main className={styles.main}>
      <section>
        <h2 className={styles.kicker}>
          Most <span className={styles.accent}>Visited</span> in Our{' '}
          <span className={styles.accent}>App</span>
        </h2>
        <HeroCarousel
          books={mostVisited.data ?? []}
          loading={mostVisited.loading}
          onOpen={handleOpen}
        />
      </section>

      <section>
        <h2 className={styles.title}>
          What do you <span className={styles.accent}>plan</span>
          <br className={styles.lineBreak} /> to <span className={styles.accent}>read</span>{' '}
          today?
        </h2>
        <CategoryShortcuts
          categories={categories.data ?? []}
          selectedId={selectedCategoryId}
          onSelect={setSelectedCategoryId}
          onSeeMore={handleSeeMore}
        />
      </section>

      <BookGrid
        books={list.data}
        loading={list.loading}
        error={list.error}
        emptyMessage="No hay libros en esta categoría todavía."
        onOpen={handleOpen}
      />
    </main>
  )
}
```

# `src/features/home/pages/HomePage.module.css`

## Código completo

```css
.main {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  max-width: 960px;
  margin: 0 auto;
  padding: 0 var(--spacing-md) var(--spacing-xl);
}

.kicker {
  margin: 0 0 var(--spacing-sm);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.title {
  margin: 0 0 var(--spacing-md);
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-bold);
  line-height: 1.3;
}

.accent {
  color: var(--color-primary);
}

.lineBreak {
  display: none;
}

@media (min-width: 1024px) {
  .main {
    gap: var(--spacing-xl);
    max-width: none;
    padding: 0 clamp(var(--spacing-xl), 4vw, 80px) var(--spacing-xl);
  }

  .kicker {
    margin-bottom: var(--spacing-md);
    font-size: var(--font-size-xl);
    font-weight: var(--font-weight-bold);
  }

  .lineBreak {
    display: inline;
  }
}
```

# `src/features/categories/pages/CategoriesPage.tsx`

## Código completo

```tsx
import { useEffect, useState } from 'react'
import { MdSearch } from 'react-icons/md'
import { useNavigate, useSearchParams } from 'react-router'
import { BookGrid } from '../../../shared/components/BookGrid/BookGrid'
import { Pagination } from '../../../shared/components/Pagination/Pagination'
import { useCategories } from '../../../shared/hooks/useCategories'
import { useDebouncedValue } from '../../../shared/hooks/useDebouncedValue'
import { CategoryFilter } from '../components/CategoryFilter/CategoryFilter'
import { useBookSearch } from '../hooks'
import styles from './CategoriesPage.module.css'

const PAGE_SIZE = 10

interface ParamChanges {
  q?: string
  categories?: number[]
  page?: number
}

function parseIds(value: string | null): number[] {
  if (!value) return []
  const ids = value
    .split(',')
    .map(Number)
    .filter((n) => Number.isInteger(n) && n > 0)
  return [...new Set(ids)].sort((a, b) => a - b)
}

function setOrDelete(params: URLSearchParams, key: string, value: string) {
  if (value) {
    params.set(key, value)
  } else {
    params.delete(key)
  }
}

export function CategoriesPage() {
  const navigate = useNavigate()
  const [params, setParams] = useSearchParams()

  // El estado de la búsqueda vive en la URL: ?q=vikingo&categories=4,6&page=2
  const search = params.get('q') ?? ''
  const page = Math.max(1, parseInt(params.get('page') ?? '', 10) || 1)
  const categoryIds = parseIds(params.get('categories'))

  const [text, setText] = useState(search)
  const debouncedText = useDebouncedValue(text.trim(), 400)

  const categories = useCategories()
  const results = useBookSearch({ search, categoryIds, page, pageSize: PAGE_SIZE })

  const books = results.data?.data
  const pagination = results.data?.pagination

  // `replace` evita llenar el historial con cada tecla o filtro
  const updateParams = (changes: ParamChanges, replace = true) => {
    setParams(
      (prev) => {
        const next = new URLSearchParams(prev)
        if (changes.q !== undefined) setOrDelete(next, 'q', changes.q)
        if (changes.categories !== undefined) {
          setOrDelete(next, 'categories', changes.categories.join(','))
        }
        if (changes.page !== undefined) {
          setOrDelete(next, 'page', changes.page > 1 ? String(changes.page) : '')
        }
        return next
      },
      { replace },
    )
  }

  useEffect(() => {
    if (debouncedText !== search) updateParams({ q: debouncedText, page: 1 })
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [debouncedText])

  const handleCategories = (ids: number[]) => updateParams({ categories: ids, page: 1 })

  const handlePage = (next: number) => {
    updateParams({ page: next }, false)
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  return (
    <main className={styles.main}>
      <div className={styles.filters}>
        <div>
          <label className={styles.label} htmlFor="book-search">
            Search by a specific book
          </label>
          <div className={styles.searchBox}>
            <input
              id="book-search"
              type="text"
              enterKeyHint="search"
              autoComplete="off"
              className={styles.input}
              placeholder="Type a book title…"
              value={text}
              onChange={(event) => setText(event.target.value)}
            />
            <MdSearch className={styles.searchIcon} aria-hidden="true" />
          </div>
        </div>

        <CategoryFilter
          categories={categories.data ?? []}
          selectedIds={categoryIds}
          onChange={handleCategories}
          loading={categories.loading}
        />
      </div>

      {pagination && (
        <p className={styles.count}>
          {pagination.totalCount} {pagination.totalCount === 1 ? 'book' : 'books'} found
        </p>
      )}

      <BookGrid
        books={books}
        loading={results.loading}
        error={results.error}
        skeletonCount={PAGE_SIZE}
        emptyMessage="No books found. Try another search or remove some filters."
        onOpen={(bookId) => navigate(`/books/${bookId}`)}
      />

      {pagination && (
        <Pagination page={page} totalPages={pagination.totalPages} onChange={handlePage} />
      )}
    </main>
  )
}
```

# `src/features/categories/pages/CategoriesPage.module.css`

## Código completo

```css
.main {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  max-width: 960px;
  margin: 0 auto;
  padding: var(--spacing-md) var(--spacing-md) var(--spacing-xl);
}

.filters {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.label {
  display: block;
  margin-bottom: var(--spacing-xs);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
}

.searchBox {
  position: relative;
}

.input {
  width: 100%;
  padding: var(--spacing-sm) calc(var(--spacing-md) + 32px) var(--spacing-sm) var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  background: var(--color-bg);
  color: var(--color-text);
  font: inherit;
  font-size: var(--font-size-md);
}

.input:focus-visible {
  outline: 2px solid var(--color-primary);
  outline-offset: 1px;
}

.searchIcon {
  position: absolute;
  top: 50%;
  right: var(--spacing-md);
  color: var(--color-text-muted);
  font-size: 22px;
  pointer-events: none;
  transform: translateY(-50%);
}

.count {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-xs);
}

@media (min-width: 1024px) {
  .main {
    gap: var(--spacing-lg);
    max-width: none;
    padding: var(--spacing-md) clamp(var(--spacing-xl), 4vw, 80px) var(--spacing-xl);
  }
}
```

# `src/features/book-detail/pages/BookDetailPage.tsx`

## Código completo

```tsx
import { useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router'
import type { LoanStatus } from '../../../api/types/loan'
import { BookHero } from '../../../shared/components/BookHero/BookHero'
import { Skeleton } from '../../../shared/components/Skeleton/Skeleton'
import { useBookDetail } from '../../../shared/hooks/useBookDetail'
import { useBookLoanStatus } from '../../../shared/hooks/useBookLoanStatus'
import { useGoBack } from '../../../shared/hooks/useGoBack'
import { BookInfoList } from '../components/BookInfoList/BookInfoList'
import { BookTabs } from '../components/BookTabs/BookTabs'
import { ExpandableText } from '../components/ExpandableText/ExpandableText'
import { LoanButton } from '../components/LoanButton/LoanButton'
import { DEFAULT_DESCRIPTION, getPanelId, getTabId, type BookTabId } from '../constants'
import styles from './BookDetailPage.module.css'

export function BookDetailPage() {
  const navigate = useNavigate()
  const { bookId } = useParams()
  const id = Number(bookId)
  const validId = Number.isInteger(id) && id > 0

  const [tab, setTab] = useState<BookTabId>('summary')

  const book = useBookDetail(id, validId)
  const loan = useBookLoanStatus(id, validId)
  const handleBack = useGoBack('/')

  // Reviews es una pantalla aparte (/books/:bookId/reviews), no un panel de esta página
  const handleTabChange = (next: BookTabId) => {
    if (next === 'reviews') navigate(`/books/${id}/reviews`)
    else setTab(next)
  }

  // Al iniciar sesión vuelve a este libro
  const handleLoanAction = (status: LoanStatus) => {
    if (status === 'LoginRequired') navigate('/login', { state: { from: `/books/${id}` } })
  }

  if (book.loading) {
    return (
      <main className={styles.main} aria-busy="true">
        <Skeleton className={styles.heroSkeleton} />
        <div className={styles.content}>
          <Skeleton className={styles.tabsSkeleton} />
          <Skeleton className={styles.textSkeleton} />
          <Skeleton className={styles.textSkeleton} />
          <Skeleton className={styles.textSkeleton} />
        </div>
      </main>
    )
  }

  if (!book.data) {
    return (
      <main className={styles.state}>
        <p role="alert" className={styles.message}>
          We couldn't load this book. It may not exist or the server is unavailable.
        </p>
        <Link to="/" className={styles.link}>
          Back to home
        </Link>
      </main>
    )
  }

  const detail = book.data
  const summary = detail.description?.trim() || DEFAULT_DESCRIPTION

  return (
    <main className={styles.main}>
      <BookHero
        title={detail.title}
        category={detail.categories[0]?.name}
        imageUrl={detail.imageUrl}
        onBack={handleBack}
      />

      <div className={styles.content}>
        <BookTabs active={tab} onChange={handleTabChange} />

        <div role="tabpanel" id={getPanelId(tab)} aria-labelledby={getTabId(tab)}>
          {tab === 'summary' && <ExpandableText text={`"${summary}"`} />}
          {tab === 'basic-info' && <BookInfoList book={detail} />}
        </div>

        <div className={styles.actions}>
          <LoanButton
            status={loan.data?.status}
            loading={loan.loading}
            hasError={Boolean(loan.error)}
            onAction={handleLoanAction}
          />
        </div>
      </div>
    </main>
  )
}
```

# `src/features/book-detail/pages/BookDetailPage.module.css`

## Código completo

```css
.main {
  display: flex;
  flex-direction: column;
}

.content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  width: 100%;
  max-width: 960px;
  margin: 0 auto;
  padding: var(--spacing-md) var(--spacing-md) var(--spacing-xl);
}

.message {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
}

.actions {
  display: flex;
  justify-content: center;
}

.state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-md);
  padding: calc(var(--spacing-xl) * 3) var(--spacing-md) var(--spacing-xl);
  text-align: center;
}

.link {
  color: var(--color-primary);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.heroSkeleton {
  height: 300px;
}

.tabsSkeleton {
  width: 60%;
  height: 24px;
  border-radius: var(--radius-sm);
}

.textSkeleton {
  height: 16px;
  border-radius: var(--radius-sm);
}

@media (min-width: 768px) {
  .heroSkeleton {
    height: 360px;
  }
}

@media (min-width: 1024px) {
  .main {
    display: grid;
    grid-template-columns: minmax(0, 4fr) minmax(0, 7fr);
    gap: var(--spacing-xl);
    align-items: start;
    padding: var(--spacing-md) clamp(var(--spacing-xl), 4vw, 80px) var(--spacing-xl);
  }

  .content {
    max-width: none;
    margin: 0;
    padding: 0;
  }

  .actions {
    justify-content: flex-start;
  }

  .state {
    padding-top: var(--spacing-xl);
  }

  .heroSkeleton {
    height: auto;
    aspect-ratio: 3 / 4;
    max-height: calc(100vh - 160px);
    border-radius: var(--radius-md);
  }
}
```

# `src/features/book-reviews/pages/BookReviewsPage.tsx`

## Código completo

```tsx
import { useState, type ReactNode } from 'react'
import { MdChevronLeft } from 'react-icons/md'
import { Link, useNavigate, useParams } from 'react-router'
import { createRating } from '../../../api/endpoints/ratings'
import { BookHero } from '../../../shared/components/BookHero/BookHero'
import { Button } from '../../../shared/components/Button/Button'
import { Pagination } from '../../../shared/components/Pagination/Pagination'
import { Skeleton } from '../../../shared/components/Skeleton/Skeleton'
import { useBookDetail } from '../../../shared/hooks/useBookDetail'
import { useBookLoanStatus } from '../../../shared/hooks/useBookLoanStatus'
import { useGoBack } from '../../../shared/hooks/useGoBack'
import { RatingSummary } from '../components/RatingSummary/RatingSummary'
import { ReviewForm, type ReviewFormValues } from '../components/ReviewForm/ReviewForm'
import { ReviewList } from '../components/ReviewList/ReviewList'
import { RATINGS_PAGE_SIZE } from '../constants'
import { useBookRatings } from '../hooks'
import { getSubmitErrorMessage } from '../utils'
import styles from './BookReviewsPage.module.css'

export function BookReviewsPage() {
  const navigate = useNavigate()
  const { bookId } = useParams()
  const id = Number(bookId)
  const validId = Number.isInteger(id) && id > 0

  const [page, setPage] = useState(1)
  const [version, setVersion] = useState(0)
  const [formOpen, setFormOpen] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [notice, setNotice] = useState<string | null>(null)

  const goBack = useGoBack(`/books/${id}`)
  const book = useBookDetail(id, validId)
  const loan = useBookLoanStatus(id, validId)
  const ratings = useBookRatings(id, page, version, validId)

  // Sin JWT el backend responde `userId: null` (status LoginRequired)
  const userId = loan.data?.userId ?? null
  const isLoggedIn = userId !== null

  const handlePage = (next: number) => {
    setPage(next)
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const handleAddReview = () => {
    if (!isLoggedIn) {
      navigate('/login', { state: { from: `/books/${id}/reviews` } }) // Al iniciar sesión vuelve a esta pantalla
      return
    }
    setNotice(null)
    setSubmitError(null)
    setFormOpen(true)
  }

  const handleSubmit = async ({ stars, comment }: ReviewFormValues) => {
    if (userId === null) return

    setSubmitting(true)
    setSubmitError(null)
    try {
      const res = await createRating(userId, { bookId: id, stars, comment: comment || null })
      setFormOpen(false)
      setNotice(res.message)
      setPage(1)
      setVersion((current) => current + 1) // fuerza recargar la lista y el promedio
    } catch (error) {
      setSubmitError(getSubmitErrorMessage(error))
    } finally {
      setSubmitting(false)
    }
  }

  if (book.loading) {
    return (
      <main className={styles.main} aria-busy="true">
        <div className={styles.hero}>
          <Skeleton className={styles.heroSkeleton} />
        </div>
        <div className={styles.content}>
          <Skeleton className={styles.titleSkeleton} />
          <Skeleton className={styles.summarySkeleton} />
          <Skeleton className={styles.cardSkeleton} />
          <Skeleton className={styles.cardSkeleton} />
          <Skeleton className={styles.cardSkeleton} />
        </div>
      </main>
    )
  }

  if (!book.data) {
    return (
      <main className={styles.state}>
        <p role="alert" className={styles.message}>
          We couldn't load this book. It may not exist or the server is unavailable.
        </p>
        <Link to="/" className={styles.link}>
          Back to home
        </Link>
      </main>
    )
  }

  const detail = book.data
  const totalPages = ratings.data ? Math.ceil(ratings.data.numberOfRatings / RATINGS_PAGE_SIZE) : 0

  let summarySection: ReactNode = null
  if (ratings.data) {
    summarySection = (
      <RatingSummary average={ratings.data.averageRating} count={ratings.data.numberOfRatings} />
    )
  } else if (ratings.loading) {
    summarySection = <Skeleton className={styles.summarySkeleton} />
  }

  let listSection: ReactNode = null
  if (ratings.data) {
    listSection =
      ratings.data.users.length > 0 ? (
        <ReviewList reviews={ratings.data.users} />
      ) : (
        <p className={styles.message}>No reviews yet. Be the first to share your thoughts.</p>
      )
  } else if (ratings.loading) {
    listSection = (
      <div className={styles.skeletonList}>
        <Skeleton className={styles.cardSkeleton} />
        <Skeleton className={styles.cardSkeleton} />
        <Skeleton className={styles.cardSkeleton} />
      </div>
    )
  }

  return (
    <main className={styles.main}>
      {/* Solo desktop: el libro a la izquierda */}
      <div className={styles.hero}>
        <BookHero
          title={detail.title}
          category={detail.categories[0]?.name}
          imageUrl={detail.imageUrl}
          onBack={goBack}
          headingLevel={2}
        />
      </div>

      <div className={styles.content}>
        <header className={styles.header}>
          <button type="button" className={styles.back} aria-label="Volver" onClick={goBack}>
            <MdChevronLeft aria-hidden="true" />
          </button>
          <h1 className={styles.title}>{detail.title} - Reviews</h1>
        </header>

        {summarySection}

        <div className={styles.actions}>
          <Button block disabled={loan.loading || formOpen} onClick={handleAddReview}>
            Add Review
          </Button>
        </div>

        {notice && (
          <p role="status" className={styles.notice}>
            {notice}
          </p>
        )}

        {formOpen && (
          <ReviewForm
            submitting={submitting}
            error={submitError}
            onSubmit={handleSubmit}
            onCancel={() => setFormOpen(false)}
          />
        )}

        {ratings.error && (
          <p role="alert" className={styles.error}>
            We couldn't load the reviews. Please try again.
          </p>
        )}

        <section
          aria-label="Reviews"
          aria-busy={ratings.loading}
          className={ratings.loading && ratings.data ? styles.stale : undefined}
        >
          {listSection}
        </section>

        <Pagination page={page} totalPages={totalPages} onChange={handlePage} />
      </div>
    </main>
  )
}
```

# `src/features/book-reviews/pages/BookReviewsPage.module.css`

## Código completo

```css
.main {
  display: flex;
  flex-direction: column;
}

/* El libro solo se muestra en desktop */
.hero {
  display: none;
}

.content {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  width: 100%;
  max-width: 960px;
  margin: 0 auto;
  padding: var(--spacing-md) var(--spacing-md) var(--spacing-xl);
}

.header {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 32px;
}

.back {
  position: absolute;
  left: 0;
  display: grid;
  place-items: center;
  width: 32px;
  height: 32px;
  padding: 0;
  border: 0;
  border-radius: var(--radius-full);
  background: none;
  color: var(--color-primary);
  font-size: 28px;
}

.back:focus-visible {
  outline: 2px solid var(--color-primary);
  outline-offset: 2px;
}

.title {
  margin: 0;
  padding: 0 var(--spacing-xl);
  color: var(--color-primary);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  text-align: center;
}

.actions {
  display: flex;
  justify-content: center;
}

.notice,
.message,
.error {
  margin: 0;
  font-size: var(--font-size-sm);
  text-align: center;
}

.notice {
  color: var(--color-success);
}

.message {
  color: var(--color-text-muted);
}

.error {
  color: var(--color-error);
}

/* Mientras llega la página siguiente, la anterior queda atenuada */
.stale {
  opacity: 0.5;
}

/* Error / no encontrado */
.state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-xl) var(--spacing-md);
  text-align: center;
}

.link {
  color: var(--color-primary);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

/* Loading */
.skeletonList {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.heroSkeleton {
  height: 300px;
}

.titleSkeleton {
  width: 70%;
  height: 24px;
  margin: 0 auto;
  border-radius: var(--radius-sm);
}

.summarySkeleton {
  width: min(100%, 220px);
  height: 160px;
  margin: 0 auto;
  border-radius: var(--radius-md);
}

.cardSkeleton {
  height: 112px;
  border-radius: var(--radius-md);
}

@media (min-width: 1024px) {
  .main {
    display: grid;
    grid-template-columns: minmax(0, 4fr) minmax(0, 7fr);
    gap: var(--spacing-xl);
    align-items: start;
    padding: var(--spacing-md) clamp(var(--spacing-xl), 4vw, 80px) var(--spacing-xl);
  }

  .hero {
    display: block;
  }

  .content {
    max-width: none;
    margin: 0;
    padding: 0;
  }

  /* El botón volver ya está sobre la portada */
  .back {
    display: none;
  }

  .header {
    justify-content: flex-start;
  }

  .title {
    padding: 0;
    font-size: var(--font-size-lg);
    text-align: left;
  }

  .heroSkeleton {
    height: auto;
    aspect-ratio: 3 / 4;
    max-height: calc(100vh - 160px);
    border-radius: var(--radius-md);
  }
}
```

# `src/features/login/pages/LoginPage.tsx`

## Código completo

```tsx
import { useState, type ChangeEvent, type FormEvent } from 'react'
import { Link, Navigate, useLocation } from 'react-router'
import { login } from '../../../api/endpoints/auth'
import { saveSession } from '../../../api/token'
import { AuthForm } from '../../../shared/components/AuthForm/AuthForm'
import { TextField } from '../../../shared/components/TextField/TextField'
import { useSession } from '../../../shared/hooks/useSession'
import { getApiErrorMessage } from '../../../shared/utils/apiError'
import { getRedirectTarget } from '../../../shared/utils/redirect'
import { validateLogin, type LoginErrors, type LoginValues } from '../utils'

export function LoginPage() {
  const user = useSession()
  const location = useLocation()

  const [values, setValues] = useState<LoginValues>({ email: '', password: '' })
  const [errors, setErrors] = useState<LoginErrors>({})
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  // Con sesión (también justo después de iniciarla) se vuelve a donde venía el usuario
  if (user) return <Navigate to={getRedirectTarget(location.state)} replace />

  const handleChange = (field: keyof LoginValues) => (event: ChangeEvent<HTMLInputElement>) => {
    setValues((current) => ({ ...current, [field]: event.target.value }))
    setErrors((current) => ({ ...current, [field]: undefined }))
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (submitting) return

    const nextErrors = validateLogin(values)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setSubmitting(true)
    setSubmitError(null)
    try {
      const auth = await login({ email: values.email.trim(), password: values.password })
      saveSession(auth) // useSession cambia y el <Navigate> de arriba redirige
    } catch (error) {
      setSubmitError(getApiErrorMessage(error, 'Something went wrong. Please try again.'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <AuthForm
      title="Log in"
      subtitle="Welcome back! Sign in to borrow books and leave reviews."
      submitLabel="Log in"
      submittingLabel="Logging in…"
      submitting={submitting}
      error={submitError}
      onSubmit={handleSubmit}
      footer={
        <>
          Don't have an account?{' '}
          <Link to="/register" state={location.state}>
            Sign up
          </Link>
        </>
      }
    >
      <TextField
        label="Email"
        type="email"
        autoComplete="email"
        value={values.email}
        error={errors.email}
        onChange={handleChange('email')}
      />
      <TextField
        label="Password"
        type="password"
        autoComplete="current-password"
        value={values.password}
        error={errors.password}
        onChange={handleChange('password')}
      />
    </AuthForm>
  )
}
```

# `src/features/register/pages/RegisterPage.tsx`

## Código completo

```tsx
import { useState, type ChangeEvent, type FormEvent } from 'react'
import { Link, Navigate, useLocation } from 'react-router'
import { register } from '../../../api/endpoints/auth'
import { saveSession } from '../../../api/token'
import { AuthForm } from '../../../shared/components/AuthForm/AuthForm'
import { TextField } from '../../../shared/components/TextField/TextField'
import { useSession } from '../../../shared/hooks/useSession'
import { getApiErrorMessage } from '../../../shared/utils/apiError'
import { getRedirectTarget } from '../../../shared/utils/redirect'
import { validateRegister, type RegisterErrors, type RegisterValues } from '../utils'

export function RegisterPage() {
  const user = useSession()
  const location = useLocation()

  const [values, setValues] = useState<RegisterValues>({
    fullName: '',
    email: '',
    password: '',
    phone: '',
  })
  const [errors, setErrors] = useState<RegisterErrors>({})
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)

  // El registro también inicia sesión: con sesión se vuelve a donde venía el usuario
  if (user) return <Navigate to={getRedirectTarget(location.state)} replace />

  const handleChange = (field: keyof RegisterValues) => (event: ChangeEvent<HTMLInputElement>) => {
    setValues((current) => ({ ...current, [field]: event.target.value }))
    setErrors((current) => ({ ...current, [field]: undefined }))
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (submitting) return

    const nextErrors = validateRegister(values)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setSubmitting(true)
    setSubmitError(null)
    try {
      const auth = await register({
        fullName: values.fullName.trim(),
        email: values.email.trim(),
        password: values.password,
        phone: values.phone.trim() || null,
      })
      saveSession(auth) // useSession cambia y el <Navigate> de arriba redirige
    } catch (error) {
      setSubmitError(getApiErrorMessage(error, 'Something went wrong. Please try again.'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <AuthForm
      title="Create account"
      subtitle="Join Lectio to borrow books and share your reviews."
      submitLabel="Sign up"
      submittingLabel="Creating account…"
      submitting={submitting}
      error={submitError}
      onSubmit={handleSubmit}
      footer={
        <>
          Already have an account?{' '}
          <Link to="/login" state={location.state}>
            Log in
          </Link>
        </>
      }
    >
      <TextField
        label="Full name"
        autoComplete="name"
        value={values.fullName}
        error={errors.fullName}
        onChange={handleChange('fullName')}
      />
      <TextField
        label="Email"
        type="email"
        autoComplete="email"
        value={values.email}
        error={errors.email}
        onChange={handleChange('email')}
      />
      <TextField
        label="Password"
        type="password"
        autoComplete="new-password"
        hint="At least 8 characters, with a letter and a number."
        value={values.password}
        error={errors.password}
        onChange={handleChange('password')}
      />
      <TextField
        label="Phone (optional)"
        type="tel"
        autoComplete="tel"
        maxLength={20}
        value={values.phone}
        error={errors.phone}
        onChange={handleChange('phone')}
      />
    </AuthForm>
  )
}
```

# `src/features/profile/pages/ProfilePage.tsx`

## Código completo

```tsx
import { MdChevronRight } from 'react-icons/md'
import { Navigate } from 'react-router'
import { Button } from '../../../shared/components/Button/Button'
import { useSession } from '../../../shared/hooks/useSession'
import { PROFILE_MENU } from '../constants'
import { useLogout } from '../hooks'
import styles from './ProfilePage.module.css'

export function ProfilePage() {
  const user = useSession()
  const { loggingOut, logout } = useLogout()

  // Sin sesión no hay perfil. Tras cerrar sesión a propósito no se guarda `from` (no volver al perfil)
  if (!user) {
    return <Navigate to="/login" replace state={loggingOut ? undefined : { from: '/profile' }} />
  }

  return (
    <main className={styles.main}>
      <section className={styles.profile}>
        <span className={styles.avatar} aria-hidden="true">
          {user.fullName.charAt(0).toUpperCase()}
        </span>
        <h1 className={styles.greeting}>
          Hola, <span className={styles.name}>{user.fullName}</span>
        </h1>
      </section>

      <ul className={styles.menu}>
        {PROFILE_MENU.map((label) => (
          <li key={label} className={styles.item}>
            <span>{label}</span>
            <MdChevronRight className={styles.chevron} aria-hidden="true" />
          </li>
        ))}
      </ul>

      <div className={styles.actions}>
        <Button block disabled={loggingOut} onClick={logout}>
          {loggingOut ? 'Logging out…' : 'Log Out'}
        </Button>
      </div>
    </main>
  )
}
```

# `src/features/profile/pages/ProfilePage.module.css`

## Código completo

```css
.main {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
  max-width: 960px;
  margin: 0 auto;
  padding: var(--spacing-xl) var(--spacing-md);
}

.profile {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-md);
}

.avatar {
  display: grid;
  place-items: center;
  width: 100px;
  height: 100px;
  border-radius: var(--radius-full);
  background: var(--color-primary-light);
  color: var(--color-primary);
  font-size: var(--font-size-3xl);
  font-weight: var(--font-weight-bold);
}

.greeting {
  margin: 0;
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-bold);
  text-align: center;
  overflow-wrap: anywhere;
}

.name {
  color: var(--color-primary);
}

.menu {
  margin: 0;
  padding: 0;
  list-style: none;
}

.item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-md);
  padding: var(--spacing-md) var(--spacing-xs);
  border-bottom: 1px solid var(--color-primary-light);
  font-size: var(--font-size-sm);
}

.chevron {
  flex: none;
  font-size: 24px;
}

.actions {
  display: flex;
  justify-content: center;
}

@media (min-width: 1024px) {
  .main {
    display: grid;
    grid-template-columns: minmax(0, 4fr) minmax(0, 7fr);
    grid-template-areas:
      'profile menu'
      'actions menu';
    align-items: start;
    column-gap: var(--spacing-xl);
    row-gap: var(--spacing-lg);
    padding: var(--spacing-xl) clamp(var(--spacing-xl), 4vw, 80px);
    max-width: none;
  }

  .profile {
    grid-area: profile;
    padding: var(--spacing-xl);
    border-radius: var(--radius-md);
    background: var(--color-surface);
    box-shadow: var(--shadow-md);
  }

  .menu {
    grid-area: menu;
    padding: var(--spacing-md) var(--spacing-lg);
    border-radius: var(--radius-md);
    background: var(--color-surface);
    box-shadow: var(--shadow-md);
  }

  .item:last-child {
    border-bottom: 0;
  }

  .actions {
    grid-area: actions;
  }
}
```

# `src/features/history/pages/HistoryPage.tsx`

## Código completo

```tsx
import { Link, Navigate, useNavigate, useSearchParams } from 'react-router'
import { Pagination } from '../../../shared/components/Pagination/Pagination'
import { Skeleton } from '../../../shared/components/Skeleton/Skeleton'
import { useSession } from '../../../shared/hooks/useSession'
import { LoanCard } from '../components/LoanCard/LoanCard'
import { PAGE_SIZE } from '../constants'
import { useUserLoans } from '../hooks'
import styles from './HistoryPage.module.css'

export function HistoryPage() {
  const user = useSession()
  const navigate = useNavigate()
  const [params, setParams] = useSearchParams()

  const page = Math.max(1, parseInt(params.get('page') ?? '', 10) || 1)

  // All hooks must be called before any early return
  const loans = useUserLoans(user?.id ?? 0, page, Boolean(user))

  if (!user) {
    return <Navigate to="/login" replace state={{ from: '/history' }} />
  }

  const handlePage = (next: number) => {
    setParams(
      (prev) => {
        const next2 = new URLSearchParams(prev)
        if (next > 1) next2.set('page', String(next))
        else next2.delete('page')
        return next2
      },
      { replace: false },
    )
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const handleReturn = (bookId: number) => {
    navigate(`/books/${bookId}`, { state: { openReturnDialog: true } })
  }

  const handleRate = (bookId: number) => {
    navigate(`/books/${bookId}/reviews`)
  }

  const loanList = loans.data?.data
  const pagination = loans.data?.pagination
  const isStale = loans.loading && Boolean(loanList)

  return (
    <main className={styles.main}>
      <h1 className={styles.heading}>My Library</h1>

      {loans.error && !loanList && (
        <p role="alert" className={styles.message}>
          We couldn&apos;t load your loans. Please try again.
        </p>
      )}

      {loans.loading && !loanList && (
        <div
          className={styles.grid}
          aria-busy="true"
          aria-label="Loading your loans"
        >
          {Array.from({ length: PAGE_SIZE }, (_, i) => (
            <Skeleton key={i} className={styles.cardSkeleton} />
          ))}
        </div>
      )}

      {!loans.loading && !loans.error && loanList && loanList.length === 0 && (
        <p className={styles.message}>
          You haven&apos;t borrowed any books yet.{' '}
          <Link to="/" className={styles.link}>
            Browse the catalogue
          </Link>
        </p>
      )}

      {loanList && loanList.length > 0 && (
        <div
          className={`${styles.grid} ${isStale ? styles.stale : ''}`}
          aria-busy={isStale}
        >
          {loanList.map((loan) => (
            <LoanCard
              key={loan.id}
              loan={loan}
              onReturn={handleReturn}
              onRate={handleRate}
            />
          ))}
        </div>
      )}

      {pagination && pagination.totalPages > 1 && (
        <Pagination
          page={page}
          totalPages={pagination.totalPages}
          onChange={handlePage}
        />
      )}
    </main>
  )
}
```

# `src/features/history/pages/HistoryPage.module.css`

## Código completo

```css
.main {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  max-width: 960px;
  margin: 0 auto;
  padding: var(--spacing-md) var(--spacing-md) var(--spacing-xl);
}

.heading {
  margin: 0;
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-bold);
}

.grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: var(--spacing-md);
  transition: opacity 0.15s ease;
}

.stale {
  opacity: 0.5;
  pointer-events: none;
}

.cardSkeleton {
  aspect-ratio: 2 / 3;
  border-radius: var(--radius-md);
}

.message {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  text-align: center;
}

.link {
  color: var(--color-primary);
  font-weight: var(--font-weight-medium);
  text-decoration: none;
}

.link:hover {
  text-decoration: underline;
}

@media (min-width: 768px) {
  .grid {
    grid-template-columns: repeat(4, minmax(0, 1fr));
  }
}

@media (min-width: 1024px) {
  .main {
    gap: var(--spacing-lg);
    max-width: none;
    padding: var(--spacing-md) clamp(var(--spacing-xl), 4vw, 80px) var(--spacing-xl);
  }

  .grid {
    grid-template-columns: repeat(5, minmax(0, 1fr));
    gap: var(--spacing-lg);
  }

  .heading {
    font-size: var(--font-size-xl);
  }
}
```

# `src/features/history/components/LoanCard/LoanCard.tsx`

## Código completo

```tsx
import { MdMenuBook } from 'react-icons/md'
import type { UserLoan } from '../../../../api/types/loan'
import { Button } from '../../../../shared/components/Button/Button'
import { RatingStars } from '../../../../shared/components/RatingStars/RatingStars'
import { formatAuthors, toHttps } from '../../../../shared/utils/book'
import { formatShortDate, getDaysUntilDue } from '../../../../shared/utils/date'
import styles from './LoanCard.module.css'

interface LoanCardProps {
  loan: UserLoan
  onReturn: (bookId: number) => void
  onRate: (bookId: number) => void
}

function StatusLine({ loan }: { loan: UserLoan }) {
  if (loan.status === 'Returned') return null

  const days = getDaysUntilDue(loan.dueDate)

  if (loan.status === 'Active') {
    const label =
      days === 0 ? 'Due today' : days === 1 ? 'Due in 1 day' : `Due in ${days} days`
    return <p className={styles.statusActive}>{label}</p>
  }

  // Overdue
  const overdueDays = Math.abs(days)
  const label =
    overdueDays === 0
      ? 'Overdue today'
      : overdueDays === 1
        ? 'Overdue by 1 day'
        : `Overdue by ${overdueDays} days`
  return <p className={styles.statusOverdue}>{label}</p>
}

export function LoanCard({ loan, onReturn, onRate }: LoanCardProps) {
  const { book } = loan
  const authors = formatAuthors(book.authors)
  const showAuthor = book.authors.length > 0

  return (
    <article className={styles.card}>
      <div className={styles.cover}>
        {book.imageUrl ? (
          <img src={toHttps(book.imageUrl)} alt={`Cover of ${book.title}`} loading="lazy" />
        ) : (
          <div className={styles.placeholder}>
            <MdMenuBook aria-hidden="true" />
          </div>
        )}
        <span className={styles.title}>{book.title}</span>
      </div>

      <div className={styles.rating}>
        <RatingStars value={book.averageRating} />
      </div>

      {showAuthor && <p className={styles.author}>{authors}</p>}

      <div className={styles.dates}>
        <p className={styles.date}>Borrowed: {formatShortDate(loan.checkoutDate)}</p>
        <p className={styles.date}>Due Date: {formatShortDate(loan.dueDate)}</p>
        {loan.status === 'Returned' && loan.returnDate && (
          <p className={styles.date}>Returned: {formatShortDate(loan.returnDate)}</p>
        )}
      </div>

      <StatusLine loan={loan} />

      {loan.status === 'Returned' ? (
        <Button block onClick={() => onRate(book.id)} aria-label={`Rate ${book.title}`}>
          Rate
        </Button>
      ) : (
        <Button
          block
          variant={loan.status === 'Overdue' ? 'danger' : 'primary'}
          onClick={() => onReturn(book.id)}
          aria-label={`Return ${book.title}`}
        >
          Return
        </Button>
      )}
    </article>
  )
}
```

# `src/features/history/components/LoanCard/LoanCard.module.css`

## Código completo

```css
.card {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-sm);
  min-width: 0;
  padding: var(--spacing-sm);
  border-radius: var(--radius-md);
  background: var(--color-surface);
  box-shadow: var(--shadow-md);
}

.cover {
  position: relative;
  width: 100%;
  aspect-ratio: 2 / 3;
  overflow: hidden;
  border-radius: var(--radius-sm);
  background: var(--color-disabled);
}

.cover img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.placeholder {
  display: grid;
  place-items: center;
  height: 100%;
  font-size: 40px;
  color: var(--color-text-muted);
}

.title {
  position: absolute;
  right: 0;
  bottom: 0;
  left: 0;
  display: -webkit-box;
  padding: var(--spacing-xs) var(--spacing-sm);
  overflow: hidden;
  background: var(--overlay-dark);
  color: var(--color-text-inverse);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
  text-align: center;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.rating {
  padding: 2px var(--spacing-sm);
  border-radius: var(--radius-sm);
  background: var(--color-bg);
  box-shadow: var(--shadow-sm);
}

.author {
  width: 100%;
  margin: 0;
  overflow: hidden;
  text-align: center;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.dates {
  width: 100%;
}

.date {
  margin: 0;
  font-size: var(--font-size-xs);
  color: var(--color-text-muted);
  line-height: 1.5;
  text-align: center;
}

.statusActive {
  width: 100%;
  margin: 0;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-bold);
  color: var(--color-text);
  text-align: center;
}

.statusOverdue {
  width: 100%;
  margin: 0;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-bold);
  color: var(--color-error);
  text-align: center;
}

@media (min-width: 1024px) {
  .cover {
    aspect-ratio: 3 / 4;
  }
}
```

# `src/features/history/hooks.ts`

## Código completo

```ts
import { getUserLoans } from '../../api/endpoints/loans'
import { useFetch } from '../../shared/hooks/useFetch'
import { PAGE_SIZE } from './constants'

// No cache (ttl 0) so the list is fresh after returning a book.
// keepPrevious: true keeps the previous page visible while the next one loads.
// The key includes userId, page and pageSize so the effect reruns on any change.
export function useUserLoans(userId: number, page: number, enabled: boolean) {
  return useFetch(
    `user-loans:${userId}:${page}:${PAGE_SIZE}`,
    (signal) => getUserLoans(userId, { page, pageSize: PAGE_SIZE }, signal),
    { enabled, ttl: 0, keepPrevious: true },
  )
}
```

# `src/features/history/constants.ts`

## Código completo

```ts
export const PAGE_SIZE = 10
```

<!-- ===== 7. Componentes compartidos (src/shared/components) ===== -->

# SECCIÓN 7. Componentes compartidos (src/shared/components)

# `src/shared/components/BookCard/BookCard.tsx`

## Código completo

```tsx
import type { BookSummary } from '../../../api/types/book'
import { formatAuthors, toHttps } from '../../utils/book'
import { RatingStars } from '../RatingStars/RatingStars'
import styles from './BookCard.module.css'
import { MdChevronRight, MdMenuBook } from 'react-icons/md'

interface BookCardProps {
  book: BookSummary
  onOpen?: (bookId: number) => void
}

export function BookCard({ book, onOpen }: BookCardProps) {
  return (
    <article className={styles.card}>
      <div className={styles.cover}>
        {book.imageUrl ? (
          <img src={toHttps(book.imageUrl)} alt={`Portada de ${book.title}`} loading="lazy" />
        ) : (
          <div className={styles.placeholder}><MdMenuBook /></div>
        )}
        <span className={styles.title}>{book.title}</span>
      </div>

      <div className={styles.rating}>
        <RatingStars value={book.rating?.stars ?? 0} />
      </div>

      <p className={styles.author}>{formatAuthors(book.authors)}</p>

      <button
        type="button"
        className={styles.open}
        aria-label={`Ver detalle de ${book.title}`}
        onClick={() => onOpen?.(book.id)}>
        <MdChevronRight className={styles.openArrow} aria-hidden="true" />
        <span className={styles.openLabel}>See More</span>
      </button>
    </article>
  )
}
```

# `src/shared/components/BookCard/BookCard.module.css`

## Código completo

```css
.card {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-sm);
  min-width: 0;
  padding: var(--spacing-sm);
  border-radius: var(--radius-md);
  background: var(--color-surface);
  box-shadow: var(--shadow-md);
}

.cover {
  position: relative;
  width: 100%;
  aspect-ratio: 2 / 3;
  overflow: hidden;
  border-radius: var(--radius-sm);
  background: var(--color-disabled);
}

.cover img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.placeholder {
  display: grid;
  place-items: center;
  height: 100%;
  font-size: 40px;
}

.title {
  position: absolute;
  right: 0;
  bottom: 0;
  left: 0;
  display: -webkit-box;
  padding: var(--spacing-xs) var(--spacing-sm);
  overflow: hidden;
  background: var(--overlay-dark);
  color: var(--color-text-inverse);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
  text-align: center;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.rating {
  padding: 2px var(--spacing-sm);
  border-radius: var(--radius-sm);
  background: var(--color-bg);
  box-shadow: var(--shadow-sm);
}

.author {
  width: 100%;
  margin: 0;
  overflow: hidden;
  text-align: center;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.open {
  display: grid;
  place-items: center;
  width: 28px;
  height: 22px;
  border: 0;
  border-radius: var(--radius-sm);
  background: var(--color-primary);
  color: var(--color-text-inverse);
  font-size: 18px;
  line-height: 1;
}

.open:hover {
  background: var(--color-primary-hover);
}

.openLabel {
  display: none;
}

@media (min-width: 1024px) {
  .cover {
    aspect-ratio: 3 / 4;
  }

  .openArrow {
    display: none;
  }

  .openLabel {
    display: inline;
  }

  .open {
    width: 100%;
    height: auto;
    padding: var(--spacing-sm) var(--spacing-md);
    border-radius: var(--radius-full);
    font-size: var(--font-size-xs);
    font-weight: var(--font-weight-semibold);
  }
}
```

# `src/shared/components/BookGrid/BookGrid.tsx`

## Código completo

```tsx
import type { BookSummary } from '../../../api/types/book'
import { BookCard } from '../BookCard/BookCard'
import styles from './BookGrid.module.css'

interface BookGridProps {
  books?: BookSummary[]
  loading: boolean
  error?: string
  emptyMessage?: string
  skeletonCount?: number
  onOpen?: (bookId: number) => void
}

export function BookGrid({
  books,
  loading,
  error,
  emptyMessage = 'No hay libros todavía.',
  skeletonCount = 4,
  onOpen,
}: BookGridProps) {
  const hasBooks = Boolean(books && books.length > 0)

  if (error) return <p role="alert" className={styles.message}>{error}</p>

  if (loading && !hasBooks) {
    return (
      <div className={styles.grid} aria-busy="true">
        {Array.from({ length: skeletonCount }, (_, i) => (
          <div key={i} className={styles.skeleton} />
        ))}
      </div>
    )
  }

  if (!books || books.length === 0) {
    return <p className={styles.message}>{emptyMessage}</p>
  }

  return (
    <div className={`${styles.grid} ${loading ? styles.stale : ''}`} aria-busy={loading}>
      {books.map((book) => (
        <BookCard key={book.id} book={book} onOpen={onOpen} />
      ))}
    </div>
  )
}
```

# `src/shared/components/BookGrid/BookGrid.module.css`

## Código completo

```css
.grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: var(--spacing-md);
  transition: opacity 0.15s ease;
}

/* Mientras llegan datos nuevos, la grilla anterior queda atenuada */
.stale {
  opacity: 0.5;
  pointer-events: none;
}

.skeleton {
  aspect-ratio: 3 / 5;
  border-radius: var(--radius-md);
  background: var(--color-disabled);
  animation: pulse 1.2s ease-in-out infinite;
}

.message {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  text-align: center;
}

@keyframes pulse {
  50% {
    opacity: 0.5;
  }
}

@media (min-width: 768px) {
  .grid {
    grid-template-columns: repeat(4, minmax(0, 1fr));
  }
}

@media (min-width: 1024px) {
  .grid {
    grid-template-columns: repeat(5, minmax(0, 1fr));
    gap: var(--spacing-lg);
  }
}
```

# `src/shared/components/RatingStars/RatingStars.tsx`

## Código completo

```tsx
import { MdStar } from 'react-icons/md'
import styles from './RatingStars.module.css'

interface RatingStarsProps {
  value: number
  size?: 'sm' | 'lg'
}

function FiveStars() {
  return (
    <>
      {Array.from({ length: 5 }, (_, i) => (
        <MdStar key={i} />
      ))}
    </>
  )
}

export function RatingStars({ value, size = 'sm' }: RatingStarsProps) {
  const safe = Math.max(0, Math.min(5, value))

  return (
    <span
      className={`${styles.stars} ${size === 'lg' ? styles.lg : ''}`}
      role="img"
      aria-label={`${safe} de 5 estrellas`}
    >
      <span className={styles.base} aria-hidden="true">
        <FiveStars />
      </span>
      <span className={styles.fill} aria-hidden="true" style={{ width: `${(safe / 5) * 100}%` }}>
        <FiveStars />
      </span>
    </span>
  )
}
```

# `src/shared/components/RatingStars/RatingStars.module.css`

## Código completo

```css
.stars {
  position: relative;
  display: inline-flex;
  font-size: var(--font-size-sm);
  line-height: 1;
}

.lg {
  font-size: var(--font-size-lg);
}

.base,
.fill {
  display: flex;
}

.base {
  color: var(--color-disabled);
}

.fill {
  position: absolute;
  top: 0;
  left: 0;
  overflow: hidden;
  color: var(--color-secondary);
}

.stars svg {
  flex: none;
}
```

# `src/shared/components/Pagination/Pagination.tsx`

## Código completo

```tsx
import { MdChevronLeft, MdChevronRight } from 'react-icons/md'
import styles from './Pagination.module.css'

interface PaginationProps {
  page: number
  totalPages: number
  onChange: (page: number) => void
}

type PageItem = number | 'start-ellipsis' | 'end-ellipsis'

// Siempre incluye la primera y la última página
function getPageItems(page: number, total: number): PageItem[] {
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1)
  if (page <= 4) return [1, 2, 3, 4, 5, 'end-ellipsis', total]
  if (page >= total - 3) {
    return [1, 'start-ellipsis', total - 4, total - 3, total - 2, total - 1, total]
  }
  return [1, 'start-ellipsis', page - 1, page, page + 1, 'end-ellipsis', total]
}

export function Pagination({ page, totalPages, onChange }: PaginationProps) {
  if (totalPages <= 1) return null

  return (
    <nav className={styles.pagination} aria-label="Paginación">
      <button
        type="button"
        className={styles.arrow}
        aria-label="Página anterior"
        disabled={page <= 1}
        onClick={() => onChange(page - 1)}
      >
        <MdChevronLeft />
      </button>

      {getPageItems(page, totalPages).map((item) =>
        typeof item === 'string' ? (
          <span key={item} className={styles.ellipsis} aria-hidden="true">
            …
          </span>
        ) : (
          <button
            key={item}
            type="button"
            className={`${styles.page} ${item === page ? styles.current : ''}`}
            aria-label={`Página ${item}`}
            aria-current={item === page ? 'page' : undefined}
            onClick={() => onChange(item)}
          >
            {item}
          </button>
        ),
      )}

      <button
        type="button"
        className={styles.arrow}
        aria-label="Página siguiente"
        disabled={page >= totalPages}
        onClick={() => onChange(page + 1)}
      >
        <MdChevronRight />
      </button>
    </nav>
  )
}
```

# `src/shared/components/Pagination/Pagination.module.css`

## Código completo

```css
.pagination {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: center;
  gap: var(--spacing-xs);
  margin-top: var(--spacing-lg);
}

.page,
.arrow {
  display: grid;
  place-items: center;
  min-width: 32px;
  height: 32px;
  padding: 0 var(--spacing-xs);
  border: 0;
  border-radius: var(--radius-full);
  background: none;
  color: var(--color-text);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
}

.page:hover,
.arrow:hover:not(:disabled) {
  background: var(--color-primary-light);
}

.page.current,
.page.current:hover {
  background: var(--color-primary);
  color: var(--color-text-inverse);
}

.arrow {
  font-size: 22px;
}

.arrow:disabled {
  color: var(--color-disabled);
  cursor: not-allowed;
}

.ellipsis {
  min-width: 24px;
  color: var(--color-text-muted);
  text-align: center;
}
```

# `src/shared/components/Navbar/Navbar.tsx`

## Código completo

```tsx
import { useCallback, useState } from 'react'
import { MdMenu, MdPerson } from 'react-icons/md'
import { Link, NavLink } from 'react-router'
import { MobileMenu } from '../MobileMenu/MobileMenu'
import styles from './Navbar.module.css'
import { NAV_ITEMS } from './navItems'

interface NavbarProps {
  /** Nombre de quien tiene sesión. Sin él se muestra "Log in" y el acceso lleva a /login. */
  userName?: string
  avatarUrl?: string
  overlay?: boolean
}

const DESKTOP_LINKS = NAV_ITEMS.filter((item) => !item.mobileOnly)

export function Navbar({ userName, avatarUrl, overlay = false }: NavbarProps) {
  const [menuOpen, setMenuOpen] = useState(false)
  const closeMenu = useCallback(() => setMenuOpen(false), [])

  return (
    <header className={`${styles.navbar} ${overlay ? styles.overlay : ''}`}>
      <div className={styles.inner}>
        <button
          type="button"
          className={styles.menu}
          aria-label="Abrir menú"
          aria-expanded={menuOpen}
          aria-controls="mobile-menu"
          onClick={() => setMenuOpen(true)}
        >
          <MdMenu />
        </button>

        <Link to="/" className={styles.brand}>
          {/* TODO: reemplazar por <img src={logo} alt="Lectio" className={styles.logo} /> */}
          <span className={styles.logo} aria-hidden="true" />
          <span className={styles.brandName}>Lectio</span>
        </Link>

        <div className={styles.right}>
          <nav className={styles.links} aria-label="Principal">
            {DESKTOP_LINKS.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                end={link.to === '/'}
                className={({ isActive }) => (isActive ? styles.linkActive : undefined)}
              >
                {link.label}
              </NavLink>
            ))}
          </nav>

          <Link to={userName ? '/profile' : '/login'} className={styles.user}>
            <span className={styles.name}>{userName ?? 'Log in'}</span>
            {avatarUrl ? (
              <img className={styles.avatar} src={avatarUrl} alt="" />
            ) : (
              <span className={styles.avatar} aria-hidden="true">
                {userName ? userName.charAt(0).toUpperCase() : <MdPerson />}
              </span>
            )}
          </Link>
        </div>
      </div>

      <MobileMenu open={menuOpen} onClose={closeMenu} />
    </header>
  )
}
```

# `src/shared/components/Navbar/Navbar.module.css`

## Código completo

```css
.navbar {
  background: var(--color-bg);
}

.inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  max-width: 960px;
  margin: 0 auto;
  padding: var(--spacing-md);
}

.menu {
  display: grid;
  place-items: center;
  padding: var(--spacing-xs);
  border: 0;
  background: none;
  color: var(--color-text);
  font-size: 28px;
}

.menu span {
  width: 22px;
  height: 2px;
  border-radius: var(--radius-full);
  background: var(--color-text);
}

.brand {
  display: none;
  align-items: center;
  gap: var(--spacing-md);
  color: inherit;
  text-decoration: none;
}
.logo {
  width: 40px;
  height: 40px;
  border-radius: var(--radius-sm);
  background: var(--color-primary-hover);
  object-fit: cover;
}

.brandName {
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-medium);
}

.right {
  display: flex;
  align-items: center;
  gap: var(--spacing-xl);
}

.links {
  display: none;
  align-items: center;
  gap: var(--spacing-xl);
}

.links a {
  color: var(--color-text);
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-medium);
  text-decoration: none;
}

.links a:hover {
  color: var(--color-primary);
}

.links .linkActive {
  color: var(--color-primary);
}

.user {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  color: inherit;
  text-decoration: none;
}

.name {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
}

.avatar {
  display: grid;
  place-items: center;
  width: 36px;
  height: 36px;
  border-radius: var(--radius-full);
  object-fit: cover;
  background: var(--color-primary-light);
  color: var(--color-primary);
  font-weight: var(--font-weight-bold);
}

@media (min-width: 1024px) {
  .inner {
    max-width: none;
    padding: var(--spacing-md) clamp(var(--spacing-xl), 4vw, 80px);
  }

  .menu {
    display: none;
  }

  .brand,
  .links {
    display: flex;
  }

  .name {
    font-size: var(--font-size-md);
  }

  .avatar {
    width: 44px;
    height: 44px;
  }
}

.overlay {
  position: absolute;
  top: 0;
  right: 0;
  left: 0;
  z-index: 10;
  background: none;
}

.overlay::before {
  content: '';
  position: absolute;
  inset: 0;
  z-index: -1;
  background: linear-gradient(var(--overlay-dark), transparent);
  pointer-events: none;
}

.overlay .menu,
.overlay .name {
  color: var(--color-text-inverse);
}

@media (min-width: 1024px) {
  .overlay {
    position: static;
    background: var(--color-bg);
  }

  .overlay::before {
    display: none;
  }

  .overlay .name {
    color: var(--color-text);
  }
}
```

# `src/shared/components/Navbar/navItems.ts`

## Código completo

```ts
import type { IconType } from 'react-icons'
import { MdCategory, MdHistory, MdHome, MdPerson } from 'react-icons/md'

export interface NavItem {
  label: string
  to: string
  icon: IconType
  mobileOnly?: boolean
  disabled?: boolean
}

export const NAV_ITEMS: NavItem[] = [
  { label: 'Home', to: '/', icon: MdHome },
  { label: 'History', to: '/history', icon: MdHistory },
  { label: 'Categories', to: '/categories', icon: MdCategory },
  { label: 'Profile', to: '/profile', icon: MdPerson, mobileOnly: true },
]
```

# `src/shared/components/MobileMenu/MobileMenu.tsx`

## Código completo

```tsx
import { useEffect, useRef } from 'react'
import { MdClose } from 'react-icons/md'
import { NavLink } from 'react-router'
import { NAV_ITEMS } from '../Navbar/navItems'
import styles from './MobileMenu.module.css'

interface MobileMenuProps {
  open: boolean
  onClose: () => void
}

export function MobileMenu({ open, onClose }: MobileMenuProps) {
  const closeRef = useRef<HTMLButtonElement>(null)

  useEffect(() => {
    if (!open) return

    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onClose()
    }

    document.addEventListener('keydown', onKeyDown)
    document.body.style.overflow = 'hidden'
    closeRef.current?.focus()

    return () => {
      document.removeEventListener('keydown', onKeyDown)
      document.body.style.overflow = ''
    }
  }, [open, onClose])

  return (
    <>
      <div
        className={`${styles.overlay} ${open ? styles.overlayOpen : ''}`}
        onClick={onClose}
        aria-hidden="true"
      />

      <aside
        id="mobile-menu"
        className={`${styles.drawer} ${open ? styles.drawerOpen : ''}`}
        role="dialog"
        aria-modal="true"
        aria-label="Menú"
      >
        <div className={styles.header}>
          <span className={styles.title}>Lectio</span>
          <button
            ref={closeRef}
            type="button"
            className={styles.close}
            aria-label="Cerrar menú"
            onClick={onClose}
          >
            <MdClose />
          </button>
        </div>

        <nav aria-label="Menú principal">
          <ul className={styles.list}>
            {NAV_ITEMS.map((item) => {
              const Icon = item.icon

              return (
                <li key={item.to}>
                  {item.disabled ? (
                    <span className={`${styles.item} ${styles.disabled}`} aria-disabled="true">
                      <Icon className={styles.icon} aria-hidden="true" />
                      <span className={styles.label}>{item.label}</span>
                      <span className={styles.soon}>Soon</span>
                    </span>
                  ) : (
                    <NavLink
                      to={item.to}
                      end={item.to === '/'}
                      onClick={onClose}
                      className={({ isActive }) =>
                        `${styles.item} ${isActive ? styles.itemActive : ''}`
                      }
                    >
                      <Icon className={styles.icon} aria-hidden="true" />
                      <span className={styles.label}>{item.label}</span>
                    </NavLink>
                  )}
                </li>
              )
            })}
          </ul>
        </nav>
      </aside>
    </>
  )
}
```

# `src/shared/components/MobileMenu/MobileMenu.module.css`

## Código completo

```css
.overlay {
  position: fixed;
  inset: 0;
  z-index: 100;
  background: var(--overlay-dark);
  opacity: 0;
  visibility: hidden;
  transition:
    opacity 0.25s ease,
    visibility 0.25s;
}

.overlayOpen {
  opacity: 1;
  visibility: visible;
}

.drawer {
  position: fixed;
  top: 0;
  bottom: 0;
  left: 0;
  z-index: 101;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-lg);
  width: min(80vw, 320px);
  padding: var(--spacing-md);
  background: var(--color-bg);
  box-shadow: var(--shadow-lg);
  transform: translateX(-100%);
  visibility: hidden;
  transition:
    transform 0.25s ease,
    visibility 0.25s;
}

.drawerOpen {
  transform: none;
  visibility: visible;
}

.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.title {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-semibold);
}

.close {
  display: grid;
  place-items: center;
  padding: var(--spacing-xs);
  border: 0;
  background: none;
  color: var(--color-text);
  font-size: 28px;
}

.list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
  margin: 0;
  padding: 0;
  list-style: none;
}

.item {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border-radius: var(--radius-md);
  color: var(--color-text);
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-medium);
  text-decoration: none;
}

.item:hover {
  background: var(--color-surface);
}

.itemActive {
  background: var(--color-primary-light);
  color: var(--color-primary);
  font-weight: var(--font-weight-semibold);
}

.icon {
  flex: none;
  font-size: 24px;
}

.label {
  flex: 1;
}

.disabled {
  color: var(--color-text-muted);
  cursor: not-allowed;
}

.disabled:hover {
  background: none;
}

.soon {
  padding: 2px var(--spacing-sm);
  border-radius: var(--radius-full);
  background: var(--color-disabled);
  font-size: var(--font-size-xs);
}

@media (min-width: 1024px) {
  .overlay,
  .drawer {
    display: none;
  }
}
```

# `src/shared/components/Skeleton/Skeleton.tsx`

## Código completo

```tsx
import styles from './Skeleton.module.css'

interface SkeletonProps {
  className?: string
}

export function Skeleton({ className = '' }: SkeletonProps) {
  return <div className={`${styles.skeleton} ${className}`} aria-hidden="true" />
}
```

# `src/shared/components/Skeleton/Skeleton.module.css`

## Código completo

```css
.skeleton {
  background: var(--color-disabled);
  animation: pulse 1.2s ease-in-out infinite;
}

@keyframes pulse {
  50% {
    opacity: 0.5;
  }
}
```

# `src/shared/components/Button/Button.tsx`

## Código completo

```tsx
import type { ButtonHTMLAttributes } from 'react'
import styles from './Button.module.css'

interface ButtonProps extends Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'className'> {
  variant?: 'primary' | 'secondary'
  /** Ocupa todo el ancho disponible hasta 260px (como los botones verdes del mockup). */
  block?: boolean
}

export function Button({ variant = 'primary', block = false, type = 'button', ...props }: ButtonProps) {
  const className = [styles.button, styles[variant], block ? styles.block : '']
    .filter(Boolean)
    .join(' ')

  return <button type={type} className={className} {...props} />
}
```

# `src/shared/components/Button/Button.module.css`

## Código completo

```css
.button {
  min-height: 44px;
  padding: var(--spacing-sm) var(--spacing-lg);
  border: 2px solid transparent;
  border-radius: var(--radius-full);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.block {
  width: min(100%, 260px);
}

.primary {
  background: var(--color-primary);
  box-shadow: var(--shadow-md);
  color: var(--color-text-inverse);
}

.primary:hover:not(:disabled) {
  background: var(--color-primary-hover);
}

.secondary {
  border-color: var(--color-primary);
  background: var(--color-bg);
  color: var(--color-primary);
}

.secondary:hover:not(:disabled) {
  background: var(--color-primary-light);
}

.button:disabled {
  border-color: transparent;
  background: var(--color-disabled);
  box-shadow: none;
  color: var(--color-text-muted);
  cursor: not-allowed;
}

.button:focus-visible {
  outline: 2px solid var(--color-primary);
  outline-offset: 2px;
}
```

# `src/shared/components/BookHero/BookHero.tsx`

## Código completo

```tsx
import { MdChevronLeft, MdMenuBook } from 'react-icons/md'
import { toHttps } from '../../utils/book'
import styles from './BookHero.module.css'

interface BookHeroProps {
  title: string
  category?: string
  imageUrl: string | null
  onBack: () => void
  /** Nivel del título: 1 por defecto; 2 cuando la página ya tiene su propio <h1>. */
  headingLevel?: 1 | 2
}

export function BookHero({ title, category, imageUrl, onBack, headingLevel = 1 }: BookHeroProps) {
  const Heading = headingLevel === 1 ? 'h1' : 'h2'

  return (
    <section className={styles.hero}>
      {imageUrl ? (
        <img className={styles.cover} src={toHttps(imageUrl)} alt={`Portada de ${title}`} />
      ) : (
        <div className={styles.placeholder}>
          <MdMenuBook aria-hidden="true" />
        </div>
      )}

      <button type="button" className={styles.back} aria-label="Volver" onClick={onBack}>
        <MdChevronLeft aria-hidden="true" />
      </button>

      <div className={styles.caption}>
        <Heading className={styles.title}>{title}</Heading>
        {category && <p className={styles.category}>{category}</p>}
      </div>
    </section>
  )
}
```

# `src/shared/components/BookHero/BookHero.module.css`

## Código completo

```css
.hero {
  position: relative;
  height: 300px;
  overflow: hidden;
  background: var(--color-primary-hover);
}

.cover {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.placeholder {
  display: grid;
  place-items: center;
  height: 100%;
  color: var(--color-text-inverse);
  font-size: 64px;
}

.back {
  position: absolute;
  top: calc(var(--spacing-xl) * 2);
  left: var(--spacing-md);
  z-index: 1;
  display: grid;
  place-items: center;
  width: 36px;
  height: 36px;
  padding: 0;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-full);
  background: var(--color-bg);
  box-shadow: var(--shadow-sm);
  color: var(--color-primary);
  font-size: 24px;
}

.back:hover {
  background: var(--color-primary-light);
}

.back:focus-visible {
  outline: 2px solid var(--color-primary);
  outline-offset: 2px;
}

.caption {
  position: absolute;
  right: 0;
  bottom: 0;
  left: 0;
  padding: var(--spacing-sm) var(--spacing-md);
  background: var(--overlay-dark);
  color: var(--color-text-inverse);
}

.title {
  display: -webkit-box;
  margin: 0;
  overflow: hidden;
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-bold);
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.category {
  margin: 0;
  font-size: var(--font-size-sm);
}

@media (min-width: 768px) {
  .hero {
    height: 360px;
  }
}

@media (min-width: 1024px) {
  .hero {
    height: auto;
    aspect-ratio: 3 / 4;
    max-height: calc(100vh - 160px);
    border-radius: var(--radius-md);
    box-shadow: var(--shadow-lg);
  }

  .back {
    top: var(--spacing-md);
  }

  .title {
    font-size: var(--font-size-lg);
  }
}
```

# `src/shared/components/TextField/TextField.tsx`

## Código completo

```tsx
import { useId, type InputHTMLAttributes } from 'react'
import styles from './TextField.module.css'

interface TextFieldProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'id' | 'className'> {
  label: string
  error?: string
  hint?: string
}

export function TextField({ label, error, hint, ...props }: TextFieldProps) {
  const id = useId()
  const hintId = `${id}-hint`
  const errorId = `${id}-error`
  const describedBy = [hint ? hintId : null, error ? errorId : null].filter(Boolean).join(' ')

  return (
    <div className={styles.field}>
      <label htmlFor={id} className={styles.label}>
        {label}
      </label>
      <input
        id={id}
        className={`${styles.input} ${error ? styles.inputError : ''}`}
        aria-invalid={error ? true : undefined}
        aria-describedby={describedBy || undefined}
        {...props}
      />
      {hint && (
        <p id={hintId} className={styles.hint}>
          {hint}
        </p>
      )}
      {error && (
        <p id={errorId} className={styles.error}>
          {error}
        </p>
      )}
    </div>
  )
}
```

# `src/shared/components/TextField/TextField.module.css`

## Código completo

```css
.field {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.label {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
}

.input {
  width: 100%;
  padding: var(--spacing-sm) var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  background: var(--color-bg);
  color: var(--color-text);
  font: inherit;
  font-size: var(--font-size-md);
}

.input:focus-visible {
  outline: 2px solid var(--color-primary);
  outline-offset: 1px;
}

.inputError {
  border-color: var(--color-error);
}

.hint,
.error {
  margin: 0;
  font-size: var(--font-size-xs);
}

.hint {
  color: var(--color-text-muted);
}

.error {
  color: var(--color-error);
}
```

# `src/shared/components/AuthForm/AuthForm.tsx`

## Código completo

```tsx
import type { FormEvent, ReactNode } from 'react'
import { Button } from '../Button/Button'
import styles from './AuthForm.module.css'

interface AuthFormProps {
  title: string
  subtitle: string
  submitLabel: string
  submittingLabel: string
  submitting: boolean
  /** Error del servidor (o de red) para mostrar sobre el botón. */
  error: string | null
  onSubmit: (event: FormEvent<HTMLFormElement>) => void
  /** Enlace a la otra pantalla ("¿No tienes cuenta?"). */
  footer: ReactNode
  /** Los campos (`TextField`). */
  children: ReactNode
}

export function AuthForm({
  title,
  subtitle,
  submitLabel,
  submittingLabel,
  submitting,
  error,
  onSubmit,
  footer,
  children,
}: AuthFormProps) {
  return (
    <main className={styles.main}>
      <div className={styles.card}>
        <h1 className={styles.title}>{title}</h1>
        <p className={styles.subtitle}>{subtitle}</p>

        <form className={styles.form} onSubmit={onSubmit} noValidate>
          {children}

          {error && (
            <p role="alert" className={styles.error}>
              {error}
            </p>
          )}

          <div className={styles.actions}>
            <Button type="submit" block disabled={submitting}>
              {submitting ? submittingLabel : submitLabel}
            </Button>
          </div>
        </form>

        <p className={styles.footer}>{footer}</p>
      </div>
    </main>
  )
}
```

# `src/shared/components/AuthForm/AuthForm.module.css`

## Código completo

```css
.main {
  padding: var(--spacing-lg) var(--spacing-md) var(--spacing-xl);
}

.card {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  max-width: 420px;
  margin: 0 auto;
  padding: var(--spacing-lg);
  border-radius: var(--radius-md);
  background: var(--color-surface);
  box-shadow: var(--shadow-md);
}

.title {
  margin: 0;
  color: var(--color-primary);
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-bold);
  text-align: center;
}

.subtitle {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  text-align: center;
}

.form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.error {
  margin: 0;
  color: var(--color-error);
  font-size: var(--font-size-sm);
  text-align: center;
}

.actions {
  display: flex;
  justify-content: center;
}

.footer {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  text-align: center;
}

.footer a {
  color: var(--color-primary);
  font-weight: var(--font-weight-semibold);
}

@media (min-width: 1024px) {
  .main {
    padding-top: var(--spacing-xl);
  }
}
```

<!-- ===== 8. Componentes específicos de features ===== -->

# SECCIÓN 8. Componentes específicos de features

# `src/features/home/components/HeroCarousel/HeroCarousel.tsx`

## Código completo

```tsx
import { useRef, useState } from 'react'
import type { Book } from '../../../../api/types/book'
import { RatingStars } from '../../../../shared/components/RatingStars/RatingStars'
import { formatAuthors, toHttps } from '../../../../shared/utils/book'
import styles from './HeroCarousel.module.css'

interface HeroCarouselProps {
  books: Book[]
  loading: boolean
  onOpen?: (bookId: number) => void
}

export function HeroCarousel({ books, loading, onOpen }: HeroCarouselProps) {
  const trackRef = useRef<HTMLDivElement>(null)
  const [active, setActive] = useState(0)

  const handleScroll = () => {
    const el = trackRef.current
    if (el) setActive(Math.round(el.scrollLeft / el.clientWidth))
  }

  const goTo = (index: number) => {
    const el = trackRef.current
    el?.scrollTo({ left: index * el.clientWidth, behavior: 'smooth' })
  }

  if (loading) return <div className={styles.skeleton} aria-busy="true" />
  if (books.length === 0) return null

  const current = books[active] ?? books[0]
  const next = () => setActive((i) => (i + 1) % books.length)

  return (
    <section aria-label="Libros más visitados">
      {/* MOBILE: swipe con puntos */}
      <div className={styles.mobile}>
        <div ref={trackRef} className={styles.track} onScroll={handleScroll}>
          {books.map((book) => (
            <button
              key={book.id}
              type="button"
              className={styles.slide}
              onClick={() => onOpen?.(book.id)}
            >
              {book.imageUrl && (
                <>
                  <img
                    className={styles.bg}
                    src={toHttps(book.imageUrl)}
                    alt=""
                    aria-hidden="true"
                  />
                  <div className={styles.shade} />
                  <img
                    className={styles.cover}
                    src={toHttps(book.imageUrl)}
                    alt={`Portada de ${book.title}`}
                  />
                </>
              )}
              {book.categories[0] && (
                <span className={styles.badge}>{book.categories[0].name}</span>
              )}
              <span className={styles.caption}>{book.title}</span>
            </button>
          ))}
        </div>

        <div className={styles.dots}>
          {books.map((book, i) => (
            <button
              key={book.id}
              type="button"
              aria-label={`Ir al libro ${i + 1}`}
              className={`${styles.dot} ${i === active ? styles.dotActive : ''}`}
              onClick={() => goTo(i)}
            />
          ))}
        </div>
      </div>

      {/* DESKTOP: libro destacado + botón Next */}
      <div className={styles.desktop}>
        <div className={styles.media}>
          {current.imageUrl && (
            <>
              <img
                className={styles.bg}
                src={toHttps(current.imageUrl)}
                alt=""
                aria-hidden="true"
              />
              <div className={styles.shade} />
              <img
                className={styles.cover}
                src={toHttps(current.imageUrl)}
                alt={`Portada de ${current.title}`}
              />
            </>
          )}
          {current.categories[0] && (
            <span className={styles.badge}>{current.categories[0].name}</span>
          )}
          <div className={styles.mediaCaption}>
            <span className={styles.mediaTitle}>{current.title}</span>
            <span className={styles.ratingPill}>
              <RatingStars value={current.rating?.stars ?? 0} />
            </span>
          </div>
        </div>

        <div className={styles.info}>
          <h3 className={styles.title}>{current.title}</h3>
          <p className={styles.by}>By: {formatAuthors(current.authors)}</p>
          <p className={styles.description}>
            {current.description ?? 'Sin descripción disponible.'}
          </p>
          <div className={styles.actions}>
            <button type="button" className={styles.action} onClick={() => onOpen?.(current.id)}>
              Start Reading
            </button>
            <button
              type="button"
              className={styles.action}
              onClick={next}
              disabled={books.length < 2}
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </section>
  )
}
```

# `src/features/home/components/HeroCarousel/HeroCarousel.module.css`

## Código completo

```css
.mobile {
  display: block;
}

.desktop {
  display: none;
}

.track {
  display: flex;
  overflow-x: auto;
  scroll-snap-type: x mandatory;
  scrollbar-width: none;
  border-radius: var(--radius-md);
}

.track::-webkit-scrollbar {
  display: none;
}

.slide {
  position: relative;
  flex: 0 0 100%;
  height: 200px;
  padding: 0;
  overflow: hidden;
  border: 0;
  background: #111;
  scroll-snap-align: start;
}

.bg {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  filter: blur(14px);
  transform: scale(1.2);
}

.shade {
  position: absolute;
  inset: 0;
  background: var(--overlay-dark);
}

.cover {
  position: absolute;
  top: 12px;
  left: 50%;
  width: auto;
  height: calc(100% - 52px);
  border-radius: 4px;
  box-shadow: var(--shadow-lg);
  transform: translateX(-50%);
}

.badge {
  position: absolute;
  top: var(--spacing-md);
  left: var(--spacing-md);
  padding: var(--spacing-xs) var(--spacing-md);
  border-radius: var(--radius-sm);
  background: var(--color-primary);
  color: var(--color-text-inverse);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
}

.caption {
  position: absolute;
  right: 0;
  bottom: 0;
  left: 0;
  padding: var(--spacing-sm);
  overflow: hidden;
  background: var(--overlay-dark);
  color: var(--color-text-inverse);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  text-align: center;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.dots {
  display: flex;
  justify-content: center;
  gap: var(--spacing-sm);
  margin-top: var(--spacing-sm);
}

.dot {
  width: 8px;
  height: 8px;
  padding: 0;
  border: 0;
  border-radius: var(--radius-full);
  background: var(--color-disabled);
}

.dotActive {
  background: var(--color-primary);
}

.skeleton {
  height: 200px;
  border-radius: var(--radius-md);
  background: var(--color-disabled);
  animation: pulse 1.2s ease-in-out infinite;
}

@keyframes pulse {
  50% {
    opacity: 0.5;
  }
}

.media {
  position: relative;
  height: 320px;
  overflow: hidden;
  border-radius: var(--radius-md);
  background: #111;
}

.mediaCaption {
  position: absolute;
  right: 0;
  bottom: 0;
  left: 0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-md);
  padding: var(--spacing-sm) var(--spacing-md);
  background: var(--overlay-dark);
  color: var(--color-text-inverse);
}

.mediaTitle {
  min-width: 0;
  overflow: hidden;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  text-overflow: ellipsis;
  white-space: nowrap;
}

.ratingPill {
  flex: none;
  padding: 2px var(--spacing-sm);
  border-radius: var(--radius-sm);
  background: var(--color-bg);
}

.info {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: var(--spacing-sm);
  min-width: 0;
}

.title {
  margin: 0;
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-bold);
}

.by {
  margin: 0;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.description {
  display: -webkit-box;
  margin: 0;
  overflow: hidden;
  font-size: var(--font-size-sm);
  line-height: 1.5;
  -webkit-line-clamp: 5;
  -webkit-box-orient: vertical;
}

.actions {
  display: flex;
  gap: var(--spacing-md);
  margin-top: var(--spacing-md);
}

.action {
  flex: 1;
  padding: var(--spacing-sm) var(--spacing-md);
  border: 0;
  border-radius: var(--radius-full);
  background: var(--color-primary);
  box-shadow: var(--shadow-md);
  color: var(--color-text-inverse);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.action:hover:not(:disabled) {
  background: var(--color-primary-hover);
}

.action:disabled {
  background: var(--color-disabled);
  box-shadow: none;
  color: var(--color-text-muted);
  cursor: not-allowed;
}

@media (min-width: 768px) {
  .slide,
  .skeleton {
    height: 280px;
  }
}

@media (min-width: 1024px) {
  .mobile {
    display: none;
  }

  .desktop {
    display: grid;
    grid-template-columns: minmax(0, 5fr) minmax(0, 6fr);
    gap: var(--spacing-lg);
  }

  .skeleton {
    height: 320px;
  }
}
```

# `src/features/home/components/CategoryShortcuts/CategoryShortcuts.tsx`

## Código completo

```tsx
import type { CSSProperties } from 'react'
import { MdGridView } from 'react-icons/md'
import type { Category } from '../../../../api/types/category'
import {
  CATEGORY_ICONS,
  CATEGORY_LABELS,
  DEFAULT_CATEGORY_ICON,
  FEATURED_CATEGORIES,
  MOBILE_VISIBLE_CATEGORIES,
} from '../../constants'
import styles from './CategoryShortcuts.module.css'

interface CategoryShortcutsProps {
  categories: Category[]
  selectedId?: number
  onSelect: (categoryId?: number) => void
  onSeeMore: () => void
}

export function CategoryShortcuts({
  categories,
  selectedId,
  onSelect,
  onSeeMore,
}: CategoryShortcutsProps) {
  const featured = FEATURED_CATEGORIES.map((name) => categories.find((c) => c.name === name)).filter(
    (c): c is Category => Boolean(c),
  )

  // Columnas = tiles visibles + "See more" (mobile muestra menos que desktop)
  const mobileColumns = Math.min(featured.length, MOBILE_VISIBLE_CATEGORIES) + 1
  const desktopColumns = featured.length + 1

  // Click de nuevo sobre la categoría activa = quitar filtro
  const toggle = (id: number) => onSelect(id === selectedId ? undefined : id)

  return (
    <div
      className={styles.row}
      style={{ '--cols': mobileColumns, '--cols-desktop': desktopColumns } as CSSProperties}
    >
      {featured.map((c, index) => {
        const Icon = CATEGORY_ICONS[c.name] ?? DEFAULT_CATEGORY_ICON

        return (
          <button
            key={c.id}
            type="button"
            aria-pressed={c.id === selectedId}
            aria-label={c.name}
            className={[
              styles.tile,
              index >= MOBILE_VISIBLE_CATEGORIES ? styles.desktopOnly : '',
              c.id === selectedId ? styles.selected : '',
            ].join(' ')}
            onClick={() => toggle(c.id)}
          >
            <span className={styles.icon}>
              <Icon aria-hidden="true" />
            </span>
            <span className={styles.label}>{CATEGORY_LABELS[c.name] ?? c.name}</span>
          </button>
        )
      })}

      <button type="button" className={styles.tile} onClick={onSeeMore}>
        <span className={styles.icon}>
          <MdGridView aria-hidden="true" />
        </span>
        <span className={styles.label}>See more</span>
      </button>
    </div>
  )
}
```

# `src/features/home/components/CategoryShortcuts/CategoryShortcuts.module.css`

## Código completo

```css
.row {
  display: grid;
  grid-template-columns: repeat(var(--cols), minmax(0, 1fr));
  width: 100%;
  gap: var(--spacing-sm);
}

.tile {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-sm);
  min-width: 0;
  padding: 0;
  border: 0;
  background: none;
}

.desktopOnly {
  display: none;
}

.icon {
  display: grid;
  place-items: center;
  width: 100%;
  aspect-ratio: 1 / 1.15;
  border: 2px solid transparent;
  border-radius: var(--radius-md);
  background: var(--color-disabled);
  font-size: clamp(28px, 8vw, 40px);
}

.selected .icon {
  border-color: var(--color-primary);
  background: var(--color-primary-light);
  color: var(--color-primary);
}

.label {
  width: 100%;
  overflow: hidden;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  text-align: center;
  text-overflow: ellipsis;
  white-space: nowrap;
}

@media (min-width: 1024px) {
  .row {
    grid-template-columns: repeat(var(--cols-desktop), minmax(0, 1fr));
    gap: var(--spacing-md);
  }

  .desktopOnly {
    display: flex;
  }

  .icon {
    aspect-ratio: auto;
    height: clamp(72px, 7vw, 120px);
    font-size: clamp(32px, 3vw, 52px);
  }

  .label {
    width: max-content;
    overflow: visible;
  }
}
```

# `src/features/home/components/BookList/BookList.tsx`

## Código completo

```tsx
import type { Book } from '../../../../api/types/book'
import { BookCard } from '../../../../shared/components/BookCard/BookCard'
import styles from './BookList.module.css'

interface BookListProps {
  books?: Book[]
  loading: boolean
  error?: string
  onOpen?: (bookId: number) => void
}

export function BookList({ books, loading, error, onOpen }: BookListProps) {
  if (error) return <p role="alert" className={styles.message}>{error}</p>

  if (loading) {
    return (
      <div className={styles.grid} aria-busy="true">
        {Array.from({ length: 4 }, (_, i) => (
          <div key={i} className={styles.skeleton} />
        ))}
      </div>
    )
  }

  if (!books || books.length === 0) {
    return <p className={styles.message}>No hay libros en esta categoría todavía.</p>
  }

  return (
    <div className={styles.grid}>
      {books.map((book) => (
        <BookCard key={book.id} book={book} onOpen={onOpen} />
      ))}
    </div>
  )
}
```

# `src/features/home/components/BookList/BookList.module.css`

## Código completo

```css
.grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: var(--spacing-md);
}

.skeleton {
  aspect-ratio: 3 / 5;
  border-radius: var(--radius-md);
  background: var(--color-disabled);
  animation: pulse 1.2s ease-in-out infinite;
}

.message {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  text-align: center;
}

@keyframes pulse {
  50% {
    opacity: 0.5;
  }
}

@media (min-width: 768px) {
  .grid {
    grid-template-columns: repeat(4, minmax(0, 1fr));
  }
}

@media (min-width: 1024px) {
  .grid {
    grid-template-columns: repeat(5, minmax(0, 1fr));
    gap: var(--spacing-lg);
  }
}
```

# `src/features/categories/components/CategoryFilter/CategoryFilter.tsx`

## Código completo

```tsx
import { useEffect, useId, useRef, useState } from 'react'
import { MdCheck, MdExpandLess, MdExpandMore } from 'react-icons/md'
import type { Category } from '../../../../api/types/category'
import styles from './CategoryFilter.module.css'

interface CategoryFilterProps {
  categories: Category[]
  selectedIds: number[]
  onChange: (ids: number[]) => void
  loading?: boolean
}

function getTriggerLabel(categories: Category[], selectedIds: number[]) {
  if (selectedIds.length === 0) return 'All categories'

  const names = categories.filter((c) => selectedIds.includes(c.id)).map((c) => c.name)
  if (names.length === 0) return `${selectedIds.length} selected`
  if (names.length <= 2) return names.join(', ')
  return `${names.length} categories`
}

export function CategoryFilter({ categories, selectedIds, onChange, loading }: CategoryFilterProps) {
  const [open, setOpen] = useState(false)
  const rootRef = useRef<HTMLDivElement>(null)
  const labelId = useId()
  const panelId = useId()

  useEffect(() => {
    if (!open) return

    const onPointerDown = (event: PointerEvent) => {
      if (!rootRef.current?.contains(event.target as Node)) setOpen(false)
    }
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') setOpen(false)
    }

    document.addEventListener('pointerdown', onPointerDown)
    document.addEventListener('keydown', onKeyDown)
    return () => {
      document.removeEventListener('pointerdown', onPointerDown)
      document.removeEventListener('keydown', onKeyDown)
    }
  }, [open])

  const toggle = (id: number) =>
    onChange(selectedIds.includes(id) ? selectedIds.filter((x) => x !== id) : [...selectedIds, id])

  const Chevron = open ? MdExpandLess : MdExpandMore

  return (
    <div className={styles.root} ref={rootRef}>
      <span id={labelId} className={styles.label}>
        Filter by category
      </span>

      <button
        type="button"
        className={styles.trigger}
        aria-expanded={open}
        aria-controls={panelId}
        aria-labelledby={labelId}
        onClick={() => setOpen((v) => !v)}
      >
        <span className={styles.triggerLabel}>{getTriggerLabel(categories, selectedIds)}</span>
        <Chevron className={styles.triggerIcon} aria-hidden="true" />
      </button>

      {open && (
        <div id={panelId} className={styles.panel} role="group" aria-labelledby={labelId}>
          {categories.length === 0 ? (
            <p className={styles.message}>
              {loading ? 'Loading categories…' : 'No categories available'}
            </p>
          ) : (
            <ul className={styles.list}>
              {categories.map((category) => {
                const selected = selectedIds.includes(category.id)

                return (
                  <li key={category.id}>
                    <label className={`${styles.item} ${selected ? styles.itemSelected : ''}`}>
                      <input
                        type="checkbox"
                        className={styles.checkbox}
                        checked={selected}
                        onChange={() => toggle(category.id)}
                      />
                      <span>{category.name}</span>
                      {selected && <MdCheck className={styles.check} aria-hidden="true" />}
                    </label>
                  </li>
                )
              })}
            </ul>
          )}

          {selectedIds.length > 0 && (
            <button type="button" className={styles.clear} onClick={() => onChange([])}>
              Clear filters
            </button>
          )}
        </div>
      )}
    </div>
  )
}
```

# `src/features/categories/components/CategoryFilter/CategoryFilter.module.css`

## Código completo

```css
.root {
  position: relative;
}

.label {
  display: block;
  margin-bottom: var(--spacing-xs);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
}

.trigger {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-sm);
  width: 100%;
  padding: var(--spacing-sm) var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  background: var(--color-bg);
  color: var(--color-text);
  font-size: var(--font-size-md);
  text-align: left;
}

.triggerLabel {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.triggerIcon {
  flex: none;
  font-size: 24px;
}

.panel {
  position: absolute;
  top: calc(100% + var(--spacing-xs));
  right: 0;
  left: 0;
  z-index: 20;
  max-height: 360px;
  padding: var(--spacing-sm);
  overflow-y: auto;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  background: var(--color-bg);
  box-shadow: var(--shadow-lg);
}

.list {
  display: grid;
  grid-template-columns: 1fr;
  gap: 2px;
  margin: 0;
  padding: 0;
  list-style: none;
}

.item {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-sm);
  padding: var(--spacing-sm) var(--spacing-md);
  border-radius: var(--radius-sm);
  font-size: var(--font-size-sm);
  cursor: pointer;
}

.item:hover {
  background: var(--color-surface);
}

.itemSelected,
.itemSelected:hover {
  background: var(--color-primary);
  color: var(--color-text-inverse);
}

.checkbox {
  position: absolute;
  opacity: 0;
  pointer-events: none;
}

.item:has(.checkbox:focus-visible) {
  outline: 2px solid var(--color-primary);
  outline-offset: -2px;
}

.check {
  flex: none;
  font-size: 18px;
}

.clear {
  display: block;
  margin: var(--spacing-sm) auto 0;
  padding: var(--spacing-xs) var(--spacing-md);
  border: 0;
  background: none;
  color: var(--color-primary);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.message {
  margin: 0;
  padding: var(--spacing-sm);
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  text-align: center;
}

@media (min-width: 1024px) {
  .list {
    grid-template-columns: repeat(5, minmax(0, 1fr));
  }
}
```

# `src/features/book-detail/components/BookTabs/BookTabs.tsx`

## Código completo

```tsx
import { BOOK_TABS, getPanelId, getTabId, type BookTabId } from '../../constants'
import styles from './BookTabs.module.css'

interface BookTabsProps {
  active: BookTabId
  onChange: (tab: BookTabId) => void
}

export function BookTabs({ active, onChange }: BookTabsProps) {
  return (
    <div className={styles.tabs} role="tablist" aria-label="Secciones del libro">
      {BOOK_TABS.map((tab) => {
        const selected = tab.id === active

        return (
          <button
            key={tab.id}
            type="button"
            role="tab"
            id={getTabId(tab.id)}
            aria-selected={selected}
            aria-controls={selected ? getPanelId(tab.id) : undefined}
            className={`${styles.tab} ${selected ? styles.tabActive : ''}`}
            onClick={() => onChange(tab.id)}
          >
            {tab.label}
          </button>
        )
      })}
    </div>
  )
}
```

# `src/features/book-detail/components/BookTabs/BookTabs.module.css`

## Código completo

```css
.tabs {
  display: flex;
  gap: var(--spacing-xl);
}

.tab {
  padding: var(--spacing-xs) 0;
  border: 0;
  background: none;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-regular);
}

.tab:hover {
  color: var(--color-primary);
}

.tabActive,
.tabActive:hover {
  color: var(--color-primary);
  font-weight: var(--font-weight-semibold);
}

.tab:focus-visible {
  outline: 2px solid var(--color-primary);
  outline-offset: 1px;
}
```

# `src/features/book-detail/components/BookInfoList/BookInfoList.tsx`

## Código completo

```tsx
import type { IconType } from 'react-icons'
import { MdApartment, MdCalendarMonth, MdGroups, MdLayers, MdLocalOffer } from 'react-icons/md'
import type { BookDetail } from '../../../../api/types/book'
import styles from './BookInfoList.module.css'

interface BookInfoListProps {
  book: BookDetail
}

interface InfoRow {
  icon: IconType
  label: string
  value: string
}

const UNKNOWN = 'Unknown'

function joinNames(items: { name: string }[]) {
  return items.length > 0 ? items.map((item) => item.name).join(', ') : UNKNOWN
}

export function BookInfoList({ book }: BookInfoListProps) {
  const rows: InfoRow[] = [
    {
      icon: MdGroups,
      label: book.authors.length > 1 ? 'Authors' : 'Author',
      value: joinNames(book.authors),
    },
    { icon: MdApartment, label: 'Editorial', value: book.publisher ?? UNKNOWN },
    // El backend solo guarda el año de publicación, no la fecha completa
    { icon: MdCalendarMonth, label: 'Publication', value: book.publishedYear?.toString() ?? UNKNOWN },
    { icon: MdLayers, label: 'Pages', value: book.pageCount?.toString() ?? UNKNOWN },
    { icon: MdLocalOffer, label: 'Categories', value: joinNames(book.categories) },
  ]

  return (
    <ul className={styles.list}>
      {rows.map(({ icon: Icon, label, value }) => (
        <li key={label} className={styles.row}>
          <Icon className={styles.icon} aria-hidden="true" />
          <span className={styles.text}>
            {label}: {value}
          </span>
        </li>
      ))}
    </ul>
  )
}
```

# `src/features/book-detail/components/BookInfoList/BookInfoList.module.css`

## Código completo

```css
.list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  margin: 0;
  padding: 0;
  list-style: none;
}

.row {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}

.icon {
  flex: none;
  font-size: 24px;
}

.text {
  min-width: 0;
  font-size: var(--font-size-sm);
  overflow-wrap: anywhere;
}
```

# `src/features/book-detail/components/LoanButton/LoanButton.tsx`

## Código completo

```tsx
import type { LoanStatus } from '../../../../api/types/loan'
import { Button } from '../../../../shared/components/Button/Button'

interface LoanButtonProps {
  status?: LoanStatus
  loading?: boolean
  hasError?: boolean
  onAction?: (status: LoanStatus) => void
}

interface ButtonState {
  label: string
  disabled: boolean
}

const UNAVAILABLE: ButtonState = { label: 'Unavailable', disabled: true }

const STATES: Record<LoanStatus, ButtonState> = {
  LoginRequired: { label: 'Log in to borrow', disabled: false },
  Available: { label: 'Borrow', disabled: false },
  BorrowedByYou: { label: 'Deliver', disabled: false },
  Unavailable: UNAVAILABLE,
}

function getState(status: LoanStatus | undefined, loading: boolean, hasError: boolean): ButtonState {
  if (loading) return { label: 'Loading…', disabled: true }
  if (hasError || !status) return { label: 'Status unavailable', disabled: true }
  return STATES[status] ?? UNAVAILABLE
}

export function LoanButton({ status, loading = false, hasError = false, onAction }: LoanButtonProps) {
  const state = getState(status, loading, hasError)

  return (
    <Button
      block
      disabled={state.disabled}
      aria-busy={loading}
      onClick={() => status && onAction?.(status)}
    >
      {state.label}
    </Button>
  )
}
```

# `src/features/book-detail/components/ExpandableText/ExpandableText.tsx`

## Código completo

```tsx
import { useEffect, useId, useRef, useState, type CSSProperties } from 'react'
import styles from './ExpandableText.module.css'

interface ExpandableTextProps {
  text: string
  lines?: number
}

export function ExpandableText({ text, lines = 14 }: ExpandableTextProps) {
  const textId = useId()
  const textRef = useRef<HTMLParagraphElement>(null)
  const [expanded, setExpanded] = useState(false)
  const [overflowing, setOverflowing] = useState(false)

  useEffect(() => {
    const el = textRef.current
    if (!el || expanded) return

    const observer = new ResizeObserver(() => {
      setOverflowing(el.scrollHeight > el.clientHeight + 1)
    })
    observer.observe(el)

    return () => observer.disconnect()
  }, [text, lines, expanded])

  return (
    <div>
      <p
        id={textId}
        ref={textRef}
        className={`${styles.text} ${expanded ? '' : styles.clamped}`}
        style={{ '--lines': lines } as CSSProperties}
      >
        {text}
      </p>

      {(overflowing || expanded) && (
        <button
          type="button"
          className={styles.toggle}
          aria-expanded={expanded}
          aria-controls={textId}
          onClick={() => setExpanded((value) => !value)}
        >
          {expanded ? 'See less' : 'See more'}
        </button>
      )}
    </div>
  )
}
```

# `src/features/book-detail/components/ExpandableText/ExpandableText.module.css`

## Código completo

```css
.text {
  margin: 0;
  font-size: var(--font-size-sm);
  line-height: 1.5;
  text-align: justify;
}

.clamped {
  display: -webkit-box;
  overflow: hidden;
  -webkit-line-clamp: var(--lines);
  -webkit-box-orient: vertical;
}

.toggle {
  margin-top: var(--spacing-sm);
  padding: 0;
  border: 0;
  background: none;
  color: var(--color-primary);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}

.toggle:hover {
  color: var(--color-primary-hover);
  text-decoration: underline;
}

.toggle:focus-visible {
  outline: 2px solid var(--color-primary);
  outline-offset: 2px;
}
```

# `src/features/book-reviews/components/RatingSummary/RatingSummary.tsx`

## Código completo

```tsx
import { RatingStars } from '../../../../shared/components/RatingStars/RatingStars'
import styles from './RatingSummary.module.css'

interface RatingSummaryProps {
  average: number
  count: number
}

export function RatingSummary({ average, count }: RatingSummaryProps) {
  return (
    <section className={styles.summary} aria-label="General rate">
      <p className={styles.label}>General Rate</p>
      <p className={styles.average}>{average.toFixed(1)}</p>
      <div className={styles.stars}>
        <RatingStars value={average} size="lg" />
      </div>
      <p className={styles.count}>rate counter: {count}</p>
    </section>
  )
}
```

# `src/features/book-reviews/components/RatingSummary/RatingSummary.module.css`

## Código completo

```css
.summary {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-sm);
  text-align: center;
}

.label {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-medium);
}

.average {
  margin: 0;
  color: var(--color-primary);
  font-size: var(--font-size-3xl);
  font-weight: var(--font-weight-bold);
  line-height: 1;
}

.stars {
  padding: var(--spacing-xs) var(--spacing-md);
  border-radius: var(--radius-sm);
  background: var(--color-surface);
  box-shadow: var(--shadow-md);
}

.count {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-xs);
}
```

# `src/features/book-reviews/components/ReviewCard/ReviewCard.tsx`

## Código completo

```tsx
import { MdPerson } from 'react-icons/md'
import type { BookRatingUser } from '../../../../api/types/rating'
import { RatingStars } from '../../../../shared/components/RatingStars/RatingStars'
import { formatReviewDate } from '../../utils'
import styles from './ReviewCard.module.css'

interface ReviewCardProps {
  review: BookRatingUser
}

export function ReviewCard({ review }: ReviewCardProps) {
  return (
    <article className={styles.card}>
      <div className={styles.header}>
        <span className={styles.avatar} aria-hidden="true">
          <MdPerson />
        </span>
        {/* El backend no expone nombres, solo el id del autor */}
        <p className={styles.name}>{review.fullName}</p>
      </div>

      {review.comment && <p className={styles.comment}>{review.comment}</p>}

      <div className={styles.footer}>
        <time className={styles.date} dateTime={review.updatedAt}>
          {formatReviewDate(review.updatedAt)}
        </time>
        <RatingStars value={review.rating} size="lg" />
      </div>
    </article>
  )
}
```

# `src/features/book-reviews/components/ReviewCard/ReviewCard.module.css`

## Código completo

```css
.card {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-sm);
  padding: var(--spacing-md);
  border-radius: var(--radius-md);
  background: var(--color-surface);
  box-shadow: var(--shadow-md);
}

.header {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}

.avatar {
  display: grid;
  flex: none;
  place-items: center;
  width: 36px;
  height: 36px;
  border-radius: var(--radius-full);
  background: var(--color-primary-light);
  color: var(--color-primary);
  font-size: 22px;
}

.name {
  margin: 0;
  min-width: 0;
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-semibold);
  overflow-wrap: anywhere;
}

.comment {
  margin: 0;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-regular);
  overflow-wrap: anywhere;
}

.footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--spacing-sm);
}

.date {
  color: var(--color-text-muted);
  font-size: var(--font-size-xs);
  font-style: italic;
}
```

# `src/features/book-reviews/components/ReviewList/ReviewList.tsx`

## Código completo

```tsx
import type { BookRatingUser } from '../../../../api/types/rating'
import { ReviewCard } from '../ReviewCard/ReviewCard'
import styles from './ReviewList.module.css'

interface ReviewListProps {
  reviews: BookRatingUser[]
}

export function ReviewList({ reviews }: ReviewListProps) {
  return (
    <ul className={styles.list}>
      {reviews.map((review) => (
        // Una reseña por usuario y libro: `idUser` es único dentro de la lista
        <li key={review.idUser}>
          <ReviewCard review={review} />
        </li>
      ))}
    </ul>
  )
}
```

# `src/features/book-reviews/components/ReviewList/ReviewList.module.css`

## Código completo

```css
.list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  margin: 0;
  padding: 0;
  list-style: none;
}
```

# `src/features/book-reviews/components/ReviewForm/ReviewForm.tsx`

## Código completo

```tsx
import { useId, useState, type FormEvent } from 'react'
import { MdStar } from 'react-icons/md'
import { Button } from '../../../../shared/components/Button/Button'
import { MAX_COMMENT_LENGTH, STAR_VALUES } from '../../constants'
import styles from './ReviewForm.module.css'

export interface ReviewFormValues {
  stars: number
  comment: string
}

interface ReviewFormProps {
  submitting: boolean
  error: string | null
  onSubmit: (values: ReviewFormValues) => void
  onCancel: () => void
}

export function ReviewForm({ submitting, error, onSubmit, onCancel }: ReviewFormProps) {
  const [stars, setStars] = useState(0)
  const [comment, setComment] = useState('')
  const starsLabelId = useId()
  const commentId = useId()

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (stars === 0 || submitting) return
    onSubmit({ stars, comment: comment.trim() })
  }

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <h2 className={styles.title}>Your review</h2>

      <div className={styles.field}>
        <span id={starsLabelId} className={styles.label}>
          Rating
        </span>
        <div className={styles.stars} role="radiogroup" aria-labelledby={starsLabelId}>
          {STAR_VALUES.map((value) => (
            <button
              key={value}
              type="button"
              role="radio"
              aria-checked={value === stars}
              aria-label={`${value} de 5 estrellas`}
              className={`${styles.star} ${value <= stars ? styles.starActive : ''}`}
              onClick={() => setStars(value)}
            >
              <MdStar aria-hidden="true" />
            </button>
          ))}
        </div>
      </div>

      <div className={styles.field}>
        <label htmlFor={commentId} className={styles.label}>
          Comment (optional)
        </label>
        <textarea
          id={commentId}
          className={styles.textarea}
          rows={4}
          maxLength={MAX_COMMENT_LENGTH}
          value={comment}
          onChange={(event) => setComment(event.target.value)}
        />
        <span className={styles.counter}>
          {comment.length}/{MAX_COMMENT_LENGTH}
        </span>
      </div>

      {error && (
        <p role="alert" className={styles.error}>
          {error}
        </p>
      )}

      <div className={styles.actions}>
        <Button type="submit" disabled={stars === 0 || submitting}>
          {submitting ? 'Sending…' : 'Submit'}
        </Button>
        <Button variant="secondary" disabled={submitting} onClick={onCancel}>
          Cancel
        </Button>
      </div>
    </form>
  )
}
```

# `src/features/book-reviews/components/ReviewForm/ReviewForm.module.css`

## Código completo

```css
.form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  padding: var(--spacing-md);
  border-radius: var(--radius-md);
  background: var(--color-surface);
  box-shadow: var(--shadow-md);
}

.title {
  margin: 0;
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-semibold);
}

.field {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xs);
}

.label {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
}

.stars {
  display: flex;
  gap: var(--spacing-xs);
}

.star {
  display: grid;
  place-items: center;
  padding: var(--spacing-xs);
  border: 0;
  border-radius: var(--radius-sm);
  background: none;
  color: var(--color-disabled);
  font-size: 32px;
}

.starActive {
  color: var(--color-secondary);
}

.star:focus-visible,
.textarea:focus-visible {
  outline: 2px solid var(--color-primary);
  outline-offset: 2px;
}

.textarea {
  padding: var(--spacing-sm) var(--spacing-md);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-sm);
  background: var(--color-bg);
  font: inherit;
  font-size: var(--font-size-sm);
  resize: vertical;
}

.counter {
  align-self: flex-end;
  color: var(--color-text-muted);
  font-size: var(--font-size-xs);
}

.error {
  margin: 0;
  color: var(--color-error);
  font-size: var(--font-size-sm);
}

.actions {
  display: flex;
  flex-wrap: wrap;
  gap: var(--spacing-sm);
}
```

<!-- ===== 9. Hooks ===== -->

# SECCIÓN 9. Hooks

# `src/shared/hooks/useFetch.ts`

## Código completo

```ts
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
```

# `src/shared/hooks/useCategories.ts`

## Código completo

```ts
import { getCategories } from '../../api/endpoints/categories'
import { useFetch } from './useFetch'

const CATEGORIES_TTL = 10 * 60 * 1000

export function useCategories() {
  return useFetch('categories', (signal) => getCategories(signal), {
    ttl: CATEGORIES_TTL,
    persist: true,
  })
}
```

# `src/shared/hooks/useDebouncedValue.ts`

## Código completo

```ts
import { useEffect, useState } from 'react'

export function useDebouncedValue<T>(value: T, delay = 400) {
  const [debounced, setDebounced] = useState(value)

  useEffect(() => {
    const id = setTimeout(() => setDebounced(value), delay)
    return () => clearTimeout(id)
  }, [value, delay])

  return debounced
}
```

# `src/shared/hooks/useBookDetail.ts`

## Código completo

```ts
import { getBookById } from '../../api/endpoints/books'
import { useFetch } from './useFetch'

const BOOK_DETAIL_TTL = 5 * 60 * 1000 // 5 minutos, solo en memoria

export function useBookDetail(bookId: number, enabled = true) {
  return useFetch(`book:${bookId}`, (signal) => getBookById(bookId, signal), {
    enabled,
    ttl: BOOK_DETAIL_TTL,
  })
}
```

# `src/shared/hooks/useBookLoanStatus.ts`

## Código completo

```ts
import { getBookLoanStatus } from '../../api/endpoints/books'
import { useFetch } from './useFetch'
import { useSession } from './useSession'

// Sin cache: el estado cambia al prestar/devolver. La key incluye al usuario porque el
// resultado del endpoint depende del JWT (y así se recarga al iniciar o cerrar sesión).
// También sirve para saber si hay sesión: `data.userId !== null` (sin JWT llega `LoginRequired` y `userId: null`).
export function useBookLoanStatus(bookId: number, enabled = true) {
  const user = useSession()

  return useFetch(
    `loan-status:${bookId}:${user?.id ?? 'anon'}`,
    (signal) => getBookLoanStatus(bookId, signal),
    { enabled },
  )
}
```

# `src/shared/hooks/useGoBack.ts`

## Código completo

```ts
import { useCallback } from 'react'
import { useNavigate } from 'react-router'

/** Vuelve a la pantalla anterior; si se entró directo por URL (sin historial), va a `fallback`. */
export function useGoBack(fallback: string) {
  const navigate = useNavigate()

  return useCallback(() => {
    if (window.history.state?.idx > 0) navigate(-1)
    else navigate(fallback)
  }, [navigate, fallback])
}
```

# `src/shared/hooks/useSession.ts`

## Código completo

```ts
import { useSyncExternalStore } from 'react'
import { getStoredUser, subscribeSession } from '../../api/token'
import type { AuthUser } from '../../api/types/auth'

/** Usuario con sesión iniciada (o `null`). Se actualiza al hacer login, logout o refresh, también entre pestañas. */
export function useSession(): AuthUser | null {
  return useSyncExternalStore(subscribeSession, getStoredUser)
}
```

# `src/features/home/hooks.ts`

## Código completo

```ts
import { getTopRatedBooks } from '../../api/endpoints/books'
import { useFetch } from '../../shared/hooks/useFetch'

const TOP_RATED_TTL = 2 * 60 * 1000 // 2 minutos

export function useTopRatedBooks(categoryId?: number, enabled = true) {
  return useFetch(
    `top-rated:${categoryId ?? 'all'}`,
    (signal) => getTopRatedBooks(categoryId, signal),
    { enabled, ttl: TOP_RATED_TTL, persist: true },
  )
}
```

# `src/features/categories/hooks.ts`

## Código completo

```ts
import { searchBooks, type BookSearchParams } from '../../api/endpoints/books'
import { useFetch } from '../../shared/hooks/useFetch'

const SEARCH_TTL = 2 * 60 * 1000 // 2 minutos, solo en memoria

export function useBookSearch(params: BookSearchParams) {
  const key = `books:${params.search}|${params.categoryIds.join(',')}|${params.page}|${params.pageSize}`

  return useFetch(key, (signal) => searchBooks(params, signal), {
    ttl: SEARCH_TTL,
    keepPrevious: true,
  })
}
```

# `src/features/book-reviews/hooks.ts`

## Código completo

```ts
import { getBookRatings } from '../../api/endpoints/books'
import { useFetch } from '../../shared/hooks/useFetch'
import { RATINGS_PAGE_SIZE } from './constants'

const RATINGS_TTL = 60 * 1000 // 1 minuto, solo en memoria

// `version` se incrementa al publicar una reseña: cambia la key y fuerza una consulta nueva.
// `keepPrevious` deja la página anterior visible mientras llega la siguiente.
export function useBookRatings(bookId: number, page: number, version: number, enabled = true) {
  return useFetch(
    `ratings:${bookId}:${page}:${RATINGS_PAGE_SIZE}:v${version}`,
    (signal) => getBookRatings(bookId, { page, pageSize: RATINGS_PAGE_SIZE }, signal),
    { enabled, ttl: RATINGS_TTL, keepPrevious: true },
  )
}
```

# `src/features/profile/hooks.ts`

## Código completo

```ts
import { useCallback, useState } from 'react'
import { logout } from '../../api/endpoints/auth'
import { clearSession, getRefreshToken } from '../../api/token'

export function useLogout() {
  const [loggingOut, setLoggingOut] = useState(false)

  const handleLogout = useCallback(async () => {
    setLoggingOut(true)

    const refreshToken = getRefreshToken()
    try {
      if (refreshToken) await logout(refreshToken)
    } catch {
      // Sin red o con error del backend igual se cierra la sesión local
    } finally {
      clearSession() // useSession cambia a null y ProfilePage redirige a /login
    }
  }, [])

  return { loggingOut, logout: handleLogout }
}
```

<!-- ===== 10. Contextos / Providers ===== -->

# SECCIÓN 10. Contextos / Providers

_No existen contextos ni providers en este proyecto._

<!-- ===== 11. Servicios / API ===== -->

# SECCIÓN 11. Servicios / API

# `src/api/http.ts`

## Código completo

```ts
import { env } from '../config/env'
import {
  clearSession,
  getAccessToken,
  getRefreshToken,
  isAccessTokenExpired,
  saveSession,
} from './token'
import type { AuthResponse } from './types/auth'

const REFRESH_PATH = '/api/auth/refresh'

interface HttpOptions {
  /** Envía `Authorization: Bearer <JWT>`; si el token venció (o responde 401) lo renueva con el refresh token. */
  auth?: boolean
  method?: 'GET' | 'POST'
  /** Se envía como JSON (agrega `Content-Type: application/json`). */
  body?: unknown
}

interface SendOptions {
  method: 'GET' | 'POST'
  body?: unknown
  token: string | null
}

/**
 * Error de una respuesta no-2xx. `message` conserva el texto genérico de siempre;
 * `serverMessage` es el `detail` que manda el backend (null en 401/403, que vienen sin cuerpo).
 */
export class ApiError extends Error {
  status: number
  serverMessage: string | null

  constructor(status: number, path: string, serverMessage: string | null) {
    super(`Error ${status} al consultar ${path}`)
    this.name = 'ApiError'
    this.status = status
    this.serverMessage = serverMessage
  }
}

// El backend responde problem+json ({ detail }) o ValidationProblemDetails ({ errors })
async function readErrorDetail(res: Response): Promise<string | null> {
  try {
    const body: unknown = await res.json()
    if (typeof body !== 'object' || body === null) return null

    const { detail, errors, title } = body as Record<string, unknown>
    if (typeof detail === 'string') return detail

    if (typeof errors === 'object' && errors !== null) {
      const first = Object.values(errors).flat().find((m) => typeof m === 'string')
      if (typeof first === 'string') return first
    }

    return typeof title === 'string' ? title : null
  } catch {
    return null
  }
}

function send(path: string, signal: AbortSignal | undefined, { method, body, token }: SendOptions) {
  const headers: Record<string, string> = {}
  if (token) headers.Authorization = `Bearer ${token}`
  if (body !== undefined) headers['Content-Type'] = 'application/json'

  return fetch(`${env.apiUrl}${path}`, {
    method,
    signal,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })
}

// --- Renovación de sesión -------------------------------------------------------------
// El refresh token es de un solo uso: si se gasta dos veces el backend cierra TODAS las
// sesiones del usuario. Por eso hay una sola renovación en curso (las demás esperan su
// resultado) y un lock entre pestañas.

let refreshing: Promise<boolean> | null = null

function refreshTokens(): Promise<boolean> {
  refreshing ??= doRefresh().finally(() => {
    refreshing = null
  })
  return refreshing
}

async function doRefresh(): Promise<boolean> {
  const used = getRefreshToken()
  if (used === null) return false

  const run = async () => {
    const current = getRefreshToken()
    if (current === null) return false
    if (current !== used) return true // otra pestaña ya lo renovó mientras esperábamos el lock

    try {
      const res = await send(REFRESH_PATH, undefined, {
        method: 'POST',
        body: { refreshToken: current },
        token: null,
      })

      if (res.ok) {
        saveSession((await res.json()) as AuthResponse)
        return true
      }
      // 400/401: el refresh token ya no sirve (vencido, usado o revocado) → sesión terminada
      if (res.status === 400 || res.status === 401) clearSession()
      return false
    } catch {
      return false // sin red: no se cierra la sesión, se puede reintentar luego
    }
  }

  return navigator.locks ? navigator.locks.request('lectio:refresh', run) : run()
}

// Antes de una petición con auth: si el access token venció (o falta) y hay refresh token, renueva
async function getValidAccessToken(): Promise<string | null> {
  const token = getAccessToken()
  if (getRefreshToken() !== null && (token === null || isAccessTokenExpired())) {
    await refreshTokens()
    return getAccessToken()
  }
  return token
}

export async function http<T>(
  path: string,
  signal?: AbortSignal,
  { auth = false, method = 'GET', body }: HttpOptions = {},
): Promise<T> {
  let res = await send(path, signal, {
    method,
    body,
    token: auth ? await getValidAccessToken() : null,
  })

  // 401 con auth: renovar una vez y reintentar una vez
  if (res.status === 401 && auth && getRefreshToken() !== null && (await refreshTokens())) {
    res = await send(path, signal, { method, body, token: getAccessToken() })
  }

  if (!res.ok) throw new ApiError(res.status, path, await readErrorDetail(res))
  if (res.status === 204) return undefined as T // logout responde sin cuerpo
  return res.json() as Promise<T>
}
```

# `src/api/endpoints/loans.ts`

## Código completo

```ts
import { http } from '../http'
import type { PagedResponse } from '../types/common'
import type { UserLoan } from '../types/loan'

export interface CreateLoanBody {
  bookId: number
  checkoutDate: string
  dueDate: string
}

export interface CreateLoanResponse {
  message: string
  loanId: number
  bookId: number
  userId: number
  checkoutDate: string
  dueDate: string
}

export interface ReturnLoanResponse {
  message: string
  loanId: number
  bookId: number
  userId: number
  returnDate: string
}

export function createLoan(userId: number, body: CreateLoanBody, signal?: AbortSignal): Promise<CreateLoanResponse> {
  return http<CreateLoanResponse>(
    `/api/users/${userId}/loans`,
    signal,
    { auth: true, method: 'POST', body },
  )
}

export function returnLoan(userId: number, bookId: number, signal?: AbortSignal): Promise<ReturnLoanResponse> {
  return http<ReturnLoanResponse>(
    `/api/users/${userId}/loans/${bookId}/return`,
    signal,
    { auth: true, method: 'POST' },
  )
}

export interface GetUserLoansParams {
  page: number
  pageSize: number
}

export function getUserLoans(
  userId: number,
  { page, pageSize }: GetUserLoansParams,
  signal?: AbortSignal,
): Promise<PagedResponse<UserLoan>> {
  return http<PagedResponse<UserLoan>>(
    `/api/users/${userId}/loans?page=${page}&pageSize=${pageSize}`,
    signal,
    { auth: true },
  )
}
```

# `src/api/endpoints/books.ts`

## Código completo

```ts
import { http } from '../http'
import type { Book, BookDetail, BookSummary } from '../types/book'
import type { ApiResponse, PagedResponse } from '../types/common'
import type { BookLoanStatus } from '../types/loan'
import type { BookRatings } from '../types/rating'

const BOOKS_PATH = '/api/books'

export interface BookRatingsParams {
  page: number
  pageSize: number
}

export interface BookSearchParams {
  search: string
  categoryIds: number[]
  page: number
  pageSize: number
}

export async function getTopRatedBooks(categoryId?: number, signal?: AbortSignal) {
  const query = categoryId !== undefined ? `?categoryId=${categoryId}` : ''
  const res = await http<ApiResponse<Book[]>>(`${BOOKS_PATH}/top-rated${query}`, signal)
  return res.data
}

export function searchBooks(
  { search, categoryIds, page, pageSize }: BookSearchParams,
  signal?: AbortSignal,
) {
  const query = new URLSearchParams()
  if (search) query.set('search', search)
  categoryIds.forEach((id) => query.append('categoryIds', String(id)))
  query.set('page', String(page))
  query.set('pageSize', String(pageSize))

  return http<PagedResponse<BookSummary>>(`${BOOKS_PATH}?${query.toString()}`, signal)
}

// Detalle y estado de préstamo devuelven el objeto directo (sin `{ data }`)
export function getBookById(bookId: number, signal?: AbortSignal) {
  return http<BookDetail>(`${BOOKS_PATH}/${bookId}`, signal)
}

// No exige JWT, pero con token el backend devuelve el estado según el usuario
export function getBookLoanStatus(bookId: number, signal?: AbortSignal) {
  return http<BookLoanStatus>(`${BOOKS_PATH}/${bookId}/loan-status`, signal, { auth: true })
}

// Reseñas más recientes primero. No trae `pagination`: el total de páginas sale de `numberOfRatings`
export function getBookRatings(
  bookId: number,
  { page, pageSize }: BookRatingsParams,
  signal?: AbortSignal,
) {
  const query = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
  return http<BookRatings>(`${BOOKS_PATH}/${bookId}/ratings?${query.toString()}`, signal)
}
```

# `src/api/endpoints/categories.ts`

## Código completo

```ts
import { http } from '../http'
import type { ApiResponse } from '../types/common'
import type { Category } from '../types/category'

export async function getCategories(signal?: AbortSignal) {
  const res = await http<ApiResponse<Category[]>>('/api/categories', signal)
  return res.data
}
```

# `src/api/endpoints/ratings.ts`

## Código completo

```ts
import { http } from '../http'
import type { CreateRatingBody, RatingResponse } from '../types/rating'

const USERS_PATH = '/api/users'

// Requiere JWT y que {userId} sea el del token. Crea o actualiza (una reseña por usuario y libro);
// el backend rechaza con 400 si el usuario nunca prestó el libro.
export function createRating(userId: number, body: CreateRatingBody, signal?: AbortSignal) {
  return http<RatingResponse>(`${USERS_PATH}/${userId}/ratings`, signal, {
    auth: true,
    method: 'POST',
    body,
  })
}
```

# `src/api/endpoints/auth.ts`

## Código completo

```ts
import { http } from '../http'
import type { AuthResponse, LoginBody, RegisterBody } from '../types/auth'

const AUTH_PATH = '/api/auth'

// La renovación (POST /api/auth/refresh) la hace `http` por su cuenta; no se llama desde las pantallas.

export function login(body: LoginBody, signal?: AbortSignal) {
  return http<AuthResponse>(`${AUTH_PATH}/login`, signal, { method: 'POST', body })
}

export function register(body: RegisterBody, signal?: AbortSignal) {
  return http<AuthResponse>(`${AUTH_PATH}/register`, signal, { method: 'POST', body })
}

// 204 sin cuerpo; el access token ya emitido no se invalida en el servidor, hay que descartarlo en el cliente
export function logout(refreshToken: string, signal?: AbortSignal) {
  return http<void>(`${AUTH_PATH}/logout`, signal, { method: 'POST', body: { refreshToken } })
}
```

# `src/api/token.ts`

## Código completo

```ts
import type { AuthResponse, AuthUser } from './types/auth'

const ACCESS_TOKEN_KEY = 'lectio:accessToken'
const REFRESH_TOKEN_KEY = 'lectio:refreshToken'
const EXPIRES_AT_KEY = 'lectio:accessTokenExpiresAt'
const USER_KEY = 'lectio:user'

// Se refresca un poco antes de que el access token (15 min) venza de verdad
const EXPIRY_MARGIN_MS = 30 * 1000

const listeners = new Set<() => void>()

function read(key: string): string | null {
  try {
    return localStorage.getItem(key)
  } catch {
    return null
  }
}

function write(key: string, value: string | null) {
  try {
    if (value === null) localStorage.removeItem(key)
    else localStorage.setItem(key, value)
  } catch {
    // Sin localStorage la sesión simplemente no persiste
  }
}

function notify() {
  listeners.forEach((listener) => listener())
}

export function getAccessToken(): string | null {
  return read(ACCESS_TOKEN_KEY)
}

export function getRefreshToken(): string | null {
  return read(REFRESH_TOKEN_KEY)
}

// Un token sin fecha guardada (ej: pegado a mano en DevTools) se considera vigente
export function isAccessTokenExpired(): boolean {
  const expiresAt = Number(read(EXPIRES_AT_KEY))
  return expiresAt > 0 && Date.now() >= expiresAt - EXPIRY_MARGIN_MS
}

/** Guarda lo que devuelven login, register y refresh. El refresh token anterior queda reemplazado. */
export function saveSession(auth: AuthResponse) {
  write(ACCESS_TOKEN_KEY, auth.accessToken)
  write(REFRESH_TOKEN_KEY, auth.refreshToken)
  write(EXPIRES_AT_KEY, String(Date.now() + auth.expiresIn * 1000))
  write(USER_KEY, JSON.stringify(auth.user))
  notify()
}

export function clearSession() {
  write(ACCESS_TOKEN_KEY, null)
  write(REFRESH_TOKEN_KEY, null)
  write(EXPIRES_AT_KEY, null)
  write(USER_KEY, null)
  notify()
}

// useSyncExternalStore exige que el snapshot sea el mismo objeto mientras nada cambie
let cachedRaw: string | null = null
let cachedUser: AuthUser | null = null

export function getStoredUser(): AuthUser | null {
  const raw = read(USER_KEY)
  if (raw !== cachedRaw) {
    cachedRaw = raw
    cachedUser = parseUser(raw)
  }
  return cachedUser
}

function parseUser(raw: string | null): AuthUser | null {
  if (!raw) return null
  try {
    return JSON.parse(raw) as AuthUser
  } catch {
    return null
  }
}

/** Avisa de cambios de sesión en esta pestaña y en las demás (evento `storage`). */
export function subscribeSession(listener: () => void) {
  listeners.add(listener)
  window.addEventListener('storage', listener)

  return () => {
    listeners.delete(listener)
    window.removeEventListener('storage', listener)
  }
}
```

<!-- ===== 12. Tipos / Interfaces ===== -->

# SECCIÓN 12. Tipos / Interfaces

# `src/api/types/auth.ts`

## Código completo

```ts
export interface AuthUser {
  id: number
  fullName: string
  email: string
  phone: string | null
  role: 'user' | 'admin'
}

/** Respuesta de register, login y refresh (sin envoltorio `data`). `expiresIn` va en segundos. */
export interface AuthResponse {
  accessToken: string
  refreshToken: string
  tokenType: string
  expiresIn: number
  user: AuthUser
}

export interface LoginBody {
  email: string
  password: string
}

export interface RegisterBody {
  fullName: string
  email: string
  password: string
  phone: string | null
}
```

# `src/api/types/book.ts`

## Código completo

```ts
import type { Category } from './category'

export interface Author {
  id: number
  name: string
}

export interface BookSummary {
  id: number
  title: string
  imageUrl: string | null
  rating: { stars: number } | null
  authors: Author[]
  categories: Category[]
}

export interface Book extends BookSummary {
  description: string | null
}

export interface BookDetail {
  id: number
  title: string
  imageUrl: string | null
  description: string | null
  pageCount: number | null
  publishedYear: number | null
  publisher: string | null
  language: string | null
  authors: Author[]
  categories: Category[]
}
```

# `src/api/types/category.ts`

## Código completo

```ts
export interface Category {
  id: number
  name: string
}
```

# `src/api/types/common.ts`

## Código completo

```ts
export interface ApiResponse<T> {
  data: T
}

export interface Pagination {
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface PagedResponse<T> {
  data: T[]
  pagination: Pagination
}
```

# `src/api/types/loan.ts`

## Código completo

```ts
export type LoanStatus = 'LoginRequired' | 'Available' | 'BorrowedByYou' | 'Unavailable'

export interface BookLoanStatus {
  bookId: number
  userId: number | null
  status: LoanStatus
  loanId: number | null
  dueDate: string | null
}
```

# `src/api/types/rating.ts`

## Código completo

```ts
/** Una reseña dentro de GET /api/books/{id}/ratings. El backend no devuelve el nombre del autor, solo `idUser`. */
export interface BookRatingUser {
  idUser: number
  fullName: string
  comment: string | null
  rating: number
  createdAt: string
  updatedAt: string
}

/** Respuesta de GET /api/books/{id}/ratings (sin envoltorio `data` ni objeto `pagination`). */
export interface BookRatings {
  averageRating: number
  numberOfRatings: number
  users: BookRatingUser[]
}

export interface CreateRatingBody {
  bookId: number
  stars: number
  comment: string | null
}

/** Respuesta de POST /api/users/{userId}/ratings (200 tanto al crear como al actualizar). */
export interface RatingResponse {
  message: string
  ratingId: number
  bookId: number
  userId: number
  stars: number
  comment: string | null
  createdAt: string
  updatedAt: string
}
```

<!-- ===== 13. Utilidades y constantes ===== -->

# SECCIÓN 13. Utilidades y constantes

# `src/shared/utils/book.ts`

## Código completo

```ts
import type { Author } from '../../api/types/book'

export const toHttps = (url: string) => url.replace(/^http:\/\//, 'https://')

export function formatAuthors(authors: Author[]) {
  if (authors.length === 0) return 'Autor desconocido'
  const [first, ...others] = authors
  return others.length > 0 ? `${first.name} +${others.length}` : first.name
}
```

# `src/shared/utils/date.ts`

## Código completo

```ts
// Formats a date string (ISO 8601) as "Mon Day" in en-US, without the year.
// Examples: "Oct 10", "Jan 14".
// Returns an empty string if the value is null, undefined, or an invalid date.
export function formatShortDate(dateString: string | null | undefined): string {
  if (!dateString) return ''
  const date = new Date(dateString)
  if (isNaN(date.getTime())) return ''

  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
  })
}

// Returns the difference in calendar days (local time) between dueDate and today.
// Positive = days remaining; 0 = due today; negative = overdue.
export function getDaysUntilDue(dueDateString: string): number {
  const today = new Date()
  const due = new Date(dueDateString)

  // Compare by calendar date only (strip time)
  const todayDate = new Date(today.getFullYear(), today.getMonth(), today.getDate())
  const dueDate = new Date(due.getFullYear(), due.getMonth(), due.getDate())

  const diffMs = dueDate.getTime() - todayDate.getTime()
  return Math.round(diffMs / (1000 * 60 * 60 * 24))
}
```

# `src/shared/utils/cache.ts`

## Código completo

```ts
interface Entry<T> {
  data: T
  expiresAt: number
}

const STORAGE_PREFIX = 'lectio:cache:'

const memory = new Map<string, Entry<unknown>>()
const inflight = new Map<string, Promise<unknown>>()

function readEntry<T>(key: string, persist: boolean): Entry<T> | undefined {
  const hit = memory.get(key) as Entry<T> | undefined
  if (hit) return hit

  if (persist) {
    try {
      const raw = localStorage.getItem(STORAGE_PREFIX + key)
      if (raw) {
        const entry = JSON.parse(raw) as Entry<T>
        memory.set(key, entry)
        return entry
      }
    } catch {
      // localStorage no disponible o JSON corrupto: se ignora
    }
  }

  return undefined
}

/** Devuelve el dato solo si sigue vigente. */
export function readCache<T>(key: string, persist = false): T | undefined {
  const entry = readEntry<T>(key, persist)
  return entry && entry.expiresAt > Date.now() ? entry.data : undefined
}

/** Devuelve el dato aunque haya expirado (para cuando el backend no responde). */
export function readStale<T>(key: string, persist = false): T | undefined {
  return readEntry<T>(key, persist)?.data
}

export function writeCache<T>(key: string, data: T, ttl: number, persist = false) {
  const entry: Entry<T> = { data, expiresAt: Date.now() + ttl }
  memory.set(key, entry)

  if (persist) {
    try {
      localStorage.setItem(STORAGE_PREFIX + key, JSON.stringify(entry))
    } catch {
      // cuota llena o modo privado: queda solo en memoria
    }
  }
}

export function invalidateCache(key: string) {
  memory.delete(key)
  try {
    localStorage.removeItem(STORAGE_PREFIX + key)
  } catch {
    // nada que hacer
  }
}

/** Una sola petición en vuelo por key; al terminar guarda el resultado en cache. */
export function fetchCached<T>(
  key: string,
  fetcher: () => Promise<T>,
  ttl: number,
  persist = false,
): Promise<T> {
  const pending = inflight.get(key) as Promise<T> | undefined
  if (pending) return pending

  const promise = fetcher()
    .then((data) => {
      writeCache(key, data, ttl, persist)
      return data
    })
    .finally(() => inflight.delete(key))

  inflight.set(key, promise)
  return promise
}
```

# `src/shared/utils/validation.ts`

## Código completo

```ts
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

export function isValidEmail(value: string) {
  return EMAIL_PATTERN.test(value.trim())
}
```

# `src/shared/utils/apiError.ts`

## Código completo

```ts
import { ApiError } from '../../api/http'

// En 4xx el `detail` del backend es seguro de mostrar; en 5xx o sin red se usa el texto genérico
export function getApiErrorMessage(error: unknown, fallback: string) {
  if (error instanceof ApiError && error.status < 500 && error.serverMessage) {
    return error.serverMessage
  }
  return fallback
}
```

# `src/shared/utils/redirect.ts`

## Código completo

```ts
// Destino al que volver tras iniciar sesión (`state.from`). Solo rutas internas: evita redirecciones abiertas.
export function getRedirectTarget(state: unknown, fallback = '/'): string {
  if (typeof state === 'object' && state !== null && 'from' in state) {
    const { from } = state as { from: unknown }
    if (typeof from === 'string' && from.startsWith('/') && !from.startsWith('//')) return from
  }
  return fallback
}
```

# `src/features/home/constants.ts`

## Código completo

```ts
import type { IconType } from 'react-icons'
import { FaGhost, FaFingerprint, FaHourglassStart, FaRocket } from 'react-icons/fa'
import { IoSparkles } from "react-icons/io5";
import {
  MdAutoStories,
  MdBolt,
  MdBusinessCenter,
  MdChildCare,
  MdComputer,
  MdEditNote,
  MdFavorite,
  MdFlight,
  MdFormatQuote,
  MdLocalFireDepartment,
  MdMenuBook,
  MdPerson,
  MdPsychology,
  MdRestaurant,
  MdScience,
  MdSelfImprovement,
} from 'react-icons/md'

// Los nombres deben coincidir exactamente con los de /api/categories.
// Las primeras MOBILE_VISIBLE_CATEGORIES se ven en mobile; todas se ven en desktop.
export const FEATURED_CATEGORIES = [
  'Fiction',
  'Romance',
  'Technology',
  'History',
  'Fantasy',
  'Mystery',
  'Science',
  'Adventure',
]

export const MOBILE_VISIBLE_CATEGORIES = 4

export const DEFAULT_CATEGORY_ICON: IconType = MdMenuBook

// Etiquetas más cortas para los tiles (si no hay entrada, se usa el nombre del API)
export const CATEGORY_LABELS: Record<string, string> = {
  Technology: 'Tech',
}

export const CATEGORY_ICONS: Record<string, IconType> = {
  Fiction: MdAutoStories,
  Romance: MdFavorite,
  Technology: MdComputer,
  History: FaHourglassStart,
  Fantasy: IoSparkles,
  Horror: FaGhost,
  Mystery: FaFingerprint,
  Adventure: FaRocket,
  Science: MdScience,
  Biography: MdPerson,
  Poetry: MdFormatQuote,
  'Self-help': MdSelfImprovement,
  Essay: MdEditNote,
  Philosophy: MdPsychology,
  Comics: MdBolt,
  "Children's": MdChildCare,
  Travel: MdFlight,
  'Food & Cooking': MdRestaurant,
  Business: MdBusinessCenter,
  Popular: MdLocalFireDepartment,
}
```

# `src/features/book-detail/constants.ts`

## Código completo

```ts
export const BOOK_TABS = [
  { id: 'summary', label: 'Summary' },
  { id: 'basic-info', label: 'Basic Info' },
  { id: 'reviews', label: 'Reviews' },
] as const

export type BookTabId = (typeof BOOK_TABS)[number]['id']

export const getTabId = (id: BookTabId) => `book-tab-${id}`
export const getPanelId = (id: BookTabId) => `book-panel-${id}`

export const DEFAULT_DESCRIPTION =
  'Every book is a door waiting to be opened. This one has not shared its summary yet, but that is part of the magic: some stories are best discovered without a map. ' +
  'Somewhere between its first and last page there may be a place you have never been, a character who feels like an old friend, or a sentence you will want to read twice. ' +
  'Maybe it will make you laugh on a quiet afternoon, keep you awake past midnight, or change the way you see something you thought you already understood. ' +
  'Books have always been the most patient companions: they wait on the shelf until the right reader arrives, and they never rush anyone through a chapter. ' +
  'They do not mind if you read them slowly, if you pause to stare out of the window, or if you come back to them months later and find them quietly changed, because it was you who changed. ' +
  'That is the beauty of reading: every visit to the same pages can feel like meeting the story for the very first time. ' +
  'So take your time. Look at the cover, check the author and the details in Basic Info, and let your curiosity decide. ' +
  'If something about it calls to you, borrow it, find a comfortable spot, make yourself a warm drink, and turn the first page. ' +
  'The best part of any story is the moment you stop wondering what it is about and start living it.'
```

# `src/features/book-reviews/constants.ts`

## Código completo

```ts
export const RATINGS_PAGE_SIZE = 5
export const MAX_COMMENT_LENGTH = 1000 // límite del backend
export const STAR_VALUES = [1, 2, 3, 4, 5] as const
```

# `src/features/book-reviews/utils.ts`

## Código completo

```ts
import { ApiError } from '../../api/http'

// Fecha en UTC (dd-mm-aaaa): así `2025-06-14T00:00:00Z` no se corre al día anterior en zonas UTC-5
export function formatReviewDate(iso: string) {
  const date = new Date(iso)
  if (Number.isNaN(date.getTime())) return ''

  const day = String(date.getUTCDate()).padStart(2, '0')
  const month = String(date.getUTCMonth() + 1).padStart(2, '0')
  return `${day}-${month}-${date.getUTCFullYear()}`
}

// 401/403 llegan sin cuerpo; en 4xx el `detail` del backend es seguro de mostrar (en 5xx no)
export function getSubmitErrorMessage(error: unknown) {
  if (error instanceof ApiError) {
    if (error.status === 401 || error.status === 403) {
      return 'Your session has expired. Please log in again.'
    }
    if (error.status < 500 && error.serverMessage) return error.serverMessage
  }
  return 'Something went wrong while saving your review. Please try again.'
}
```

# `src/features/login/utils.ts`

## Código completo

```ts
import { isValidEmail } from '../../shared/utils/validation'

export interface LoginValues {
  email: string
  password: string
}

export type LoginErrors = Partial<Record<keyof LoginValues, string>>

export function validateLogin({ email, password }: LoginValues): LoginErrors {
  const errors: LoginErrors = {}

  if (!email.trim()) errors.email = 'Email is required.'
  else if (!isValidEmail(email)) errors.email = 'Enter a valid email address.'

  if (!password) errors.password = 'Password is required.'

  return errors
}
```

# `src/features/register/utils.ts`

## Código completo

```ts
import { isValidEmail } from '../../shared/utils/validation'

export interface RegisterValues {
  fullName: string
  email: string
  password: string
  phone: string
}

export type RegisterErrors = Partial<Record<keyof RegisterValues, string>>

// Mismas reglas que el backend: nombre 2-255, email válido, contraseña 8-72 (≤ 72 bytes) con letra y número
export function validateRegister({ fullName, email, password }: RegisterValues): RegisterErrors {
  const errors: RegisterErrors = {}

  const name = fullName.trim()
  if (name.length < 2) errors.fullName = 'Enter your full name (at least 2 characters).'
  else if (name.length > 255) errors.fullName = 'Name must be at most 255 characters.'

  if (!email.trim()) errors.email = 'Email is required.'
  else if (!isValidEmail(email)) errors.email = 'Enter a valid email address.'
  else if (email.trim().length > 255) errors.email = 'Email must be at most 255 characters.'

  if (password.length < 8) errors.password = 'Password must be at least 8 characters.'
  else if (password.length > 72 || new TextEncoder().encode(password).length > 72) {
    errors.password = 'Password must be at most 72 characters.'
  } else if (!/\p{L}/u.test(password) || !/\d/.test(password)) {
    errors.password = 'Password must include at least one letter and one number.'
  }

  return errors
}
```

# `src/features/profile/constants.ts`

## Código completo

```ts
// Opciones del menú del perfil: por ahora solo se muestran, todavía no llevan a ninguna pantalla
export const PROFILE_MENU = ['Booking History', 'My Info', 'Wishlist', 'Terms and Conditions'] as const
```

