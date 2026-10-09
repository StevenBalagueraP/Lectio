# FRONTEND_CONTEXT.md — Lectio Frontend

> Snapshot técnico del frontend **LectioFrontend** actualizado el 2026-10-07 a partir de `LectioFrontend.zip` (archivos modificados hasta 2026-10-07) y contrastado con `FRONTEND_FILES.md`. Reemplaza al snapshot del 2026-10-04.
> **Regla principal:** el código real del repositorio es la fuente de verdad. Todo lo de este documento fue contrastado contra el código; lo que no pudo verificarse está marcado `[UNKNOWN / NEEDS VERIFICATION]` y las deducciones están marcadas **(inferencia)**.
> El archivo `.env` existe pero **no se incluye su valor**; solo se documenta el nombre de la variable.
> `FRONTEND_FILES.md` es el complemento con el código; este archivo explica el porqué y las reglas.

## Cambios respecto al snapshot anterior (2026-10-04)

**Features nuevas o completadas**
- `book-detail` pasó de stub a pantalla funcional: portada (`BookHero`), pestañas Summary / Basic Info, texto expandible y botón de préstamo que refleja el estado (`LoanButton`).
- `book-reviews` (nueva): promedio, lista paginada de reseñas y formulario para crear o actualizar la reseña propia (ruta `/books/:bookId/reviews`).
- `login` (nueva, **provisional**): página `/login` con aviso "coming soon". No hay login real.

**Componentes compartidos nuevos** (`src/shared/components/`): `Button`, `BookHero` (movido desde `book-detail`), `Skeleton`. `RatingStars` ganó la prop `size`, `Navbar` la prop `overlay`.

**Hooks nuevos**: `useBookDetail`, `useBookLoanStatus`, `useGoBack` (en `src/shared/hooks/`) y `useBookRatings` (en `src/features/book-reviews/hooks.ts`).

**Componentes de feature nuevos**: `book-detail` → `BookTabs`, `BookInfoList`, `ExpandableText`, `LoanButton`; `book-reviews` → `RatingSummary`, `ReviewCard`, `ReviewList`, `ReviewForm`.

**Capa api**: `http` ahora soporta POST, cuerpo JSON y JWT opcional, y lanza `ApiError`; nuevos `api/token.ts`, `endpoints/ratings.ts`, tipos `loan.ts` y `rating.ts`; `books.ts` suma `getBookById`, `getBookLoanStatus`, `getBookRatings`.

**Eliminados**: `src/App.tsx` y `src/App.css` (eran código muerto). `LoanButton.module.css` (ahora usa `Button`). **Movidos**: `BookHero` y los hooks de detalle pasaron a `shared/`.

**Otros**: se retiraron casi todos los comentarios del código (quedan 25 líneas de comentario); `index.css` y los archivos de configuración no cambiaron.

## ⚠️ Avisos críticos antes de leer

1. **`README.md` está desactualizado / es aspiracional.** Describe `auth/`, `dashboard/`, `products/`, `AuthContext`, `ProtectedRoute`, `PublicRoute`, `client.ts`, `useLocalStorage.ts`, `formatters.ts`, etc. **Nada de eso existe en el código.** No lo uses como referencia de estructura.
2. **Código muerto confirmado (verificado con grep de imports):** solo `src/features/home/components/BookList/` (nadie lo importa; lo reemplazó `shared/components/BookGrid`). Además `invalidateCache` (exportado en `cache.ts`) no tiene usos y `writeCache` solo se usa dentro de `cache.ts`.
3. **No hay login real, ni roles, ni rutas protegidas.** `src/api/token.ts` solo **lee** `localStorage['lectio:accessToken']`; **nada en el código escribe ese token**. `/login` es una página provisional. `Navbar` muestra un `userName` por defecto `'User'` sin ninguna fuente de datos. La "sesión" se infiere de `useBookLoanStatus().data.userId !== null`.
4. **`HistoryPage` es un stub** (solo un `<h1>`). En el detalle, **Borrow y Deliver todavía no ejecutan nada**: `LoanButton` muestra el estado, pero `BookDetailPage` solo reacciona a `LoginRequired` (navega a `/login`).
5. **Estado de calidad verificado sobre este snapshot:** `tsc -b` → exit 0; `eslint .` → exit 0; `vite build` → OK (177 módulos; JS 368.96 kB (116.83 kB gzip); CSS 28.90 kB (5.21 kB gzip)). No existe ningún test ni runner de tests.
6. **Contrato del backend:** lo que no se pueda ver en el código del frontend (reglas de negocio, validaciones, stack) está marcado `[UNKNOWN / NEEDS VERIFICATION]`. Contrastar con el contrato del backend (`BACKEND_API_CONTEXT.md`, si está disponible) antes de asumir comportamientos.

---

# 1. FRONTEND OVERVIEW

**Qué es:** *Lectio* es una SPA de catálogo de libros que consume una API REST. Permite ver los libros mejor valorados, filtrarlos por categoría desde la home, buscar por título + filtrar por varias categorías + paginar en la página de categorías, abrir el detalle de un libro (resumen, datos básicos, estado de préstamo) y leer o publicar reseñas.

**Problema que resuelve (inferencia):** descubrir y explorar libros de un catálogo, y opinar sobre ellos. El préstamo (Borrow/Deliver), el login real, el historial y el perfil **aún no están implementados en el front**.

**Framework:** React 19 + TypeScript 6 sobre Vite 8, ruteo con React Router 8 (data router), estilos con CSS Modules + variables CSS globales en `index.css`, íconos con `react-icons`.

**Backend:** API REST en `VITE_API_URL` (por defecto `http://localhost:5031`, ver `src/config/env.ts`). Conviven tres formas de respuesta: `{ data: ... }` (top-rated, categorías), `{ data: [...], pagination: {...} }` (búsqueda) y **objeto directo sin envoltorio** (detalle, loan-status, ratings). Stack del backend: `[UNKNOWN / NEEDS VERIFICATION]` (no se puede saber desde el frontend).

**Arquitectura general (resumen):**
- `src/api/` → capa de acceso a la API (cliente `http` con `ApiError`, lectura del token, endpoints, tipos de DTO).
- `src/features/<feature>/` → pantallas y componentes propios de cada feature (`pages/`, `components/`, y según la feature `hooks.ts`, `constants.ts`, `utils.ts`).
- `src/shared/` → componentes, hooks y utilidades reutilizables entre features.
- `src/routes/` → definición del router y el layout.
- `src/config/` → variables de entorno tipadas.

**Features existentes:**

| Feature | Estado | Qué hace |
|---|---|---|
| `home` | **Completa (funcional)** | Hero carousel de libros top-rated, atajos de categorías (filtran el grid), grid de libros |
| `categories` | **Completa (funcional)** | Búsqueda por título (debounce), filtro multi-categoría, paginación, estado en la URL |
| `book-detail` | **Funcional (parcial)** | Portada con botón volver, pestañas Summary / Basic Info (Reviews navega a otra ruta), texto expandible, botón de préstamo según estado. Borrow/Deliver sin acción |
| `book-reviews` | **Funcional** | Promedio y contador, lista paginada (5 por página), formulario de estrellas + comentario que hace POST con JWT |
| `login` | **Provisional** | Página con aviso "Login is coming soon"; destino de los botones que exigen sesión |
| Layout / navegación | **Funcional** | Navbar desktop + drawer móvil (`MobileMenu`); Navbar en modo overlay sobre la portada del detalle |
| `history` | **Stub** | Ruta `/history` que solo imprime "History" |
| `profile` | **No existe** | Ítem de navegación deshabilitado ("Soon") sin ruta |

**Partes pendientes (hechos):** login/registro reales y guardado del token, acciones Borrow y Deliver, historial, perfil, nombre/avatar reales en el Navbar, logo real (TODO en Navbar), favicon real, página 404 y `errorElement`, tests.

---

# 2. TECHNOLOGY STACK

Versiones: "declarada" = `package.json`; "instalada" = verificada en `node_modules` del zip (solo las marcadas).

```text
Technology: React
Version: declarada ^19.2.8 — instalada 19.3.0
Purpose in this project: UI. Solo componentes de función + hooks.
Where it is used: todo src/. StrictMode activo en main.tsx.
```
```text
Technology: React DOM
Version: declarada ^19.2.8 — instalada 19.3.0
Purpose in this project: montaje (createRoot) en main.tsx.
Where it is used: src/main.tsx
```
```text
Technology: react-router
Version: declarada ^8.4.0 — instalada 8.4.0
Purpose in this project: data router (createBrowserRouter), layout con Outlet, NavLink/Link, useNavigate, useParams, useSearchParams, useMatch. Se importa del paquete "react-router" (no "react-router-dom"); RouterProvider se importa de "react-router/dom".
Where it is used: main.tsx, routes/index.tsx, routes/AppLayout.tsx (useMatch), Navbar, MobileMenu, HomePage, CategoriesPage, BookDetailPage, BookReviewsPage, LoginPage, shared/hooks/useGoBack
```
```text
Technology: react-icons
Version: declarada ^5.7.0 — instalada 5.7.0
Purpose in this project: todos los íconos. Sets usados: "react-icons/md" (mayoría), "react-icons/fa" y "react-icons/io5" (solo en features/home/constants.ts). Tipo IconType de "react-icons".
Where it is used: BookCard, RatingStars, Pagination, Navbar, MobileMenu, navItems, BookHero, BookInfoList, ReviewCard, ReviewForm, BookReviewsPage, CategoryShortcuts, CategoryFilter, CategoriesPage, home/constants.ts
```
```text
Technology: Vite (+ @vitejs/plugin-react)
Version: vite declarada ^8.3.0 — instalada 8.3.1; plugin-react ^6.1.1
Purpose in this project: dev server y build. vite.config.ts solo registra el plugin react(). Sin alias de rutas, sin proxy.
Where it is used: vite.config.ts, scripts dev/build/preview
```
```text
Technology: TypeScript
Version: declarada ~6.0.2 — instalada 6.0.3
Purpose in this project: tipado. El modo estricto está ACTIVO por defecto (verificado: un archivo de prueba con `any` implícito y `null` asignado a string falla en tsc) aunque tsconfig.app.json no declare "strict".
Where it is used: todo src/. Opciones relevantes en tsconfig.app.json: verbatimModuleSyntax, erasableSyntaxOnly, noUnusedLocals, noUnusedParameters, noFallthroughCasesInSwitch, jsx react-jsx, moduleResolution bundler, target es2023.
```
```text
Technology: ESLint (flat config) + typescript-eslint + eslint-plugin-react-hooks + eslint-plugin-react-refresh
Version: eslint ^10.10.0, typescript-eslint ^8.69.0, react-hooks ^7.1.1, react-refresh ^0.5.6, @eslint/js ^10.0.1, globals ^17.12.0
Purpose in this project: lint de **/*.{ts,tsx} con js recommended + tseslint recommended + reactHooks flat recommended + reactRefresh.vite. Ignora dist.
Where it is used: eslint.config.js, script "lint"
```
```text
Technology: CSS Modules + CSS custom properties (vanilla CSS)
Version: n/a
Purpose in this project: sistema de estilos. Un archivo Nombre.module.css por componente/página; tokens globales en src/index.css.
Where it is used: 25 archivos *.module.css + src/index.css
```
```text
Technology: Google Fonts — Poppins (400/500/600/700, display=swap)
Version: n/a
Purpose in this project: tipografía global. Se carga con <link> en index.html (no con @import en CSS).
Where it is used: index.html, variable --font-family-base
```
```text
Technology: Browser APIs
Purpose: fetch + AbortController (api/http.ts, useFetch), localStorage (shared/utils/cache.ts y lectura del JWT en api/token.ts), URLSearchParams (api/endpoints/books.ts, CategoriesPage), CSS scroll-snap (HeroCarousel), ResizeObserver (ExpandableText), window.history.state (useGoBack), window.scrollTo (CategoriesPage, BookReviewsPage), <form>/<textarea> nativos (ReviewForm)
```

**NO existen en el proyecto (verificado en package.json y código):** Tailwind, Redux, Zustand, React Query/TanStack, Axios, Context API propio, librerías de formularios (react-hook-form, formik), librerías UI (MUI, shadcn), Prettier (sin config), Vitest/Jest/Testing Library, i18n, alias de paths (`@/`), Storybook.

# 3. COMPLETE APPLICATION TREE

Árbol real (excluye `node_modules`, `package-lock.json`, `.env`). `*.module.css` junto a cada componente. No hay `.gitkeep`, `public/` ni `src/vite-env.d.ts` en el zip.

```text
LectioFrontend/
├── .env    ← NO INCLUIDO (solo se cita VITE_API_URL)
├── .gitignore
├── eslint.config.js
├── index.html    ← usa /favicon.svg (no está en el zip)
├── package-lock.json
├── package.json
├── README.md    ← obsoleto
├── tsconfig.app.json
├── tsconfig.json
├── tsconfig.node.json
├── vite.config.ts
└── src/
    ├── api/
    │   ├── endpoints/
    │   │   ├── books.ts
    │   │   ├── categories.ts
    │   │   └── ratings.ts
    │   ├── types/
    │   │   ├── book.ts
    │   │   ├── category.ts
    │   │   ├── common.ts
    │   │   ├── loan.ts
    │   │   └── rating.ts
    │   ├── http.ts    ← fetch GET/POST + ApiError
    │   └── token.ts    ← lee el JWT de localStorage
    ├── config/
    │   └── env.ts    ← env.apiUrl
    ├── features/
    │   ├── book-detail/
    │   │   ├── components/
    │   │   │   ├── BookInfoList/
    │   │   │   │   ├── BookInfoList.module.css
    │   │   │   │   └── BookInfoList.tsx
    │   │   │   ├── BookTabs/
    │   │   │   │   ├── BookTabs.module.css
    │   │   │   │   └── BookTabs.tsx
    │   │   │   ├── ExpandableText/
    │   │   │   │   ├── ExpandableText.module.css
    │   │   │   │   └── ExpandableText.tsx
    │   │   │   └── LoanButton/
    │   │   │       └── LoanButton.tsx
    │   │   ├── pages/
    │   │   │   ├── BookDetailPage.module.css
    │   │   │   └── BookDetailPage.tsx
    │   │   └── constants.ts
    │   ├── book-reviews/
    │   │   ├── components/
    │   │   │   ├── RatingSummary/
    │   │   │   │   ├── RatingSummary.module.css
    │   │   │   │   └── RatingSummary.tsx
    │   │   │   ├── ReviewCard/
    │   │   │   │   ├── ReviewCard.module.css
    │   │   │   │   └── ReviewCard.tsx
    │   │   │   ├── ReviewForm/
    │   │   │   │   ├── ReviewForm.module.css
    │   │   │   │   └── ReviewForm.tsx
    │   │   │   └── ReviewList/
    │   │   │       ├── ReviewList.module.css
    │   │   │       └── ReviewList.tsx
    │   │   ├── pages/
    │   │   │   ├── BookReviewsPage.module.css
    │   │   │   └── BookReviewsPage.tsx
    │   │   ├── constants.ts
    │   │   ├── hooks.ts    ← useBookRatings
    │   │   └── utils.ts
    │   ├── categories/
    │   │   ├── components/
    │   │   │   └── CategoryFilter/
    │   │   │       ├── CategoryFilter.module.css
    │   │   │       └── CategoryFilter.tsx
    │   │   ├── pages/
    │   │   │   ├── CategoriesPage.module.css
    │   │   │   └── CategoriesPage.tsx
    │   │   └── hooks.ts
    │   ├── history/
    │   │   └── pages/
    │   │       └── HistoryPage.tsx    ← STUB
    │   ├── home/
    │   │   ├── components/
    │   │   │   ├── BookList/
    │   │   │   │   ├── BookList.module.css    ← CÓDIGO MUERTO
    │   │   │   │   └── BookList.tsx    ← CÓDIGO MUERTO
    │   │   │   ├── CategoryShortcuts/
    │   │   │   │   ├── CategoryShortcuts.module.css
    │   │   │   │   └── CategoryShortcuts.tsx
    │   │   │   └── HeroCarousel/
    │   │   │       ├── HeroCarousel.module.css
    │   │   │       └── HeroCarousel.tsx
    │   │   ├── pages/
    │   │   │   ├── HomePage.module.css
    │   │   │   └── HomePage.tsx
    │   │   ├── constants.ts
    │   │   └── hooks.ts
    │   └── login/
    │       └── pages/
    │           ├── LoginPage.module.css
    │           └── LoginPage.tsx    ← provisional
    ├── routes/
    │   ├── AppLayout.tsx    ← Navbar + Outlet (overlay en /books/:id)
    │   └── index.tsx    ← router (6 rutas)
    ├── shared/
    │   ├── components/
    │   │   ├── BookCard/
    │   │   │   ├── BookCard.module.css
    │   │   │   └── BookCard.tsx
    │   │   ├── BookGrid/
    │   │   │   ├── BookGrid.module.css
    │   │   │   └── BookGrid.tsx
    │   │   ├── BookHero/
    │   │   │   ├── BookHero.module.css
    │   │   │   └── BookHero.tsx    ← portada (detalle + reseñas)
    │   │   ├── Button/
    │   │   │   ├── Button.module.css
    │   │   │   └── Button.tsx    ← botón primary/secondary
    │   │   ├── MobileMenu/
    │   │   │   ├── MobileMenu.module.css
    │   │   │   └── MobileMenu.tsx
    │   │   ├── Navbar/
    │   │   │   ├── Navbar.module.css
    │   │   │   ├── Navbar.tsx
    │   │   │   └── navItems.ts
    │   │   ├── Pagination/
    │   │   │   ├── Pagination.module.css
    │   │   │   └── Pagination.tsx
    │   │   ├── RatingStars/
    │   │   │   ├── RatingStars.module.css
    │   │   │   └── RatingStars.tsx
    │   │   └── Skeleton/
    │   │       ├── Skeleton.module.css
    │   │       └── Skeleton.tsx    ← placeholder de carga
    │   ├── hooks/
    │   │   ├── useBookDetail.ts    ← detalle (cache 5 min)
    │   │   ├── useBookLoanStatus.ts    ← préstamo (sin cache, JWT)
    │   │   ├── useCategories.ts
    │   │   ├── useDebouncedValue.ts
    │   │   ├── useFetch.ts
    │   │   └── useGoBack.ts    ← volver con fallback
    │   └── utils/
    │       ├── book.ts
    │       └── cache.ts
    ├── index.css    ← tokens + reset global
    └── main.tsx    ← entry point
```

Conteo en `src/`: **30 `.tsx`**, **27 `.ts`**, **26 `.css`** (25 módulos + `index.css`), **4 184 líneas** de código en total (`.ts`, `.tsx`, `.css`).

**Responsabilidad de cada carpeta:**

| Carpeta | Responsabilidad |
|---|---|
| `src/api/` | Todo lo que habla con el backend. `http.ts` = único wrapper de `fetch` (GET/POST + `ApiError`); `token.ts` = lectura del JWT; `endpoints/` = una función por llamada; `types/` = DTOs |
| `src/config/` | Variables de entorno tipadas (`env.apiUrl`) |
| `src/routes/` | Router (`createBrowserRouter`) y `AppLayout` |
| `src/shared/components/` | Componentes reutilizables entre pantallas (una carpeta por componente): `BookCard`, `BookGrid`, `BookHero`, `Button`, `MobileMenu`, `Navbar`, `Pagination`, `RatingStars`, `Skeleton` |
| `src/shared/hooks/` | Hooks reutilizables, **un archivo por hook** (`useFetch`, `useCategories`, `useDebouncedValue`, `useBookDetail`, `useBookLoanStatus`, `useGoBack`) |
| `src/shared/utils/` | Funciones puras/infra (formato de libros, cache) |
| `src/features/<x>/` | Todo lo específico de una pantalla/feature |
| `src/features/<x>/pages/` | Componente de página (el que monta el router) |
| `src/features/<x>/components/` | Componentes usados solo dentro de esa feature |
| `src/features/<x>/hooks.ts` | Hooks de datos de la feature (archivo único; en `home`, `categories`, `book-reviews`) |
| `src/features/<x>/constants.ts` | Constantes de la feature (`home`, `book-detail`, `book-reviews`) |
| `src/features/<x>/utils.ts` | Helpers de la feature (solo `book-reviews`) |

---

# 4. ARCHITECTURE

**Arquitectura real:** *feature-based* + carpeta `shared/` + capa `api/` separada. No hay context, ni store global, ni capa de "services" con clases: la capa de servicio son funciones sueltas en `api/endpoints/`.

Flujo de datos real (Home):

```text
router (routes/index.tsx)
 ↓
AppLayout (Navbar + <Outlet/>)
 ↓
Page  (features/home/pages/HomePage.tsx)           ← orquesta estado local y navegación
 ↓
Feature hook (features/home/hooks.ts: useTopRatedBooks)
   o shared hook (shared/hooks/useCategories.ts)
 ↓
useFetch (shared/hooks/useFetch.ts)                ← cache + abort + estados
 ↓
shared/utils/cache.ts (Map en memoria + localStorage opcional)
 ↓
api/endpoints/books.ts | categories.ts
 ↓
api/http.ts (fetch)
 ↓
Backend (VITE_API_URL)
```

Flujo (Categories): `CategoriesPage` lee/escribe estado en la URL (`useSearchParams`) → `useBookSearch` (`features/categories/hooks.ts`) → `useFetch` → `searchBooks` → `http`.

Flujo (Detalle y Reseñas, lectura): `BookDetailPage` / `BookReviewsPage` → `useBookDetail` + `useBookLoanStatus` (shared) y, en reseñas, `useBookRatings` (feature) → `useFetch` → `getBookById` / `getBookLoanStatus` / `getBookRatings` → `http`. `getBookLoanStatus` pide `auth: true`, de modo que `http` agrega `Authorization: Bearer <token>` **si** `api/token.ts` encuentra un token.

Flujo (Reseñas, escritura): `ReviewForm` (estado local) → `BookReviewsPage.handleSubmit` → `createRating` (`api/endpoints/ratings.ts`) → `http` (POST + JWT) → en error `ApiError` → `getSubmitErrorMessage` (`book-reviews/utils.ts`) → se muestra en el formulario. En éxito: se cierra el formulario, se muestra `res.message`, `page = 1` y `version + 1` (cambia la key de `useBookRatings` y se recarga lista y promedio). **No hay hook de mutación**: la página llama a `createRating` directamente y maneja `submitting` / `submitError` / `notice` con `useState`.

**Reglas de relación observadas (verificadas por script: sin imports de `shared/` → `features/`, ni entre features):**
- Las **pages** son quienes llaman hooks de datos y `useNavigate`; los componentes reciben datos y callbacks por props (`onOpen`, `onSelect`, `onChange`, `onBack`, `onSubmit`, `onAction`). Los componentes presentacionales **no** navegan ni hacen fetch.
- `shared/` **no importa** de `features/`. Las features importan de `shared/` y de `api/`. `api/` no importa de ninguna otra capa (salvo `config/`).
- Las features **no se importan entre sí**. Cuando dos features necesitan lo mismo, se sube a `shared/`: así pasó con `BookHero`, `useBookDetail`, `useBookLoanStatus` y `useGoBack` (detalle + reseñas) y con `Button`.
- Los hooks de datos que usan 2+ features viven en `shared/hooks` (`useCategories`, `useBookDetail`, `useBookLoanStatus`); los de una sola feature, en `features/<x>/hooks.ts`.
- Los tipos de DTO viven en `api/types/` y se importan desde cualquier capa.
- `AppLayout` solo compone `Navbar` + `Outlet` (y decide el modo `overlay` del Navbar); cada página renderiza su propio `<main>`.

---

# 5. INDEX.CSS — ARCHIVO `src/index.css` (análisis completo)

Archivo de 4 189 bytes. Contiene: 1 bloque `:root` con todos los tokens, 1 `@media (min-width: 1024px)` que redefine los tamaños de fuente, y 4 reglas globales (`*`, `body`, `button`, `img`). **No hay**: dark mode, `html`, selectores de heading/link/input, scrollbar, selection, animaciones globales ni utility classes. El código completo está en la sección 28.

## Colors (todas las variables, valores reales)

```text
Variable: --color-primary            Value: #236A34   Purpose: verde de marca (botones principales, tabs/links activos, texto destacado)
  Where it should be used: botones primarios, estado activo/seleccionado, acentos de texto (.accent), outline de foco
Variable: --color-primary-hover      Value: #1B4F27   Purpose: :hover de elementos primarios
  Where: hover de botones primarios (Button, HeroCarousel, BookCard, ExpandableText). (Nota: Navbar.logo y BookHero lo usan como fondo de placeholder)
Variable: --color-primary-light      Value: #E3F0E6   Purpose: fondo verde suave
  Where: hover de páginas en Pagination y de Button secondary, ítem activo de MobileMenu, tile de categoría seleccionado, avatar fallback, avatar de ReviewCard
Variable: --color-secondary          Value: #F3F02B   Purpose: amarillo de estrellas de rating
  Where: relleno de RatingStars y estrella activa del selector de ReviewForm
Variable: --color-secondary-hover    Value: #D6D323   Purpose: hover del amarillo   → SIN USO actual
Variable: --color-bg                 Value: #FFFFFF   Purpose: fondo general (body, navbar, inputs, drawer, panel)
Variable: --color-surface            Value: #F9FAFE   Purpose: fondo de cards/elementos elevados
  Where: BookCard, hover de ítems de lista (MobileMenu, CategoryFilter), RatingSummary (pill de estrellas), ReviewCard, ReviewForm
Variable: --color-border             Value: #747474   Purpose: bordes de inputs y líneas
  Where: input de búsqueda, trigger y panel de CategoryFilter, textarea de ReviewForm, botón volver de BookHero
Variable: --color-text               Value: #000000   Purpose: texto principal
Variable: --color-text-muted         Value: #6B7280   Purpose: texto secundario
  Where: mensajes de estado vacío/error, contador de resultados, ellipsis de paginación, ítem deshabilitado
Variable: --color-disabled           Value: #E0E0E0   Purpose: fondo de elementos inactivos
  Where: skeletons (Skeleton y copias), placeholders de portada, estrellas vacías y estrellas inactivas de ReviewForm, dots del carrusel, tiles de categoría sin seleccionar, flecha de paginación deshabilitada, Button deshabilitado, badge "Soon"
Variable: --color-success            Value: #236A34   Purpose: estado positivo   → USADO: aviso de éxito de BookReviewsPage (`.notice`)
Variable: --color-error              Value: #FF1226   Purpose: estado negativo   → USADO: errores de BookReviewsPage (`.error`) y ReviewForm (`.error`)
Variable: --color-warning            Value: #F59E0B   Purpose: advertencias futuras   → SIN USO actual
Variable: --color-info               Value: #3B82F6   Purpose: informativo futuro   → SIN USO actual
Variable: --color-text-inverse       Value: #FFFFFF   Purpose: texto blanco sobre fondo oscuro/imagen/verde
Variable: --color-text-inverse-muted Value: #E5E7EB   Purpose: texto secundario sobre imágenes   → SIN USO actual
Variable: --overlay-dark             Value: rgba(0, 0, 0, 0.55)   Purpose: capa oscura sobre imágenes
  Where: captions de BookCard/HeroCarousel, shade del hero, fondo del overlay de MobileMenu
```

> Los comentarios dentro de `index.css` mencionan pantallas que **no existen** en esta app ("My Library", tab "Returned", badge "Active/Expired", "lugar"): el archivo parece derivado de otro diseño. Los **valores** son los vigentes; los textos de "Purpose" originales deben leerse como orientativos.

## IMPORTANT COLOR RULE

> **Los colores existentes definidos en `index.css` son la fuente de verdad para el sistema visual de la aplicación.**

El siguiente Claude debe:
- reutilizar las variables existentes y las clases/patrones existentes;
- **evitar colores hardcodeados**: nada de `#ffffff`, `#000000`, `rgb(...)`, `rgba(...)` si ya existe un token equivalente (`--color-bg`, `--color-text`, `--color-text-inverse`, `--overlay-dark`, …);
- no inventar colores nuevos: si hace falta uno, **agregarlo como variable en `:root` de `index.css`** y usarlo vía `var(--...)`;
- mantener consistencia visual y usar tokens existentes antes de crear nuevos;
- los tokens de estado ya existen: `--color-error` y `--color-success` ya los usan `BookReviewsPage` y `ReviewForm` (seguir ese patrón para errores y avisos de éxito); `--color-warning` y `--color-info` siguen sin uso. **Úsalos** en lugar de crear rojos/verdes nuevos.

**Excepciones existentes en el código (documentadas, no imitar):**
- `HeroCarousel.module.css` → `.slide { background: #111 }` y `.media { background: #111 }` (fondo detrás de la portada difuminada). No existe token equivalente. Si se toca, preferir crear un token (p. ej. `--color-bg-dark`) en `index.css`.
- `HeroCarousel.module.css` → `.cover { border-radius: 4px }` (valor literal, no token; el radio más pequeño del sistema es `--radius-sm: 8px`).

# 6. TYPOGRAPHY

| Aspecto | Valor real |
|---|---|
| Familia | `--font-family-base: 'Poppins', sans-serif` → aplicada en `body` |
| Carga | `<link>` a Google Fonts en `index.html`: `Poppins:wght@400;500;600;700&display=swap` (con `preconnect`) |
| Pesos | `--font-weight-regular: 400` (usado en `BookTabs .tab` y `ReviewCard .comment`), `-medium: 500`, `-semibold: 600`, `-bold: 700` |
| Line-height | **No hay token.** Valores sueltos: `1.3` (`HomePage .title`), `1.5` (`HeroCarousel .description`, `ExpandableText .text`), `1` (`RatingStars`, botón de `BookCard`, `RatingSummary .average`) |
| Headings | **No hay estilos globales para h1–h6**: cada módulo CSS estiliza el suyo (`margin: 0`, tamaño/peso por tokens) |
| Botones | `button { font: inherit; cursor: pointer }` global; tamaños/pesos por módulo |
| Inputs | Sin regla global; `CategoriesPage .input` usa `font: inherit; font-size: var(--font-size-md)` |
| Links | Sin regla global; `Navbar` y `MobileMenu` ponen `text-decoration: none` y color por token |

**Escala de tamaños (mobile-first; override a ≥1024px dentro de `index.css`):**

| Token | Base (mobile) | ≥1024px | Uso observado |
|---|---|---|---|
| `--font-size-xs` | 13px | 14px | captions, título sobre portada, contador "books found", badge "Soon", nombre de usuario (mobile) |
| `--font-size-sm` | 15px | 16px | labels, autor, mensajes, tabs, estrellas, botón del hero, ítems de lista |
| `--font-size-md` | 17px | 20px | links del navbar, input de búsqueda, trigger del filtro, ítems del drawer, título del hero de detalle, etiqueta de `RatingSummary`, nombre en `ReviewCard`, título de `ReviewForm` |
| `--font-size-lg` | 20px | 24px | título del drawer, título del libro en hero desktop, estrellas `size="lg"`, título de la página de reseñas (desktop) |
| `--font-size-xl` | 24px | 32px | `kicker`/`title` de HomePage |
| `--font-size-2xl` | 28px | 40px | **SIN USO** |
| `--font-size-3xl` | 36px | 48px | promedio de `RatingSummary` (`.average`) — único uso |

Tamaños literales en px usados fuera de los tokens (principalmente ícono/controles): `18px, 22px, 24px, 28px, 32px, 40px, 64px` (íconos y controles), ver sección 28 por archivo. `font-size: clamp(...)` solo en `CategoryShortcuts`.

# 7. GLOBAL CSS

Todo lo global está en `src/index.css` (nada más se importa globalmente; `App.css` fue eliminado):

| Grupo | Regla real | Para qué sirve |
|---|---|---|
| CSS variables | `:root { ... }` (42 tokens) | Sistema visual (ver secciones 5, 6, 8) |
| Responsive tokens | `@media (min-width: 1024px) { :root { --font-size-* } }` | Escala tipográfica mayor en desktop |
| Reset de caja | `*, *::before, *::after { box-sizing: border-box }` | Modelo de caja uniforme |
| Body | `margin: 0; font-family: var(--font-family-base); color: var(--color-text); background: var(--color-bg)` | Base visual |
| Botones | `button { font: inherit; cursor: pointer }` | Botones heredan tipografía |
| Imágenes | `img { display: block; max-width: 100% }` | Evita gap inline y desbordes |

**No existen:** estilos de `html`/`#root`, scrollbar global, `::selection`, links, inputs, headings, animaciones globales, transiciones globales, clases utilitarias, dark mode, `prefers-reduced-motion`.

**Breakpoints reales en toda la app:** `@media (min-width: 768px)` (5 usos) y `@media (min-width: 1024px)` (14 usos + el de `index.css`). Siempre `min-width` → **mobile-first**.

**Animaciones locales (no globales):** `@keyframes pulse { 50% { opacity: .5 } }` existe en **4 módulos**: `Skeleton` (la versión compartida), `BookGrid`, `BookList` (muerto) y `HeroCarousel` (CSS Modules aísla el nombre). Transiciones: `BookGrid .grid` (`opacity .15s ease`), `MobileMenu` overlay/drawer (`.25s ease`).

# 8. DESIGN SYSTEM

### Colors

| Token | Value | Purpose |
|---|---|---|
| `--color-primary` | `#236A34` | Marca / acción primaria / activo |
| `--color-primary-hover` | `#1B4F27` | Hover primario |
| `--color-primary-light` | `#E3F0E6` | Fondo verde suave (hover, seleccionado) |
| `--color-secondary` | `#F3F02B` | Estrellas de rating |
| `--color-secondary-hover` | `#D6D323` | (sin uso) |
| `--color-bg` | `#FFFFFF` | Fondo base |
| `--color-surface` | `#F9FAFE` | Cards, hover de listas |
| `--color-border` | `#747474` | Bordes de inputs/paneles |
| `--color-text` | `#000000` | Texto principal |
| `--color-text-muted` | `#6B7280` | Texto secundario |
| `--color-disabled` | `#E0E0E0` | Inactivo / skeleton / vacío |
| `--color-success` | `#236A34` | Aviso de éxito (`BookReviewsPage .notice`) |
| `--color-error` | `#FF1226` | Errores (`BookReviewsPage .error`, `ReviewForm .error`) |
| `--color-warning` | `#F59E0B` | (sin uso) |
| `--color-info` | `#3B82F6` | (sin uso) |
| `--color-text-inverse` | `#FFFFFF` | Texto sobre oscuro/verde |
| `--color-text-inverse-muted` | `#E5E7EB` | (sin uso) |
| `--overlay-dark` | `rgba(0,0,0,0.55)` | Capa sobre imágenes |

### Typography

| Element | Font | Size | Weight |
|---|---|---|---|
| Body | Poppins | (hereda; sin tamaño base global) | 400 (por defecto del navegador) |
| Título de sección (HomePage `.title`) | Poppins | `--font-size-xl` | 700 (`bold`), line-height 1.3 |
| Kicker (HomePage `.kicker`) | Poppins | `sm` mobile → `xl` desktop | 600 mobile → 700 desktop |
| Título de libro en hero (desktop) | Poppins | `--font-size-lg` | 700 |
| Autor en card | Poppins | `--font-size-sm` | 600 |
| Título sobre portada (card) | Poppins | `--font-size-xs` | 500 |
| Links navbar / ítems drawer | Poppins | `--font-size-md` | 500 (activo del drawer: 600) |
| Botones pill (hero, "See More") | Poppins | `--font-size-sm` / `--font-size-xs` | 600 |
| Label de formulario | Poppins | `--font-size-sm` | 500 |
| Mensajes vacío/error | Poppins | `--font-size-sm` | 400, color muted |
| Promedio de reseñas (`RatingSummary .average`) | Poppins | `--font-size-3xl` | 700, line-height 1 |
| Botón compartido (`Button`) | Poppins | `--font-size-sm` | 600 |
| Texto expandible (`ExpandableText`) | Poppins | `--font-size-sm` | 400, line-height 1.5, justificado |

### Spacing (tokens)

| Token | Value |
|---|---|
| `--spacing-xs` | 4px |
| `--spacing-sm` | 8px |
| `--spacing-md` | 16px (el más usado) |
| `--spacing-lg` | 24px |
| `--spacing-xl` | 32px |

Convenciones: gap de grids `--spacing-md` (mobile) / `--spacing-lg` (desktop ≥1024); padding de contenedor de página `--spacing-md` horizontal; en desktop `clamp(var(--spacing-xl), 4vw, 80px)` (patrón **repetido** en Navbar, HomePage, CategoriesPage, BookDetailPage y BookReviewsPage). `2px` y `calc(var(--spacing-md) + 32px)` aparecen como valores sueltos.

### Border radius

| Token | Value | Uso |
|---|---|---|
| `--radius-sm` | 8px | inputs internos, textarea, badges, portada, botón "open" móvil, pill de estrellas de `RatingSummary` |
| `--radius-md` | 12px | cards (incl. `ReviewCard`, `ReviewForm`), carrusel, panel, `BookHero` (desktop) |
| `--radius-lg` | 16px | input de búsqueda y trigger del filtro |
| `--radius-full` | 999px | botones pill (`Button`), dots, avatares (Navbar, `ReviewCard`), páginas, botón volver de `BookHero` |

### Shadows

| Token | Value | Uso |
|---|---|---|
| `--shadow-sm` | `0 1px 2px rgba(0,0,0,.05)` | rating pill de BookCard, botón volver de BookHero |
| `--shadow-md` | `0 4px 6px rgba(0,0,0,.1)` | BookCard, botones del hero, `Button` primario, `RatingSummary`, `ReviewCard`, `ReviewForm` |
| `--shadow-lg` | `0 10px 15px rgba(0,0,0,.1)` | drawer, panel del filtro, portada del hero, `BookHero` (desktop) |

### Transitions

`opacity .15s ease` (BookGrid stale), `opacity/visibility/transform .25s ease` (MobileMenu), `pulse 1.2s ease-in-out infinite` (skeletons). No hay token de transición.

### Breakpoints

`768px` (grid 2→4 columnas; alturas de hero) y `1024px` (cambio mobile→desktop). No hay tokens CSS de breakpoint (no se pueden usar variables en media queries): se escriben literales.

### Other visual conventions

- Cards: `--color-surface` + `--radius-md` + `--shadow-md` + padding `--spacing-sm`.
- Botón primario pill: **componente `Button`** (`variant="primary"`): `--color-primary`, texto `--color-text-inverse`, `--radius-full`, `min-height: 44px`, hover `--color-primary-hover`, deshabilitado `--color-disabled` + texto `--color-text-muted` + `cursor: not-allowed` + sin sombra. `variant="secondary"` = borde y texto `--color-primary` sobre `--color-bg` (hover `--color-primary-light`). `block` limita el ancho a `min(100%, 260px)`. `BookCard .open` y `HeroCarousel .action` siguen con su propio botón.
- Seleccionado/activo: o bien `--color-primary-light` + texto/borde `--color-primary` (tiles, drawer) o `--color-primary` + texto inverso (CategoryFilter, página actual).
- Skeleton: componente compartido `Skeleton` (`--color-disabled` + animación `pulse`); `BookGrid`, `BookList` y `HeroCarousel` conservan copias propias.
- Foco visible: `:focus-visible` con outline `2px solid var(--color-primary)` en 9 reglas de 8 módulos (Button, BookHero, BookTabs, ExpandableText, ReviewForm, BookReviewsPage, CategoryFilter, CategoriesPage). **No hay estilo global de foco.**
- z-index: `-1` (`::before` del degradado del Navbar overlay), `1` (botón volver de BookHero), `10` (Navbar overlay), `20` (panel del filtro), `100` (overlay del drawer), `101` (drawer).
- Contenedor móvil/tablet: `max-width: 960px; margin: 0 auto`; desktop ≥1024: ancho completo con padding `clamp`. Detalle y reseñas: en ≥1024 grid `minmax(0,4fr) minmax(0,7fr)` (portada a la izquierda, contenido a la derecha).

# 9. SHARED COMPONENTS

Ubicación: `src/shared/components/`. Todos son **named exports** y reciben props tipadas con una `interface XxxProps` local (no exportada). Hay **9 componentes** (`BookCard`, `BookGrid`, `BookHero`, `Button`, `MobileMenu`, `Navbar`, `Pagination`, `RatingStars`, `Skeleton`) más el helper `NAV_ITEMS`.

```text
Component: BookCard
Path: src/shared/components/BookCard/BookCard.tsx (+ BookCard.module.css)
Purpose: Tarjeta de libro: portada (o ícono placeholder), título superpuesto, estrellas, autor y botón "ver detalle".
Props: book, onOpen
Required props: book: BookSummary
Optional props: onOpen?: (bookId: number) => void
Variants: ninguna por prop. Responsive: mobile = botón compacto con flecha (MdChevronRight); ≥1024px = botón pill ancho con texto "See More" y portada 3/4 (mobile 2/3).
State: ninguno (stateless)
Events: onOpen(book.id) al click del botón
Dependencies: RatingStars, utils/book (formatAuthors, toHttps), react-icons/md (MdChevronRight, MdMenuBook), tipo BookSummary
Used by: BookGrid (shared), BookList (feature home, DEAD)
```
```text
Component: BookGrid
Path: src/shared/components/BookGrid/BookGrid.tsx (+ .module.css)
Purpose: Grilla de BookCard que además resuelve los estados loading / error / vacío / "stale".
Props: books, loading, error, emptyMessage, skeletonCount, onOpen
Required props: loading: boolean
Optional props: books?: BookSummary[]; error?: string; emptyMessage?: string (default 'No hay libros todavía.'); skeletonCount?: number (default 4); onOpen?: (bookId:number)=>void
Variants: estados — error → <p role="alert">; loading sin libros → skeletons con aria-busy; sin libros → mensaje; con libros → grilla (si loading=true se atenúa: clase .stale opacity .5 + pointer-events none)
State: ninguno
Events: reenvía onOpen a cada BookCard
Dependencies: BookCard, tipo BookSummary
Used by: HomePage, CategoriesPage
```
```text
Component: Pagination
Path: src/shared/components/Pagination/Pagination.tsx (+ .module.css)
Purpose: Paginador numérico con flechas y elipsis; siempre muestra primera y última página.
Props: page, totalPages, onChange
Required props: page: number (1-based), totalPages: number, onChange: (page:number)=>void
Optional props: ninguna
Variants: página actual (.current), flechas deshabilitadas en extremos; devuelve null si totalPages <= 1
State: ninguno (función pura getPageItems local: ≤7 páginas muestra todas; si no, ventanas con elipsis)
Events: onChange(n)
Dependencies: react-icons/md (MdChevronLeft, MdChevronRight)
Used by: CategoriesPage, BookReviewsPage
```
```text
Component: RatingStars
Path: src/shared/components/RatingStars/RatingStars.tsx (+ .module.css)
Purpose: Estrellas de 0 a 5 con relleno proporcional (soporta decimales) superponiendo 5 estrellas grises y 5 amarillas recortadas por ancho.
Props: value, size
Required props: value: number (se limita a [0,5])
Optional props: size?: 'sm' | 'lg' (default 'sm'; 'lg' usa --font-size-lg)
Variants: sm / lg
State: ninguno
Events: ninguno
Dependencies: react-icons/md (MdStar)
Used by: BookCard, HeroCarousel, RatingSummary (lg), ReviewCard (lg)
```
```text
Component: Navbar
Path: src/shared/components/Navbar/Navbar.tsx (+ Navbar.module.css, navItems.ts)
Purpose: Cabecera: botón hamburguesa (mobile), marca "Lectio", links de navegación (desktop) y bloque de usuario (nombre + avatar o inicial).
Props: userName, avatarUrl, overlay
Required props: ninguna
Optional props: userName?: string (default 'User'); avatarUrl?: string; overlay?: boolean (default false)
Variants: avatar con imagen o con inicial; links ocultos en mobile; overlay=true → header absoluto y transparente con degradado oscuro sobre la portada (mobile); en ≥1024px vuelve a fondo blanco normal
State: menuOpen (useState); closeMenu memoizado con useCallback
Events: abre MobileMenu; NavLink marca activo con clase .linkActive
Dependencies: MobileMenu, NAV_ITEMS, react-router (Link, NavLink), react-icons/md (MdMenu)
Used by: AppLayout
Nota: el logo es un <span> placeholder con un TODO para reemplazar por <img>. AppLayout solo le pasa `overlay` (true en /books/:bookId); nadie pasa userName/avatarUrl, por lo que siempre muestra "User".
```
```text
Component: MobileMenu
Path: src/shared/components/MobileMenu/MobileMenu.tsx (+ .module.css)
Purpose: Drawer lateral de navegación para <1024px con overlay.
Props: open, onClose
Required props: open: boolean, onClose: () => void
Optional props: ninguna
Variants: ítems normales (NavLink) y deshabilitados (span con badge "Soon")
State: ref al botón de cerrar (useRef)
Events: Escape cierra; click en overlay cierra; click en ítem cierra. Al abrir: foco al botón cerrar y body.style.overflow='hidden' (se restaura al cerrar/desmontar)
Dependencies: NAV_ITEMS (de ../Navbar/navItems), react-router NavLink, react-icons/md (MdClose)
Used by: Navbar
```
```text
Component: Skeleton
Path: src/shared/components/Skeleton/Skeleton.tsx (+ Skeleton.module.css)
Purpose: Bloque placeholder gris pulsante para estados de carga; el tamaño lo pone el consumidor.
Props: className
Required props: ninguna
Optional props: className?: string (default '')
Variants: ninguna
State: ninguno
Events: ninguno
Dependencies: ninguna
Used by: BookDetailPage, BookReviewsPage
Nota: aria-hidden="true". BookGrid, BookList y HeroCarousel todavía tienen su propia copia de `.skeleton` + `pulse`.
```
```text
Component: Button
Path: src/shared/components/Button/Button.tsx (+ Button.module.css)
Purpose: Botón pill compartido de la app (acción primaria o secundaria).
Props: variant, block, y todos los atributos nativos de <button> excepto className (disabled, onClick, type, aria-*, children…)
Required props: ninguna
Optional props: variant?: 'primary' | 'secondary' (default 'primary'); block?: boolean (default false → ancho `min(100%, 260px)`); type (default 'button')
Variants: primary (relleno verde + sombra), secondary (borde verde sobre blanco), disabled (gris), block
State: ninguno (stateless)
Events: los de <button> (onClick, onSubmit vía type="submit" dentro de un form)
Dependencies: ninguna (solo CSS Module)
Used by: LoanButton, BookReviewsPage ("Add Review"), ReviewForm (Submit y Cancel)
Nota: `className` está excluido del tipo → no se pueden añadir clases desde fuera. Si falta una variante, agregarla al propio componente.
```
```text
Component: BookHero
Path: src/shared/components/BookHero/BookHero.tsx (+ BookHero.module.css)
Purpose: Portada grande de un libro con botón "volver" y franja inferior con título y categoría.
Props: title, category, imageUrl, onBack, headingLevel
Required props: title: string; imageUrl: string | null; onBack: () => void
Optional props: category?: string; headingLevel?: 1 | 2 (default 1)
Variants: con imagen (toHttps) o placeholder (ícono MdMenuBook); título como <h1> o <h2>; layout mobile (300px/360px de alto) y desktop (aspect-ratio 3/4, radio y sombra)
State: ninguno (stateless)
Events: onBack() al pulsar el botón (aria-label "Volver"); no navega por sí mismo
Dependencies: utils/book (toHttps), react-icons/md (MdChevronLeft, MdMenuBook)
Used by: BookDetailPage (h1), BookReviewsPage (h2, dentro de .hero que solo se ve en desktop)
Nota: estaba en features/book-detail/components/ y se movió a shared al reutilizarse en reseñas.
```
```text
Helper (no es componente): NAV_ITEMS / NavItem
Path: src/shared/components/Navbar/navItems.ts
Purpose: lista única de ítems de navegación {label,to,icon,mobileOnly?,disabled?} compartida por Navbar y MobileMenu.
```

---

# 10. SHARED COMPONENT RELATIONSHIPS

| Component | Shared? | Used by | Purpose |
|---|---|---|---|
| `BookCard` | **Sí (de facto vía BookGrid)** | BookGrid → HomePage, CategoriesPage (+ BookList muerto) | Card de libro |
| `BookGrid` | **Sí (2 features)** | HomePage, CategoriesPage | Grilla + loading/error/empty |
| `RatingStars` | **Sí (4 consumidores, 3 features)** | BookCard, HeroCarousel, RatingSummary, ReviewCard | Estrellas (`size` sm/lg) |
| `Button` | **Sí (2 features)** | LoanButton (book-detail), BookReviewsPage y ReviewForm (book-reviews) | Botón pill primary/secondary |
| `BookHero` | **Sí (2 features)** | BookDetailPage, BookReviewsPage | Portada con botón volver y título |
| `Skeleton` | **Sí (2 features)** | BookDetailPage, BookReviewsPage | Placeholder de carga |
| `Pagination` | **Sí (2 features)** | CategoriesPage, BookReviewsPage | Paginador |
| `Navbar` | Global (layout) | AppLayout | Cabecera (con modo `overlay`) |
| `MobileMenu` | Solo de Navbar | Navbar | Drawer (shared por ubicación, uso exclusivo de Navbar) |
| `NAV_ITEMS` | Compartido entre Navbar y MobileMenu | Navbar, MobileMenu | Fuente única de navegación |
| `HeroCarousel` | **Exclusivo de feature home** | HomePage | Hero de libros top-rated |
| `CategoryShortcuts` | **Exclusivo de feature home** | HomePage | Tiles de categorías destacadas |
| `CategoryFilter` | **Exclusivo de feature categories** | CategoriesPage | Dropdown multi-selección |
| `BookTabs`, `BookInfoList`, `ExpandableText`, `LoanButton` | **Exclusivos de feature book-detail** | BookDetailPage | Pestañas, datos básicos, texto expandible, botón de préstamo |
| `RatingSummary`, `ReviewCard`, `ReviewList`, `ReviewForm` | **Exclusivos de feature book-reviews** | BookReviewsPage (`ReviewCard` vía `ReviewList`) | Promedio, tarjeta, lista, formulario |
| `BookList` | **Exclusivo de home y SIN USO** | nadie | Duplicado antiguo de BookGrid |

- **Globales:** Navbar (vía AppLayout).
- **Compartidos entre features:** BookGrid, BookCard, RatingStars, Button, BookHero, Skeleton, Pagination (+ hooks `useCategories`, `useFetch`, `useBookDetail`, `useBookLoanStatus`, `useGoBack`).
- **Exclusivos de una feature:** HeroCarousel, CategoryShortcuts, CategoryFilter, los 4 de book-detail y los 4 de book-reviews.
- **Candidatos a unificar (duplicación actual):** los skeletons de `BookGrid`/`BookList`/`HeroCarousel` (copias de `.skeleton` + `pulse` que `Skeleton` ya cubre); los botones pill de `BookCard .open` y `HeroCarousel .action` (`Button` aún no los reemplaza); `HeroCarousel` duplica la lógica de portada difuminada (`bg`+`shade`+`cover`) en dos ramas; los inputs con foco (`CategoriesPage .input`, `CategoryFilter .trigger`, `ReviewForm .textarea`) comparten estilo pero no componente; el layout de dos columnas (grid 4fr/7fr) de `BookDetailPage` y `BookReviewsPage` está repetido en ambos CSS.

---

# 11. FEATURES

```text
Feature: home
Purpose: Pantalla principal: hero de libros top-rated, filtro rápido por categoría y grilla de libros.
Location: src/features/home/

Components: HeroCarousel, CategoryShortcuts, (BookList — sin uso)
Hooks: useTopRatedBooks (hooks.ts) + shared useCategories
Services: api/endpoints/books.getTopRatedBooks, api/endpoints/categories.getCategories
Types: Book, Category (de api/types)
Pages: HomePage (ruta "/")
State: selectedCategoryId (useState en HomePage)
API calls: GET /api/books/top-rated, GET /api/books/top-rated?categoryId=N, GET /api/categories
Dependencies: shared/BookGrid, shared hooks, react-router (useNavigate), constants.ts (FEATURED_CATEGORIES, CATEGORY_ICONS…)

Cómo funciona: HomePage llama useTopRatedBooks() (sin categoría) para el hero y como lista por defecto; si el usuario elige una categoría en CategoryShortcuts, activa un segundo useTopRatedBooks(categoryId) y la grilla muestra ese resultado. Click de nuevo sobre la categoría activa quita el filtro. "See more" navega a /categories. Abrir un libro navega a /books/:id. Los nombres de FEATURED_CATEGORIES deben coincidir EXACTAMENTE con los nombres que devuelve /api/categories (si no coinciden, el tile no aparece).
```
```text
Feature: categories
Purpose: Explorar el catálogo completo: búsqueda por título, filtro multi-categoría, paginación.
Location: src/features/categories/

Components: CategoryFilter
Hooks: useBookSearch (hooks.ts) + shared useCategories, useDebouncedValue
Services: api/endpoints/books.searchBooks, categories.getCategories
Types: BookSummary, PagedResponse, BookSearchParams, Category
Pages: CategoriesPage (ruta "/categories")
State: URL (?q=&categories=&page=) + estado local `text` del input
API calls: GET /api/books?search=&categoryIds=..&page=&pageSize=10, GET /api/categories
Dependencies: shared/BookGrid, shared/Pagination, react-router (useSearchParams, useNavigate)

Cómo funciona: el estado de búsqueda vive en la URL. El input mantiene `text` local; con debounce de 400 ms escribe `q` en la URL (replace) y reinicia page=1. CategoryFilter cambia `categories` (coma-separadas, ids únicos y ordenados, enteros >0) y reinicia page=1. Cambiar de página usa push (no replace) y hace scroll al top. PAGE_SIZE=10 (constante en la página). Muestra "N books found".
```
```text
Feature: book-detail
Purpose: Detalle de un libro: portada, pestañas Summary / Basic Info, botón de préstamo.
Location: src/features/book-detail/

Components: BookTabs, BookInfoList, ExpandableText, LoanButton (+ shared BookHero, Skeleton, Button)
Hooks: shared useBookDetail, useBookLoanStatus, useGoBack (la feature no tiene hooks.ts propio)
Services: api/endpoints/books.getBookById, books.getBookLoanStatus
Types: BookDetail, BookLoanStatus, LoanStatus
Pages: BookDetailPage (ruta "/books/:bookId")
State: tab (useState, 'summary' | 'basic-info')
API calls: GET /api/books/{id}; GET /api/books/{id}/loan-status (con JWT si hay token)
Constants: BOOK_TABS, getTabId, getPanelId, DEFAULT_DESCRIPTION (constants.ts)
Dependencies: shared/BookHero, Skeleton, Button; react-router (useNavigate, useParams, Link)

Cómo funciona: la página valida el id de la URL (entero > 0) y carga detalle y estado de préstamo. Mientras carga el detalle muestra skeletons; si no hay datos (id inválido, 404, error de red) muestra un mensaje de error con link a "/". El resumen sale de `description` (o de DEFAULT_DESCRIPTION si viene vacío) y se recorta a 14 líneas con "See more". La pestaña "Reviews" no muestra panel: navega a /books/:id/reviews. El botón de préstamo traduce el estado del backend (LoginRequired / Available / BorrowedByYou / Unavailable); hoy solo "Log in to borrow" tiene efecto (va a /login). El Navbar se superpone a la portada en esta ruta (AppLayout).
```
```text
Feature: book-reviews
Purpose: Ver el promedio y las reseñas de un libro, y publicar o actualizar la propia.
Location: src/features/book-reviews/

Components: RatingSummary, ReviewCard, ReviewList, ReviewForm (+ shared BookHero, Button, Pagination, Skeleton, RatingStars)
Hooks: useBookRatings (hooks.ts) + shared useBookDetail, useBookLoanStatus, useGoBack
Services: api/endpoints/books.getBookRatings, api/endpoints/ratings.createRating
Types: BookRatings, BookRatingUser, CreateRatingBody, RatingResponse
Pages: BookReviewsPage (ruta "/books/:bookId/reviews")
State: page, version, formOpen, submitting, submitError, notice (useState en la página); estado del formulario (stars, comment) dentro de ReviewForm
API calls: GET /api/books/{id}; GET /api/books/{id}/loan-status; GET /api/books/{id}/ratings?page=&pageSize=5; POST /api/users/{userId}/ratings (JWT)
Constants/Utils: RATINGS_PAGE_SIZE=5, MAX_COMMENT_LENGTH=1000, STAR_VALUES (constants.ts); formatReviewDate, getSubmitErrorMessage (utils.ts)
Dependencies: shared/*, api/http (ApiError), react-router

Cómo funciona: el estado de la página (número de página, formulario abierto, envío) es local, no está en la URL. La sesión se deduce de `loan.data.userId` (null = sin sesión). "Add Review" sin sesión lleva a /login; con sesión abre ReviewForm. Al enviar se hace POST con `{ bookId, stars, comment|null }`; si sale bien se cierra el formulario, se muestra el `message` del backend, se vuelve a la página 1 y se incrementa `version` para que `useBookRatings` consulte de nuevo (lista y promedio). Si falla, el mensaje de `getSubmitErrorMessage` aparece dentro del formulario. El total de páginas se calcula con `ceil(numberOfRatings / 5)` porque la respuesta no incluye `pagination`. En desktop el libro (BookHero) se ve a la izquierda; en mobile solo hay un botón volver en la cabecera.
```
```text
Feature: login
Purpose: Página provisional para los flujos que exigen sesión. NO es un login real.
Location: src/features/login/pages/
Pages: LoginPage (ruta "/login") — título "Log in", mensaje "Login is coming soon…" y link "Back to home".
API calls: ninguna. Sin formulario, sin estado, sin escritura de token.
Quién navega aquí: BookDetailPage (botón "Log in to borrow") y BookReviewsPage ("Add Review" sin sesión).
```
```text
Feature: history
Purpose: Historial. STUB.
Location: src/features/history/pages/HistoryPage.tsx
Pages: HistoryPage (ruta "/history") — solo <h1>History</h1> (sin <main>).
```

---

# 12. PAGES / ROUTES

Router: `createBrowserRouter` en `src/routes/index.tsx`, montado con `RouterProvider` (`react-router/dom`) en `main.tsx`. **6 rutas** bajo un layout común.

```text
Route: /
Page: HomePage
File: src/features/home/pages/HomePage.tsx
Purpose: Home (hero + categorías + grilla)
Authentication required: No (no existe auth)
Role required: Ninguno
Main components: HeroCarousel, CategoryShortcuts, BookGrid
Data required: categorías, libros top-rated (global y por categoría)
API calls: GET /api/books/top-rated[?categoryId], GET /api/categories
```
```text
Route: /history
Page: HistoryPage — src/features/history/pages/HistoryPage.tsx
Purpose: Historial (stub: <h1>History</h1>)
Authentication required: No · Role: ninguno · API calls: ninguna
```
```text
Route: /categories
Page: CategoriesPage — src/features/categories/pages/CategoriesPage.tsx
Purpose: Búsqueda + filtro + paginación
Authentication required: No · Role: ninguno
Query params: q (texto), categories (ids separados por coma), page (>1; se omite si es 1)
Main components: CategoryFilter, BookGrid, Pagination
API calls: GET /api/books?..., GET /api/categories
```
```text
Route: /books/:bookId
Page: BookDetailPage
File: src/features/book-detail/pages/BookDetailPage.tsx
Purpose: Detalle del libro (portada, pestañas, botón de préstamo)
Authentication required: No para ver el libro. El estado de préstamo usa el JWT si existe (sin él llega 'LoginRequired')
Role required: Ninguno
Params: bookId (entero > 0; si no, se muestra el estado de error)
Main components: BookHero, BookTabs, ExpandableText, BookInfoList, LoanButton, Skeleton
Data required: detalle del libro, estado de préstamo
API calls: GET /api/books/{id}, GET /api/books/{id}/loan-status
Layout: el Navbar va en modo overlay (AppLayout)
```
```text
Route: /books/:bookId/reviews
Page: BookReviewsPage
File: src/features/book-reviews/pages/BookReviewsPage.tsx
Purpose: Promedio, lista paginada y formulario de reseñas
Authentication required: No para leer. Publicar exige sesión (JWT) y userId (de loan-status); sin sesión el botón lleva a /login
Role required: Ninguno
Params: bookId (entero > 0)
Main components: BookHero (h2), RatingSummary, Button, ReviewForm, ReviewList → ReviewCard, Pagination, Skeleton
Data required: detalle del libro, estado de préstamo (para userId), reseñas (página actual)
API calls: GET /api/books/{id}, GET /api/books/{id}/loan-status, GET /api/books/{id}/ratings?page=&pageSize=5, POST /api/users/{userId}/ratings
Estado de paginación: local (useState), NO en la URL
```
```text
Route: /login
Page: LoginPage — src/features/login/pages/LoginPage.tsx
Purpose: Página provisional ("Login is coming soon")
Authentication required: No · Role: ninguno · API calls: ninguna
```

**Otros aspectos del ruteo (hechos):**
- **Layout:** una sola ruta padre sin `path` con `element: <AppLayout />` (Navbar + `<Outlet />`) y 6 hijas. Cada página aporta su propio `<main>` (`HistoryPage` no lo tiene). `AppLayout` usa `useMatch('/books/:bookId')` para activar el Navbar `overlay` solo en el detalle (no en `/books/:bookId/reviews`).
- **Nested routes:** solo la anidación bajo AppLayout. Las reseñas son una ruta hermana del detalle, no un hijo.
- **Protected routes / redirects:** no existen. La "protección" es de UI: los botones que exigen sesión navegan a `/login`.
- **404 / `errorElement`:** **no existen** (una URL desconocida usa el fallback por defecto de React Router). Los ids inválidos en detalle/reseñas se manejan dentro de la página (mensaje de error + link a "/").
- **Loading de rutas (loaders):** no se usan loaders; la carga es por hooks dentro de las páginas.
- **`/profile`:** aparece en `NAV_ITEMS` (solo móvil, `disabled: true`, muestra "Soon") pero **no hay ruta**.
- **Navegación programática:** `useNavigate()` en HomePage y CategoriesPage hacia `/books/${id}` y `/categories`; BookDetailPage hacia `/books/${id}/reviews` y `/login`; BookReviewsPage hacia `/login`. `useGoBack(fallback)` en detalle (`/`) y reseñas (`/books/${id}`).

---

# 13. CUSTOM HOOKS

**9 hooks propios**: 6 en `src/shared/hooks/` (`useFetch`, `useCategories`, `useDebouncedValue`, `useBookDetail`, `useBookLoanStatus`, `useGoBack`) y 3 en archivos `hooks.ts` de feature (`useTopRatedBooks` en home, `useBookSearch` en categories, `useBookRatings` en book-reviews). Todos los de datos son envoltorios de `useFetch`.

```text
Hook: useFetch<T>(key, fetcher, options?)
File: src/shared/hooks/useFetch.ts
Purpose: Hook genérico de datos: ejecuta fetcher con AbortSignal cuando cambia `key`, devuelve {data, error, loading}, con cache opcional (memoria/localStorage), de-duplicación de requests en vuelo, fallback a datos vencidos si falla la red y opción de conservar datos previos.
Parameters:
  key: string  (DEBE codificar todos los inputs del fetcher: el efecto depende solo de [key, enabled])
  fetcher: (signal: AbortSignal) => Promise<T>
  options: { enabled=true, ttl=0 (ms; 0 = sin cache), persist=false (además en localStorage), keepPrevious=false }
Return value: { data: T|undefined, error: string|undefined, loading: boolean }
State: useState<{key, data?, error?}>. `loading = enabled && state.key !== key` (si hay cache vigente devuelve data síncronamente, loading=false).
Side effects: fetch en useEffect; AbortController (con ttl>0 la petición es compartida y NO se cancela al desmontar: solo se ignora el resultado); en error con ttl>0 intenta readStale.
Dependencies: shared/utils/cache (fetchCached, readCache, readStale)
Used by: useCategories, useBookDetail, useBookLoanStatus, useTopRatedBooks, useBookSearch, useBookRatings
Problema que resuelve: evitar re-fetch de datos casi estáticos, requests duplicados, race conditions al cambiar de key y parpadeos al paginar.
Nota: usa `// eslint-disable-next-line react-hooks/exhaustive-deps` (fetcher/ttl/persist fuera de deps a propósito).
```
```text
Hook: useCategories()
File: src/shared/hooks/useCategories.ts
Purpose: Lista de categorías con cache.
Parameters: ninguno
Return value: igual que useFetch<Category[]>
State/Side effects: vía useFetch — key 'categories', ttl 10 min, persist: true (localStorage)
Used by: HomePage, CategoriesPage
```
```text
Hook: useDebouncedValue<T>(value, delay = 400)
File: src/shared/hooks/useDebouncedValue.ts
Purpose: Devuelve `value` retrasado `delay` ms.
Return value: T
State: debounced (useState). Side effects: setTimeout limpiado en cleanup.
Used by: CategoriesPage (input de búsqueda)
```
```text
Hook: useBookDetail(bookId, enabled = true)
File: src/shared/hooks/useBookDetail.ts
Purpose: Detalle de un libro con cache corto.
Parameters: bookId: number; enabled?: boolean (las páginas pasan `validId`)
Return value: useFetch<BookDetail> → { data, error, loading }
State/Side effects: vía useFetch — key `book:${bookId}`, ttl 5 min, SOLO memoria
Dependencies: api/endpoints/books.getBookById
Used by: BookDetailPage, BookReviewsPage (comparten key → la segunda pantalla sale del cache)
Nota: antes vivía en features/book-detail/hooks.ts.
```
```text
Hook: useBookLoanStatus(bookId, enabled = true)
File: src/shared/hooks/useBookLoanStatus.ts
Purpose: Estado de préstamo del libro para el usuario actual; sirve además para saber si hay sesión (`data.userId !== null`).
Parameters: bookId: number; enabled?: boolean
Return value: useFetch<BookLoanStatus> → { data, error, loading }
State/Side effects: vía useFetch — key `loan-status:${bookId}:${'auth'|'anon'}` (según haya token en localStorage), SIN cache (ttl 0); la petición lleva `Authorization` si hay token
Dependencies: api/endpoints/books.getBookLoanStatus, api/token.getAccessToken
Used by: BookDetailPage, BookReviewsPage
```
```text
Hook: useGoBack(fallback: string)
File: src/shared/hooks/useGoBack.ts
Purpose: Handler de "volver": retrocede en el historial si existe; si no (entrada directa por URL), navega a `fallback`.
Parameters: fallback: string
Return value: () => void (memoizado con useCallback)
State/Side effects: ninguno propio; usa useNavigate y `window.history.state?.idx` (campo interno de React Router, inferencia)
Used by: BookDetailPage (fallback '/'), BookReviewsPage (fallback `/books/${id}`)
```
```text
Hook: useTopRatedBooks(categoryId?, enabled = true)
File: src/features/home/hooks.ts
Purpose: Libros mejor valorados, global o por categoría.
Return value: useFetch<Book[]>
Side effects/State: vía useFetch — key `top-rated:${categoryId ?? 'all'}`, ttl 2 min, persist: true
Dependencies: api/endpoints/books.getTopRatedBooks
Used by: HomePage (dos instancias: global y por categoría con enabled = hay categoría)
```
```text
Hook: useBookSearch(params: BookSearchParams)
File: src/features/categories/hooks.ts
Purpose: Búsqueda paginada de libros.
Return value: useFetch<PagedResponse<BookSummary>>
Side effects/State: vía useFetch — key `books:${search}|${categoryIds.join(',')}|${page}|${pageSize}`, ttl 2 min, SOLO memoria, keepPrevious: true (la grilla anterior queda atenuada mientras llega la nueva)
Dependencies: api/endpoints/books.searchBooks
Used by: CategoriesPage
```
```text
Hook: useBookRatings(bookId, page, version, enabled = true)
File: src/features/book-reviews/hooks.ts
Purpose: Reseñas paginadas de un libro (promedio, total y lista).
Parameters: bookId: number; page: number (1-based); version: number (se incrementa al publicar una reseña); enabled?: boolean
Return value: useFetch<BookRatings> → { data, error, loading }
State/Side effects: vía useFetch — key `ratings:${bookId}:${page}:5:v${version}`, ttl 1 min, SOLO memoria, keepPrevious: true (la página anterior queda atenuada mientras llega la nueva)
Dependencies: api/endpoints/books.getBookRatings, constants.RATINGS_PAGE_SIZE
Used by: BookReviewsPage
```

Hooks nativos relevantes usados en componentes: `useState`, `useEffect`, `useRef`, `useCallback`, `useId` (CategoryFilter, ExpandableText, ReviewForm), y de router `useNavigate`, `useParams`, `useSearchParams`, `useMatch` (AppLayout). `ExpandableText` usa además `ResizeObserver` dentro de un efecto. **No hay `useMemo`/`memo`/`useReducer`/`useContext`** en el código.

---

# 14. STATE MANAGEMENT

| Tipo de estado | Dónde vive | Detalle |
|---|---|---|
| Local de UI | `useState` en componentes | `menuOpen` (Navbar), `open` (CategoryFilter), `active` (HeroCarousel), `selectedCategoryId` (HomePage), `text` del input (CategoriesPage), `tab` (BookDetailPage), `expanded`/`overflowing` (ExpandableText), `page`/`version`/`formOpen`/`submitting`/`submitError`/`notice` (BookReviewsPage), `stars`/`comment` (ReviewForm) |
| Compartido | **No hay** store/context global | Se pasa por props desde la página |
| Estado de feature en la URL | `useSearchParams` | `/categories?q=&categories=&page=` (el único estado compartible/enlazable). La paginación de reseñas **no** está en la URL |
| Estado del backend | `useFetch` + cache | `data/error/loading` por key |
| Persistente | `localStorage`, prefijo `lectio:cache:` | Solo cache de categorías y top-rated (`persist: true`). Entrada `{data, expiresAt}` |
| Sesión (JWT) | `localStorage['lectio:accessToken']` | **Solo lectura** (`api/token.ts`). Nada en el código lo escribe ni lo borra; no hay estado de usuario en React |
| En memoria | `Map` en `shared/utils/cache.ts` | cache `memory` + `inflight` (requests en vuelo por key) |
| sessionStorage / Redux / Zustand / React Query / Context | **No se usan** | — |

**TTLs actuales:** categorías 10 min (persistente); top-rated 2 min (persistente); búsqueda 2 min (solo memoria); detalle de libro 5 min (solo memoria); reseñas 1 min (solo memoria); estado de préstamo **sin cache**.
**Invalidación:** `invalidateCache` existe pero **nadie la llama**; `writeCache` solo se usa dentro de `cache.ts`. Tras publicar una reseña no se invalida el cache: se cambia la key de `useBookRatings` con `version + 1`. El cache de detalle (5 min) no se toca.
**Efecto de DOM global:** `MobileMenu` fija `document.body.style.overflow` mientras está abierto.
**Mutaciones:** la única es `createRating` (POST), manejada con `useState` en la página (sin hook de mutación ni optimistic update).

---

# 15. API / SERVICES

Cliente único: `src/api/http.ts`. Autenticación: `src/api/token.ts`.

```text
Service: http<T>(path, signal?, options?)
File: src/api/http.ts
Purpose: Único wrapper de fetch. `fetch(`${env.apiUrl}${path}`, { method, signal, headers, body })`; devuelve res.json() tipado como T.
Options: { auth?: boolean (false), method?: 'GET' | 'POST' ('GET'), body?: unknown }
Authentication: con `auth: true` y token en localStorage envía `Authorization: Bearer <token>`; sin token no envía header (no falla). Sin `credentials`.
Body: si `body !== undefined` agrega `Content-Type: application/json` y usa JSON.stringify.
Errors: si !res.ok lanza `ApiError(status, path, serverMessage)`: `message` = `Error ${status} al consultar ${path}`; `status`; `serverMessage` = `detail` de problem+json, o el primer string de `errors` (validación), o `title`, o null (cuerpo vacío, 401/403 sin cuerpo o JSON inválido).
Limitaciones (hechos): solo GET y POST; sin timeout; sin validación en runtime de la respuesta.
Used by: api/endpoints/books.ts, categories.ts, ratings.ts
```
```text
Service: token
File: src/api/token.ts
Purpose: getAccessToken(): string | null — lee localStorage['lectio:accessToken'] (try/catch).
Escritura: NO existe (no hay login). Valor del token: no se documenta.
Used by: api/http.ts, shared/hooks/useBookLoanStatus.ts
```
```text
Service: books
File: src/api/endpoints/books.ts
Purpose: Endpoints de libros. BOOKS_PATH = '/api/books'

Methods:
- getTopRatedBooks(categoryId?: number, signal?) → Promise<Book[]>
    Endpoint: /api/books/top-rated[?categoryId=N] · HTTP method: GET
    Response: ApiResponse<Book[]> → devuelve res.data
    Used by: features/home/hooks.ts (useTopRatedBooks)
- searchBooks({search, categoryIds, page, pageSize}, signal?) → Promise<PagedResponse<BookSummary>>
    Endpoint: /api/books?search=&categoryIds=1&categoryIds=2&page=&pageSize= · GET
    Request: `search` solo si no está vacío; `categoryIds` repetido por cada id; page y pageSize siempre
    Response: PagedResponse<BookSummary> (objeto completo {data, pagination}, NO desempaqueta)
    Used by: features/categories/hooks.ts (useBookSearch)
- getBookById(bookId, signal?) → Promise<BookDetail>
    Endpoint: /api/books/{id} · GET · Response: objeto directo (sin `{ data }`)
    Used by: shared/hooks/useBookDetail.ts
- getBookLoanStatus(bookId, signal?) → Promise<BookLoanStatus>
    Endpoint: /api/books/{id}/loan-status · GET con `auth: true` · Response: objeto directo
    Used by: shared/hooks/useBookLoanStatus.ts
- getBookRatings(bookId, {page, pageSize}, signal?) → Promise<BookRatings>
    Endpoint: /api/books/{id}/ratings?page=&pageSize= · GET · Response: objeto directo {averageRating, numberOfRatings, users[]} (sin `pagination`)
    Used by: features/book-reviews/hooks.ts (useBookRatings)
Exporta además: interface BookSearchParams, interface BookRatingsParams
```
```text
Service: categories
File: src/api/endpoints/categories.ts
Methods:
- getCategories(signal?) → Promise<Category[]>
    Endpoint: /api/categories · GET · Response: ApiResponse<Category[]> → devuelve res.data
    Used by: shared/hooks/useCategories.ts
```
```text
Service: ratings
File: src/api/endpoints/ratings.ts
Purpose: Crear o actualizar la reseña de un usuario. USERS_PATH = '/api/users'
Methods:
- createRating(userId: number, body: CreateRatingBody, signal?) → Promise<RatingResponse>
    Endpoint: /api/users/{userId}/ratings · HTTP method: POST · Authentication: JWT (auth: true)
    Request: { bookId, stars, comment: string | null }
    Response: RatingResponse { message, ratingId, bookId, userId, stars, comment, createdAt, updatedAt }
    Used by: features/book-reviews/pages/BookReviewsPage.tsx
Reglas de negocio del backend (una reseña por usuario y libro, requisito de haber prestado el libro, que {userId} coincida con el del token): [UNKNOWN / NEEDS VERIFICATION] — no se pueden comprobar desde el front; el front solo traduce el error.
```
```text
Config: src/config/env.ts
env.apiUrl = (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:5031'
```

**Convenciones de respuesta (hechos):** tres formas conviven. `{ data }` desempaquetado (`getTopRatedBooks`, `getCategories`), `{ data, pagination }` sin desempaquetar (`searchBooks`) y objeto directo (`getBookById`, `getBookLoanStatus`, `getBookRatings`, `createRating`). Al consumir un endpoint existente, respetar la suya.

**No existe** en el frontend endpoint de login/registro, de historial, de usuarios ni de préstamo (Borrow/Deliver). Cualquier otro endpoint: `[UNKNOWN / NEEDS VERIFICATION]` (no inventar; confirmar contra el backend).

Observación: `getTopRatedBooks` usa la ruta `top-rated` pero la UI de Home lo titula **"Most Visited in Our App"** (inferencia: decisión de producto/diseño; el endpoint real es de mejor valorados).

---

# 16. TYPES / INTERFACES

```text
Type: Category            File: src/api/types/category.ts
Properties: id: number, name: string
Used by: Book(Summary), CategoryShortcuts, CategoryFilter, getCategories
```
```text
Type: Author              File: src/api/types/book.ts
Properties: id: number, name: string
Used by: BookSummary, BookDetail, utils/book.formatAuthors
```
```text
Type: BookSummary         File: src/api/types/book.ts
Purpose: "Lo mínimo que necesita una card" (lo que devuelve el buscador).
Properties: id: number; title: string; imageUrl: string|null; rating: { stars: number }|null; authors: Author[]; categories: Category[]
Used by: BookCard, BookGrid, searchBooks, PagedResponse<BookSummary>
```
```text
Type: Book (extends BookSummary)   File: src/api/types/book.ts
Properties: + description: string|null
Used by: getTopRatedBooks, HeroCarousel, BookList(dead)
```
```text
Type: BookDetail          File: src/api/types/book.ts   (NO extiende BookSummary; sin `rating`)
Properties: id: number; title: string; imageUrl: string|null; description: string|null; pageCount: number|null; publishedYear: number|null; publisher: string|null; language: string|null; authors: Author[]; categories: Category[]
Used by: getBookById, BookInfoList, BookDetailPage, BookReviewsPage (vía useBookDetail)
Nota: `language` existe en el tipo pero ninguna pantalla lo muestra.
```
```text
Type: LoanStatus          File: src/api/types/loan.ts
Definition: 'LoginRequired' | 'Available' | 'BorrowedByYou' | 'Unavailable'   (union de strings, no enum)
Type: BookLoanStatus      File: src/api/types/loan.ts
Properties: bookId: number; userId: number|null; status: LoanStatus; loanId: number|null; dueDate: string|null
Used by: getBookLoanStatus, useBookLoanStatus, LoanButton, BookDetailPage, BookReviewsPage
Nota: `userId === null` significa "sin sesión". `loanId` y `dueDate` aún no se usan en la UI.
```
```text
Type: BookRatingUser      File: src/api/types/rating.ts
Properties: idUser: number; fullName: string; comment: string|null; rating: number; createdAt: string; updatedAt: string
Type: BookRatings         File: src/api/types/rating.ts
Properties: averageRating: number; numberOfRatings: number; users: BookRatingUser[]   (sin `pagination`)
Type: CreateRatingBody    File: src/api/types/rating.ts
Properties: bookId: number; stars: number; comment: string|null
Type: RatingResponse      File: src/api/types/rating.ts
Properties: message: string; ratingId: number; bookId: number; userId: number; stars: number; comment: string|null; createdAt: string; updatedAt: string
Used by: getBookRatings, createRating, ReviewCard, ReviewList, BookReviewsPage
```
```text
Type: ApiResponse<T>      File: src/api/types/common.ts   Properties: data: T
Type: Pagination          File: src/api/types/common.ts   Properties: page, pageSize, totalCount, totalPages (number)
Type: PagedResponse<T>    File: src/api/types/common.ts   Properties: data: T[], pagination: Pagination
Used by: endpoints/books.ts, endpoints/categories.ts, CategoriesPage
```
```text
Class: ApiError           File: src/api/http.ts   (extends Error)
Properties: status: number; serverMessage: string|null; name = 'ApiError'; message = `Error ${status} al consultar ${path}`
Used by: http (lanza), features/book-reviews/utils.getSubmitErrorMessage (instanceof)
```
```text
Type: BookSearchParams    File: src/api/endpoints/books.ts
Properties: search: string, categoryIds: number[], page: number, pageSize: number
Type: BookRatingsParams   File: src/api/endpoints/books.ts
Properties: page: number, pageSize: number
```
```text
Type: NavItem             File: src/shared/components/Navbar/navItems.ts
Properties: label: string, to: string, icon: IconType, mobileOnly?: boolean, disabled?: boolean
Used by: NAV_ITEMS
Type: BookTabId           File: src/features/book-detail/constants.ts
Definition: 'summary' | 'basic-info' | 'reviews'  (derivado de BOOK_TABS `as const`)
Type: ReviewFormValues    File: src/features/book-reviews/components/ReviewForm/ReviewForm.tsx (exportado)
Properties: stars: number, comment: string
```
Tipos locales (no exportados): `HttpOptions` (http.ts), `State<T>` y `UseFetchOptions` (useFetch.ts), `Entry<T>` (cache.ts), `PageItem` (Pagination.tsx), `ParamChanges` (CategoriesPage.tsx), `InfoRow` (BookInfoList.tsx), `ButtonState` (LoanButton.tsx), y las `interface XxxProps` de cada componente. **No hay enums ni DTOs de request propios salvo `CreateRatingBody`** (el tsconfig usa `erasableSyntaxOnly`: no usar `enum`/`namespace`).

---

# 17. UTILITIES

| Utilidad | Archivo | Qué resuelve |
|---|---|---|
| `toHttps(url)` | `shared/utils/book.ts` | Reemplaza `http://` por `https://` en URLs de portadas (evita contenido mixto). Lo usan BookCard, HeroCarousel y BookHero |
| `formatAuthors(authors)` | `shared/utils/book.ts` | `'Autor desconocido'` si vacío; `"Primero +N"` si hay más de uno |
| `readCache / readStale / writeCache / invalidateCache / fetchCached` | `shared/utils/cache.ts` | Cache en memoria + localStorage con TTL, de-duplicación de requests en vuelo, lectura de datos vencidos |
| `getAccessToken()` | `api/token.ts` | Lee el JWT de `localStorage['lectio:accessToken']` (null si no hay o si el storage falla) |
| `formatReviewDate(iso)` | `features/book-reviews/utils.ts` | `dd-mm-aaaa` en UTC (fecha inválida → `''`) |
| `getSubmitErrorMessage(error)` | `features/book-reviews/utils.ts` | Convierte el error del POST en un texto para el usuario (401/403 → sesión expirada; 4xx con `serverMessage` → ese texto; otro → mensaje genérico) |
| `getPageItems(page,total)` | `Pagination.tsx` (local) | Calcula números/elipsis de paginación |
| `parseIds`, `setOrDelete` | `CategoriesPage.tsx` (locales) | Parsear/serializar ids de categoría en la URL |
| `getTriggerLabel` | `CategoryFilter.tsx` (local) | Texto del botón del filtro ("All categories", nombres, "N categories") |
| `joinNames` | `BookInfoList.tsx` (local) | Une nombres con coma o devuelve 'Unknown' |
| `getState` | `LoanButton.tsx` (local) | Traduce (status, loading, hasError) a etiqueta + disabled |
| `getTabId`, `getPanelId` | `features/book-detail/constants.ts` | Ids `book-tab-{id}` / `book-panel-{id}` para ARIA de pestañas |
| Constantes de home | `features/home/constants.ts` | `FEATURED_CATEGORIES`, `MOBILE_VISIBLE_CATEGORIES=4`, `DEFAULT_CATEGORY_ICON`, `CATEGORY_LABELS`, `CATEGORY_ICONS` |
| Constantes de detalle | `features/book-detail/constants.ts` | `BOOK_TABS`, `DEFAULT_DESCRIPTION` |
| Constantes de reseñas | `features/book-reviews/constants.ts` | `RATINGS_PAGE_SIZE=5`, `MAX_COMMENT_LENGTH=1000`, `STAR_VALUES` |
| Constantes sueltas | varios | `PAGE_SIZE=10` (CategoriesPage), TTLs en hooks, `STORAGE_PREFIX='lectio:cache:'`, `ACCESS_TOKEN_KEY` |

No hay `formatters` genéricos, validadores ni `shared/constants`.

---

# 18. FORMS AND VALIDATION

No hay librería de formularios. Hay **un solo `<form>`** (`ReviewForm`) y dos controles de búsqueda/filtro sin form.

- **Reseña (`ReviewForm`)**: `<form onSubmit>` con `preventDefault`. Estado **controlado** local (`stars`, `comment`).
  - *Estrellas*: `role="radiogroup"` con 5 `<button role="radio" aria-checked>` (valores de `STAR_VALUES`), `aria-label="{n} de 5 estrellas"`; el color pasa de `--color-disabled` a `--color-secondary` hasta el valor elegido.
  - *Comentario*: `<textarea>` con `<label htmlFor>` "Comment (optional)", `rows=4`, `maxLength=1000` (`MAX_COMMENT_LENGTH`) y contador `{n}/1000`.
  - *Validación*: obligatoria 1–5 estrellas (Submit deshabilitado y `handleSubmit` aborta si `stars === 0`); el comentario es opcional, se `trim()` y la página lo manda como `null` si queda vacío. Mientras `submitting` los botones se deshabilitan y Submit muestra "Sending…".
  - *Errores del servidor*: llegan por la prop `error` y se muestran en `<p role="alert">`. 401/403 → "Your session has expired. Please log in again."; 4xx con `serverMessage` → ese texto; resto → mensaje genérico.
  - *Límites*: el estado se pierde al cerrar; no precarga la reseña existente del usuario; sin validación de longitud mínima del comentario.
- **Búsqueda (`CategoriesPage`)**: input **controlado** (`value={text}` + `onChange`), `type="text"`, `enterKeyHint="search"`, `autoComplete="off"`, con `<label htmlFor>` visible. Sin submit: se dispara por debounce de 400 ms. Se hace `trim()` antes del debounce. Sin validación ni mensajes de error propios. No hay reset salvo vaciar el texto.
- **Filtro de categorías (`CategoryFilter`)**: checkboxes **controlados** dentro de `<label>`; el `<input>` está visualmente oculto (`opacity:0`), con foco visible simulado mediante `.item:has(.checkbox:focus-visible)`. Botón "Clear filters" resetea a `[]`.
- **Validación de URL**: `parseIds` y `page >= 1` sanean los parámetros de `/categories`; en detalle y reseñas el `bookId` se valida como entero > 0.

---

# 19. ERROR / LOADING / EMPTY STATES

| Estado | Cómo se maneja | Dónde |
|---|---|---|
| Loading | Skeletons (`.skeleton` + `pulse`) con `aria-busy="true"`; carrusel muestra un skeleton de altura fija; con `keepPrevious` la lista previa se atenúa (`.stale`) | `BookGrid`, `HeroCarousel`, `CategoryFilter` ("Loading categories…"), `BookDetailPage` y `BookReviewsPage` (componente `Skeleton` compartido), sección de reseñas |
| Error de red/HTTP | `http` lanza `ApiError`; `useFetch` expone `error: string`; si hay copia en cache (aun vencida) se muestra esa en vez del error | `BookGrid` → `<p role="alert">` |
| Error del detalle | Si no hay `data` tras cargar (id inválido, 404, red) → mensaje "We couldn't load this book. It may not exist or the server is unavailable." + link "Back to home" | `BookDetailPage`, `BookReviewsPage` |
| Error de estado de préstamo | No rompe la página: `LoanButton` muestra "Status unavailable" (deshabilitado) | `BookDetailPage` |
| Error de reseñas | `<p role="alert">` "We couldn't load the reviews. Please try again." | `BookReviewsPage` |
| Error al publicar | Texto de `getSubmitErrorMessage` dentro del formulario (`role="alert"`) | `ReviewForm` |
| Éxito al publicar | `<p role="status">` con el `message` del backend (color `--color-success`) | `BookReviewsPage` |
| Vacío | Mensaje configurable `emptyMessage` | `BookGrid` (Home: "No hay libros en esta categoría todavía."; Categories: "No books found. Try another search or remove some filters."); reseñas: "No reviews yet. Be the first to share your thoughts."; HeroCarousel devuelve `null` si no hay libros; CategoryFilter: "No categories available" |
| Sin sesión | No hay manejo de "Unauthorized": se evita en la UI llevando a `/login` ("Log in to borrow", "Add Review"). Un 401/403 al publicar muestra "Your session has expired…" | `LoanButton`, `BookReviewsPage`, `getSubmitErrorMessage` |
| Not found (ruta) | **No existe** página 404 | — |
| Error de categorías | `useCategories().error` **no se muestra en ningún lado** (HomePage/CategoriesPage solo usan `data ?? []`) | — |
| Error del hero | `mostVisited.error` tampoco se muestra en el hero (solo `loading`/`books`); la grilla de Home sí muestra `list.error` | HomePage |

Componentes reutilizables para estos estados: `BookGrid` (listas de libros) y `Skeleton` (placeholders). No hay `ErrorBoundary`, `EmptyState` ni `Spinner` genéricos; los mensajes de error/vacío de detalle y reseñas están escritos en cada página (duplicados entre `BookDetailPage` y `BookReviewsPage`).

---

# 20. RESPONSIVE DESIGN

- **Enfoque:** mobile-first con `@media (min-width: …)`. Dos breakpoints literales: **768px** y **1024px**.
- **Mobile (<768):** grilla de libros 2 columnas; hero = carrusel horizontal con `scroll-snap` y dots; navbar = botón hamburguesa + `MobileMenu` (drawer `min(80vw, 320px)`); `CategoryShortcuts` muestra los primeros 4 + "See more". Detalle: `BookHero` de 300px de alto a ancho completo con el Navbar superpuesto (overlay) y contenido en una columna (`max-width: 960px`). Reseñas: **no se muestra el libro**; cabecera propia con botón volver y título "{libro} - Reviews".
- **Tablet (≥768):** grilla de 4 columnas; slides del hero y skeleton pasan de 200px a 280px de alto; `BookHero` y su skeleton pasan de 300px a 360px. (Contenedor sigue `max-width: 960px` centrado.)
- **Desktop (≥1024):** tamaños de fuente suben (override de tokens); grilla de 5 columnas y gap `--spacing-lg`; hero = layout de 2 columnas (media + info, botón "Next"); Navbar con marca y links visibles (hamburguesa y drawer ocultos); contenedores a ancho completo con padding `clamp(var(--spacing-xl), 4vw, 80px)`; BookCard muestra botón "See More" y portada 3/4; `CategoryShortcuts` muestra todos los destacados (8) + "See more"; `CategoryFilter` lista en 5 columnas. **Detalle y reseñas** pasan a grid de 2 columnas `minmax(0,4fr) minmax(0,7fr)`: `BookHero` (aspect-ratio 3/4, radio y sombra) a la izquierda y el contenido a la derecha; el Navbar overlay vuelve a fondo blanco normal; en reseñas el botón volver de la cabecera se oculta (queda el del `BookHero`).
- **Utilidades responsive:** variable CSS inline `--cols`/`--cols-desktop` (CategoryShortcuts) y `--lines` (ExpandableText), `clamp()`, `aspect-ratio`, `minmax(0,1fr)`, `-webkit-line-clamp`. Se renderizan **ambas** ramas (mobile y desktop) en `HeroCarousel`/`BookCard`/`Navbar` y se alterna con `display`; en reseñas el `BookHero` se renderiza siempre dentro de `.hero` que está en `display: none` hasta 1024px.
- **Navegación móvil:** `Navbar` + `MobileMenu` (ver sección 9).

---

# 21. ACCESSIBILITY

Prácticas **observadas**:
- HTML semántico: `<header>`, `<nav aria-label>`, `<main>`, `<section aria-label>` (hero, resumen "General rate", reseñas), `<article>` (BookCard, ReviewCard), `<ul>/<li>` en listas (BookInfoList, ReviewList), `<label htmlFor>` en buscador y comentario, `<time dateTime>` en reseñas, `<form>` en ReviewForm.
- Botones `type="button"` en todos los controles (el componente `Button` lo aplica por defecto; `type="submit"` solo en Submit); `aria-label` en botones solo-ícono ("Abrir menú", "Cerrar menú", "Ver detalle de {título}", "Página anterior/siguiente", "Ir al libro N", "Volver").
- Estados ARIA: `aria-expanded` + `aria-controls` (hamburguesa, CategoryFilter con `useId`, ExpandableText), `aria-pressed` (tiles de categoría), `aria-current="page"` (paginación), `aria-busy` (grillas, páginas en carga, `LoanButton`, sección de reseñas), `role="alert"` (errores), `role="status"` (aviso de éxito al publicar), `role="dialog" aria-modal` (drawer), `role="img" aria-label="X de 5 estrellas"` (RatingStars), `role="tablist"/"tab"/"tabpanel"` con `aria-selected`, `aria-controls` y `aria-labelledby` (BookTabs y panel de BookDetailPage), `role="radiogroup"/"radio"` con `aria-checked` (estrellas de ReviewForm), `aria-disabled` (ítem "Soon"), `aria-hidden` en íconos/decoración y en `Skeleton`.
- Imágenes: `alt="Portada de {título}"`; fondos difuminados con `alt=""` + `aria-hidden`; `loading="lazy"` solo en `BookCard` (no en el hero ni en `BookHero`).
- Teclado: `Escape` cierra drawer y filtro; clic fuera cierra el filtro; foco inicial al botón "Cerrar" al abrir el drawer; el drawer cerrado usa `visibility:hidden` (no focuseable).
- Foco visible: `:focus-visible` con outline primary en `Button`, `BookHero .back`, `BookTabs`, `ExpandableText .toggle`, `ReviewForm` (estrellas y textarea), `BookReviewsPage .back`, input de búsqueda y filtro. **No hay estilo global de foco.**
- **Carencias observadas:** el drawer no tiene focus-trap ni devuelve el foco al botón que lo abrió; `html lang="en"` pero las `aria-label`/mensajes están mayormente en español (mezcla de idiomas, ver sección 22); no se respeta `prefers-reduced-motion`; no hay skip-link; el carrusel móvil no anuncia el cambio de slide; `BookTabs` y el selector de estrellas no soportan navegación con flechas (solo Tab/click) y los `tab` no usan roving tabindex; la pestaña "Reviews" tiene rol `tab` pero navega a otra ruta; `aria-controls` de los tabs solo se pone en el seleccionado; el aviso de éxito (`role="status"`) se monta junto con el resultado, no existe una región viva persistente. En conjunto: hay una práctica consistente a nivel de componente, pero **no una estrategia global documentada**.

---

# 22. CODE CONVENTIONS

Convenciones **observadas** (no hay Prettier ni guía escrita; lo siguiente es lo que hace el código):

- **Archivos:** componentes `PascalCase.tsx`; hooks `useXxx.ts` en `shared/hooks/` (**un archivo por hook**), o `hooks.ts` (singular, varios hooks) dentro de una feature; utilidades/constantes `camelCase.ts` (`cache.ts`, `book.ts`, `navItems.ts`, `constants.ts`, `utils.ts`, `http.ts`, `token.ts`); estilos `PascalCase.module.css` junto al componente.
- **Carpetas:** `kebab-case` para features (`book-detail`, `book-reviews`); `PascalCase` para carpetas de componente (`BookCard/`); `camelCase`/minúsculas para el resto.
- **Componentes:** función `export function Nombre(props)` — **named exports**, sin `default` (ya no queda ningún `export default`). Props en `interface NombreProps` encima del componente, no exportada (única excepción: `ReviewFormValues`, exportada porque la página la usa). Los componentes que envuelven un elemento nativo extienden sus atributos (`Button`: `Omit<ButtonHTMLAttributes, 'className'>`).
- **Hooks:** prefijo `use`; devuelven objeto con nombre (`{data,error,loading}`); los de datos son envoltorios finos de `useFetch` con TTL declarado como constante local.
- **Imports:** rutas **relativas** (hasta `../../../../`); no hay alias. `import type { … }` para tipos (obligatorio por `verbatimModuleSyntax`); a veces `type` inline (`import { searchBooks, type BookSearchParams }`). `import styles from './X.module.css'` suele ir al final del bloque de imports locales (con excepciones: `BookCard.tsx`, `CategoryShortcuts.tsx`). Orden alfabético no estricto.
- **Estilos:** CSS Modules; clases `camelCase` (`.itemSelected`, `.desktopOnly`, `.openLabel`, `.starActive`); modificadores se componen con template string o `[..].join(' ')`; sin librerías de clases (`clsx`).
- **Estructura interna de un componente/página:** hooks de estado/refs → datos derivados → handlers → early returns (loading/error/empty) → JSX. Los hooks siempre se llaman **antes** de los early returns (ver HeroCarousel, BookDetailPage, BookReviewsPage). Las páginas de detalle y reseñas validan `bookId` con `validId` y lo pasan como `enabled` a los hooks en vez de saltarse su llamada.
- **Async:** `async/await` en `endpoints` y en handlers de página (`handleSubmit` con `try/catch/finally`); promesas encadenadas en `useFetch`; `AbortSignal` como último parámetro opcional de cada función de endpoint.
- **Manejo de errores:** `throw new ApiError(...)` en `http`; `catch` → `err instanceof Error ? err.message : 'Error desconocido'` (useFetch) o `getSubmitErrorMessage(error)` (reseñas); `try/catch` con comentario breve en cache/localStorage.
- **Formato:** 2 espacios, **sin punto y coma**, comillas simples, trailing commas, ancho ≈100 col. (inferencia: no hay Prettier ni config de formato). Inconsistencias menores: `features/home/constants.ts` usa comillas dobles y `;` en un import (`react-icons/io5`) y `"Children's"`; el cierre `>` de los atributos JSX multilínea unas veces va pegado (`BookCard`, `HeroCarousel`) y otras en línea propia (`Pagination`, `Navbar`, `BookDetailPage`).
- **Comentarios:** el código quedó **casi sin comentarios**: 25 líneas en total (en español, breves, explican el "porqué"). Hay un `TODO` (logo en Navbar) y dos `eslint-disable-next-line react-hooks/exhaustive-deps` (useFetch, CategoriesPage). Si añades comentarios, mantenlos así de escasos y en español.
- **Idioma de la UI (inconsistente):** títulos/etiquetas visibles mayormente en **inglés** ("Most Visited", "See More", "Start Reading", "Add Review", "Log in to borrow", "Summary", "Basic Info", mensajes de reseñas); `aria-label`, errores de bajo nivel y mensajes por defecto de listas en **español** ("Portada de…", "Abrir menú", "Volver", "{n} de 5 estrellas", "No hay libros todavía.", "Autor desconocido"); nombres de código en inglés. Antes de añadir texto, seguir el idioma del componente vecino y **(sugerencia)** preguntar al usuario si quiere unificarlo.
- **TypeScript:** estricto, `noUnusedLocals/Parameters`, `erasableSyntaxOnly` (sin enums; los estados de `LoanStatus` son un union de strings), sin `any` en el código observado.

---

# 23. BEST PRACTICES CURRENTLY USED

Observadas realmente en el código:
- **Separación de capas:** `api/` aislado; componentes presentacionales sin fetch ni navegación; `shared/` nunca importa de `features/` (verificado por script).
- **Hooks de datos con cache propio** (`useFetch`): de-duplicación de requests, TTL, fallback a datos vencidos, cancelación y manejo de race conditions por `key`. Cada hook elige un TTL acorde al dato (5 min detalle, 1 min reseñas, sin cache para préstamo).
- **Promoción a `shared/` por uso real:** lo que usan dos features (`BookHero`, `Button`, `Skeleton`, hooks de detalle, `useGoBack`) se movió a `shared/`.
- **Estado de búsqueda en la URL** (enlazable, compatible con botón atrás; `replace` para no ensuciar el historial).
- **Debounce** de la búsqueda (`useDebouncedValue`).
- **Tokens de diseño centralizados**: prácticamente todo el CSS usa `var(--…)` (solo 3 literales fuera de tokens: dos `#111` y un `border-radius: 4px`, ambos en `HeroCarousel`). Los tokens de estado `--color-error`/`--color-success` ya se usan en reseñas.
- **Componente de lista único** (`BookGrid`) con loading/error/vacío, y **`Button`/`Skeleton`/`BookHero` compartidos**; `NAV_ITEMS` como fuente única de navegación.
- **Errores de API tipados:** `ApiError` conserva `status` y el detalle del servidor; en 5xx nunca se muestra el detalle del backend al usuario.
- **Re-fetch explícito tras mutar:** `version` en la key de `useBookRatings` fuerza una consulta nueva sin invalidar caches a mano.
- **Mobile-first** y `aspect-ratio`/skeletons para evitar saltos de layout.
- **Accesibilidad a nivel componente** (ver sección 21): tabs, radiogroup, `aria-*` de estado.
- **Type safety:** DTOs tipados, props tipadas, `import type`, modo estricto, build limpio (`tsc -b` + `eslint` sin errores).
- **Colocación:** feature-first, estilos junto al componente.

---

# 24. THINGS THAT MUST NOT BE DONE

### Styling
- **No hardcodear colores** (`#fff`, `#000`, `rgb()`, `rgba()`): usar `var(--color-*)` / `var(--overlay-dark)`.
- No crear colores nuevos si existe un token equivalente; si falta uno, añadirlo en `:root` de `src/index.css` primero.
- Para estados (error/éxito/aviso/info) usar `--color-error`, `--color-success`, `--color-warning`, `--color-info` (ya definidos; error y success ya se usan en reseñas).
- Usar tokens de `--spacing-*`, `--radius-*`, `--shadow-*`, `--font-size-*`, `--font-weight-*`; no usar `px` sueltos para esos conceptos.
- No usar Tailwind ni CSS-in-JS: el sistema es **CSS Modules**. No meter estilos de componentes en `index.css`.
- No imitar las excepciones existentes (`#111`, `border-radius: 4px` en `HeroCarousel`).
- **No volver a copiar `@keyframes pulse`/`.skeleton`:** usar el componente `Skeleton` compartido (las copias de `BookGrid`, `BookList` y `HeroCarousel` son deuda, no ejemplo).
- No repetir el grid 4fr/7fr a mano en una tercera pantalla sin antes extraerlo (ya está duplicado en detalle y reseñas).

### Components
- **Antes de crear un componente, revisar `src/shared/components/`** y las carpetas `components/` de la feature.
- Reutilizar `BookGrid`/`BookCard` para cualquier lista de libros; **no** reactivar ni copiar `BookList`.
- Reutilizar `Button` (no escribir otro botón pill), `Skeleton` (placeholders), `BookHero` (portada de libro), `RatingStars` (ratings, `size="lg"` para los grandes), `Pagination` (paginar) y `NAV_ITEMS` (navegación).
- Mantener separación: lo reutilizable entre features → `shared/`; lo exclusivo → `features/<x>/components/`. Un componente pasa a `shared/` cuando lo usa más de una feature.
- Los componentes presentacionales **no** deben hacer fetch ni llamar `useNavigate`: eso lo hace la página y baja por props (`onOpen`, `onBack`, `onAction`, etc.).
- Una carpeta por componente: `Nombre/Nombre.tsx` + `Nombre.module.css`; named export; `interface NombreProps`. `Button` no acepta `className`: no intentar estilizarlo desde fuera.

### Architecture
- Respetar la estructura `api/ · features/ · shared/ · routes/ · config/`; no mover archivos sin justificación.
- No llamar `fetch` directamente: usar `api/http.ts` y crear la función en `api/endpoints/`; tipar la respuesta en `api/types/`.
- No usar `useEffect + fetch` a mano para datos: usar `useFetch` (con `key` que incluya TODOS los parámetros, también `version`/sesión cuando afecten el resultado).
- No guardar ni borrar el JWT en sitios sueltos: hoy solo existe `getAccessToken`; cuando se implemente el login, centralizar la escritura en `api/token.ts`.
- No introducir librerías de estado/datos/UI/formularios/íconos nuevas si ya existe solución (react-icons, useFetch, CSS Modules, react-router).
- `shared/` no debe importar de `features/`; las features no se importan entre sí.
- **No fiarse del `README.md`**: está obsoleto.
- No añadir `enum`/`namespace` (rompen `erasableSyntaxOnly`); usar `import type` para tipos (`verbatimModuleSyntax`) y unions de strings.
- No subir secretos: `.env` **no** está en `.gitignore` actualmente (sugerencia: añadirlo antes de poner datos sensibles).

---

# 25. COMPLETE FILE INVENTORY

Inventario real de `src/` + configuración (excluye `node_modules`, `package-lock.json`, `.env`). **83 archivos en `src/`**: 30 `.tsx`, 27 `.ts`, 26 `.css`.

```text
# Entrada / config / rutas
src/config/env.ts
src/index.css
src/main.tsx
src/routes/AppLayout.tsx
src/routes/index.tsx

# api
src/api/endpoints/books.ts
src/api/endpoints/categories.ts
src/api/endpoints/ratings.ts
src/api/http.ts
src/api/token.ts
src/api/types/book.ts
src/api/types/category.ts
src/api/types/common.ts
src/api/types/loan.ts
src/api/types/rating.ts

# shared
src/shared/components/BookCard/BookCard.module.css
src/shared/components/BookCard/BookCard.tsx
src/shared/components/BookGrid/BookGrid.module.css
src/shared/components/BookGrid/BookGrid.tsx
src/shared/components/BookHero/BookHero.module.css
src/shared/components/BookHero/BookHero.tsx
src/shared/components/Button/Button.module.css
src/shared/components/Button/Button.tsx
src/shared/components/MobileMenu/MobileMenu.module.css
src/shared/components/MobileMenu/MobileMenu.tsx
src/shared/components/Navbar/Navbar.module.css
src/shared/components/Navbar/Navbar.tsx
src/shared/components/Navbar/navItems.ts
src/shared/components/Pagination/Pagination.module.css
src/shared/components/Pagination/Pagination.tsx
src/shared/components/RatingStars/RatingStars.module.css
src/shared/components/RatingStars/RatingStars.tsx
src/shared/components/Skeleton/Skeleton.module.css
src/shared/components/Skeleton/Skeleton.tsx
src/shared/hooks/useBookDetail.ts
src/shared/hooks/useBookLoanStatus.ts
src/shared/hooks/useCategories.ts
src/shared/hooks/useDebouncedValue.ts
src/shared/hooks/useFetch.ts
src/shared/hooks/useGoBack.ts
src/shared/utils/book.ts
src/shared/utils/cache.ts

# features
src/features/book-detail/components/BookInfoList/BookInfoList.module.css
src/features/book-detail/components/BookInfoList/BookInfoList.tsx
src/features/book-detail/components/BookTabs/BookTabs.module.css
src/features/book-detail/components/BookTabs/BookTabs.tsx
src/features/book-detail/components/ExpandableText/ExpandableText.module.css
src/features/book-detail/components/ExpandableText/ExpandableText.tsx
src/features/book-detail/components/LoanButton/LoanButton.tsx
src/features/book-detail/constants.ts
src/features/book-detail/pages/BookDetailPage.module.css
src/features/book-detail/pages/BookDetailPage.tsx
src/features/book-reviews/components/RatingSummary/RatingSummary.module.css
src/features/book-reviews/components/RatingSummary/RatingSummary.tsx
src/features/book-reviews/components/ReviewCard/ReviewCard.module.css
src/features/book-reviews/components/ReviewCard/ReviewCard.tsx
src/features/book-reviews/components/ReviewForm/ReviewForm.module.css
src/features/book-reviews/components/ReviewForm/ReviewForm.tsx
src/features/book-reviews/components/ReviewList/ReviewList.module.css
src/features/book-reviews/components/ReviewList/ReviewList.tsx
src/features/book-reviews/constants.ts
src/features/book-reviews/hooks.ts
src/features/book-reviews/pages/BookReviewsPage.module.css
src/features/book-reviews/pages/BookReviewsPage.tsx
src/features/book-reviews/utils.ts
src/features/categories/components/CategoryFilter/CategoryFilter.module.css
src/features/categories/components/CategoryFilter/CategoryFilter.tsx
src/features/categories/hooks.ts
src/features/categories/pages/CategoriesPage.module.css
src/features/categories/pages/CategoriesPage.tsx
src/features/history/pages/HistoryPage.tsx   (stub)
src/features/home/components/BookList/BookList.module.css   (dead)
src/features/home/components/BookList/BookList.tsx   (dead)
src/features/home/components/CategoryShortcuts/CategoryShortcuts.module.css
src/features/home/components/CategoryShortcuts/CategoryShortcuts.tsx
src/features/home/components/HeroCarousel/HeroCarousel.module.css
src/features/home/components/HeroCarousel/HeroCarousel.tsx
src/features/home/constants.ts
src/features/home/hooks.ts
src/features/home/pages/HomePage.module.css
src/features/home/pages/HomePage.tsx
src/features/login/pages/LoginPage.module.css
src/features/login/pages/LoginPage.tsx   (provisional)

# raíz del proyecto
index.html
package.json
package-lock.json (no documentado)
tsconfig.json
tsconfig.app.json
tsconfig.node.json
vite.config.ts
eslint.config.js
.gitignore
README.md (obsoleto)
.env (no incluido)
```

No existen en el zip: `src/App.tsx`, `src/App.css`, `src/vite-env.d.ts`, `public/`, archivos `.gitkeep`, tests.

---

# 26. COMPLETE .TSX FILES

Los **30 archivos `.tsx`** de `src/`, cada uno con su ficha y su código completo (insertado por script desde el repo, sin editar).

## 26.1 Entrada y rutas

```text
==================================================
FILE: src/main.tsx
==================================================
Purpose: Punto de entrada real de la app.
Responsibilities: crear el root de React, activar StrictMode, montar el router e importar el CSS global.
Exports: ninguno
Imports: StrictMode (react), createRoot (react-dom/client), RouterProvider (react-router/dom), router (./routes), './index.css'
Props: —
State: —
Hooks: —
Components used: RouterProvider
Components that use this file: index.html (<script type="module" src="/src/main.tsx">)
API calls: —
Styles: es el ÚNICO lugar que importa CSS global (index.css)
Important behavior: usa `document.getElementById('root')!` (non-null assertion).
Potential dependencies: routes/index.tsx
Notes: RouterProvider se importa de 'react-router/dom' (React Router 8).
```
**COMPLETE CURRENT FILE** — `src/main.tsx`

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

```text
==================================================
FILE: src/routes/index.tsx
==================================================
Purpose: Definición del router de la aplicación.
Responsibilities: declarar las rutas y el layout padre.
Exports: router (createBrowserRouter)
Imports: createBrowserRouter (react-router), BookDetailPage, BookReviewsPage, CategoriesPage, HistoryPage, HomePage, LoginPage, AppLayout
Props/State/Hooks: —
Components used: AppLayout, HomePage, HistoryPage, CategoriesPage, BookDetailPage, BookReviewsPage, LoginPage
Components that use this file: main.tsx
API calls: —  ·  Styles: —
Important behavior: ruta padre sin path con AppLayout y 6 hijas: '/', '/history', '/categories', '/books/:bookId', '/books/:bookId/reviews', '/login'. Sin errorElement ni 404 ni rutas protegidas.
Notes: para añadir una pantalla: crear la página en features/<x>/pages y registrar la ruta aquí; si va en el menú, añadir a NAV_ITEMS.
```
**COMPLETE CURRENT FILE** — `src/routes/index.tsx`

```tsx
import { createBrowserRouter } from 'react-router'
import { BookDetailPage } from '../features/book-detail/pages/BookDetailPage'
import { BookReviewsPage } from '../features/book-reviews/pages/BookReviewsPage'
import { CategoriesPage } from '../features/categories/pages/CategoriesPage'
import { HistoryPage } from '../features/history/pages/HistoryPage'
import { HomePage } from '../features/home/pages/HomePage'
import { LoginPage } from '../features/login/pages/LoginPage'
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
    ],
  },
])
```

```text
==================================================
FILE: src/routes/AppLayout.tsx
==================================================
Purpose: Layout común.
Responsibilities: renderizar Navbar y el <Outlet/> de la ruta activa; decidir si el Navbar va en modo `overlay`.
Exports: AppLayout
Imports: Outlet, useMatch (react-router), Navbar
Props: ninguna  ·  State: —
Hooks: useMatch
Components used: Navbar, Outlet
Components that use this file: routes/index.tsx
API calls: —  ·  Styles: —
Important behavior: `overlayNavbar = useMatch('/books/:bookId') !== null` → solo en el detalle del libro el Navbar se superpone a la portada (transparente con degradado). `/books/:bookId/reviews` NO coincide (useMatch exige coincidencia exacta) y usa el Navbar normal. No pasa userName/avatarUrl (se muestra 'User').
Notes: no envuelve en <main>; cada página renderiza el suyo (HistoryPage no lo hace).
```
**COMPLETE CURRENT FILE** — `src/routes/AppLayout.tsx`

```tsx
import { Outlet, useMatch } from 'react-router'
import { Navbar } from '../shared/components/Navbar/Navbar'

export function AppLayout() {
  const overlayNavbar = useMatch('/books/:bookId') !== null

  return (
    <>
      <Navbar overlay={overlayNavbar} />
      <Outlet />
    </>
  )
}
```

## 26.2 Componentes shared

```text
==================================================
FILE: src/shared/components/BookCard/BookCard.tsx
==================================================
Purpose: Tarjeta de libro.
Responsibilities: mostrar portada o placeholder, título superpuesto, estrellas, autor y botón de apertura.
Exports: BookCard
Imports: tipo BookSummary, formatAuthors/toHttps, RatingStars, styles, MdChevronRight/MdMenuBook
Props: { book: BookSummary; onOpen?: (bookId:number)=>void }
State: —  ·  Hooks: —
Components used: RatingStars
Components that use this file: BookGrid, BookList (muerto)
API calls: —
Styles: BookCard.module.css (tokens: surface, radius-md/sm, shadow-md/sm, overlay-dark, primary, text-inverse, disabled)
Important behavior: imagen con toHttps + loading="lazy" + alt "Portada de {title}"; sin imagen → ícono MdMenuBook; rating null → 0 estrellas; botón con aria-label "Ver detalle de {title}"; en mobile muestra flecha, en desktop el texto "See More" (ambos elementos en el DOM, alternados por CSS).
Potential dependencies: utils/book
Notes: el componente no navega: delega en onOpen.
```
**COMPLETE CURRENT FILE** — `src/shared/components/BookCard/BookCard.tsx`

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

```text
==================================================
FILE: src/shared/components/BookGrid/BookGrid.tsx
==================================================
Purpose: Grilla de libros con estados.
Responsibilities: elegir entre error, skeleton, vacío o grilla (atenuada si loading con datos previos).
Exports: BookGrid
Imports: tipo BookSummary, BookCard, styles
Props: { books?; loading: boolean; error?; emptyMessage? ('No hay libros todavía.'); skeletonCount? (4); onOpen? }
State/Hooks: —
Components used: BookCard
Components that use this file: HomePage, CategoriesPage
API calls: —
Styles: BookGrid.module.css (grid 2→4→5 columnas; .stale; .skeleton; .message; keyframes pulse)
Important behavior: orden de decisión: error → (loading && sin libros) → vacío → grilla. Con libros y loading=true: clase .stale (opacity .5, pointer-events none) y aria-busy.
Notes: es el único componente de estados reutilizable de la app.
```
**COMPLETE CURRENT FILE** — `src/shared/components/BookGrid/BookGrid.tsx`

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

```text
==================================================
FILE: src/shared/components/Pagination/Pagination.tsx
==================================================
Purpose: Paginación numérica.
Responsibilities: calcular items visibles y emitir cambios de página.
Exports: Pagination
Imports: MdChevronLeft/MdChevronRight, styles
Props: { page: number; totalPages: number; onChange: (page:number)=>void }
State/Hooks: —
Components used: —
Components that use this file: CategoriesPage, BookReviewsPage
API calls: —
Styles: Pagination.module.css
Important behavior: null si totalPages <= 1; ≤7 páginas: todas; si no: 1 … ventana … total (getPageItems); aria-current en la actual; flechas deshabilitadas en extremos.
Notes: key de las elipsis = su nombre ('start-ellipsis'/'end-ellipsis').
```
**COMPLETE CURRENT FILE** — `src/shared/components/Pagination/Pagination.tsx`

```tsx
import { MdChevronLeft, MdChevronRight } from 'react-icons/md'
import styles from './Pagination.module.css'

interface PaginationProps {
  page: number
  totalPages: number
  onChange: (page: number) => void
}

type PageItem = number | 'start-ellipsis' | 'end-ellipsis'

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

```text
==================================================
FILE: src/shared/components/RatingStars/RatingStars.tsx
==================================================
Purpose: Estrellas de rating proporcionales.
Responsibilities: pintar 5 estrellas grises y una capa de 5 amarillas recortada por ancho.
Exports: RatingStars
Imports: MdStar, styles
Props: { value: number; size?: 'sm' | 'lg' ('sm') }
State/Hooks: —
Components used: FiveStars (local, no exportado)
Components that use this file: BookCard, HeroCarousel, RatingSummary, ReviewCard
API calls: —
Styles: RatingStars.module.css (base color-disabled; fill color-secondary; `.lg` sube el tamaño a --font-size-lg)
Important behavior: value se limita a [0,5]; ancho del relleno = value/5·100%; role="img" aria-label "{n} de 5 estrellas"; `size="lg"` añade la clase `.lg` (usado en RatingSummary y ReviewCard).
Notes: solo muestra; no es interactivo (el selector de estrellas del formulario es propio de ReviewForm).
```
**COMPLETE CURRENT FILE** — `src/shared/components/RatingStars/RatingStars.tsx`

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

```text
==================================================
FILE: src/shared/components/Navbar/Navbar.tsx
==================================================
Purpose: Cabecera global.
Responsibilities: hamburguesa (mobile), marca, links (desktop), bloque de usuario, y host del MobileMenu.
Exports: Navbar
Imports: useCallback/useState, MdMenu, Link/NavLink, MobileMenu, styles, NAV_ITEMS
Props: { userName?: string ('User'); avatarUrl?: string; overlay?: boolean (false) }
State: menuOpen
Hooks: useState, useCallback
Components used: MobileMenu, Link, NavLink
Components that use this file: AppLayout
API calls: —
Styles: Navbar.module.css (contenedor max-width 960px en <1024; full width con padding clamp en ≥1024)
Important behavior: con overlay=true añade la clase `.overlay` (header absoluto, sin fondo, con degradado --overlay-dark → transparente; hamburguesa y nombre en --color-text-inverse); en ≥1024px vuelve a `position: static` con fondo --color-bg. DESKTOP_LINKS = NAV_ITEMS sin mobileOnly; NavLink activo con .linkActive (end para '/'); avatar con imagen o inicial en mayúscula.
Potential dependencies: navItems.ts
Notes: TODO en el código: logo real. La marca y los links están ocultos en mobile (display:none).
```
**COMPLETE CURRENT FILE** — `src/shared/components/Navbar/Navbar.tsx`

```tsx
import { useCallback, useState } from 'react'
import { MdMenu } from 'react-icons/md'
import { Link, NavLink } from 'react-router'
import { MobileMenu } from '../MobileMenu/MobileMenu'
import styles from './Navbar.module.css'
import { NAV_ITEMS } from './navItems'

interface NavbarProps {
  userName?: string
  avatarUrl?: string
  overlay?: boolean
}

const DESKTOP_LINKS = NAV_ITEMS.filter((item) => !item.mobileOnly)

export function Navbar({ userName = 'User', avatarUrl, overlay = false }: NavbarProps) {
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

          <div className={styles.user}>
            <span className={styles.name}>{userName}</span>
            {avatarUrl ? (
              <img className={styles.avatar} src={avatarUrl} alt={`Avatar de ${userName}`} />
            ) : (
              <span className={styles.avatar} aria-hidden="true">
                {userName.charAt(0).toUpperCase()}
              </span>
            )}
          </div>
        </div>
      </div>

      <MobileMenu open={menuOpen} onClose={closeMenu} />
    </header>
  )
}
```

```text
==================================================
FILE: src/shared/components/MobileMenu/MobileMenu.tsx
==================================================
Purpose: Drawer de navegación móvil.
Responsibilities: overlay + panel lateral, cierre con Escape/overlay/click en ítem, bloqueo de scroll, foco inicial.
Exports: MobileMenu
Imports: useEffect/useRef, MdClose, NavLink, NAV_ITEMS, styles
Props: { open: boolean; onClose: () => void }
State: closeRef (useRef al botón cerrar)
Hooks: useEffect (listener keydown + body overflow + foco; limpia al cerrar)
Components used: NavLink
Components that use this file: Navbar
API calls: —
Styles: MobileMenu.module.css (z-index 100/101, transiciones .25s, oculto ≥1024px)
Important behavior: ítems con disabled → <span aria-disabled> + badge "Soon"; el resto NavLink con clase activa (end solo para '/'). Drawer siempre en el DOM, oculto con visibility.
Potential dependencies: ../Navbar/navItems (import cruzado entre carpetas shared)
Notes: sin focus-trap ni retorno de foco.
```
**COMPLETE CURRENT FILE** — `src/shared/components/MobileMenu/MobileMenu.tsx`

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

```text
==================================================
FILE: src/shared/components/Skeleton/Skeleton.tsx
==================================================
Purpose: Bloque placeholder de carga (gris pulsante).
Responsibilities: pintar un <div> con fondo --color-disabled y animación `pulse`; el tamaño lo define quien lo usa vía `className`.
Exports: Skeleton
Imports: styles
Props: { className?: string ('') }
State/Hooks: —
Components used: —
Components that use this file: BookDetailPage, BookReviewsPage
API calls: —
Styles: Skeleton.module.css (fondo --color-disabled, `@keyframes pulse`)
Important behavior: aria-hidden="true"; el accesible es el contenedor (que pone aria-busy). El className se concatena como `${styles.skeleton} ${className}`.
Notes: es el skeleton compartido, pero BookGrid, BookList y HeroCarousel siguen con su propia copia de `.skeleton` + `pulse`.
```
**COMPLETE CURRENT FILE** — `src/shared/components/Skeleton/Skeleton.tsx`

```tsx
import styles from './Skeleton.module.css'

interface SkeletonProps {
  className?: string
}

export function Skeleton({ className = '' }: SkeletonProps) {
  return <div className={`${styles.skeleton} ${className}`} aria-hidden="true" />
}
```

```text
==================================================
FILE: src/shared/components/Button/Button.tsx
==================================================
Purpose: Botón pill compartido.
Responsibilities: aplicar el estilo de botón de la app (primario relleno o secundario con borde) y reenviar el resto de atributos nativos.
Exports: Button
Imports: tipo ButtonHTMLAttributes, styles
Props: ButtonHTMLAttributes<HTMLButtonElement> menos `className` + { variant?: 'primary' | 'secondary' ('primary'); block?: boolean (false) }
State/Hooks: —
Components used: —
Components that use this file: LoanButton, BookReviewsPage ("Add Review"), ReviewForm (Submit y Cancel)
API calls: —
Styles: Button.module.css (min-height 44px; primary = --color-primary + --shadow-md; secondary = borde primary sobre --color-bg; disabled = --color-disabled + --color-text-muted; `.block` = width min(100%, 260px); :focus-visible con outline primary)
Important behavior: `type` por defecto 'button' (para enviar un form hay que pasar type="submit", como hace ReviewForm); como `className` está excluido del tipo, NO se puede añadir clases desde fuera.
Notes: no lo usan BookCard (`.open`) ni HeroCarousel (`.action`): conservan su propio botón pill.
```
**COMPLETE CURRENT FILE** — `src/shared/components/Button/Button.tsx`

```tsx
import type { ButtonHTMLAttributes } from 'react'
import styles from './Button.module.css'

interface ButtonProps extends Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'className'> {
  variant?: 'primary' | 'secondary'
  block?: boolean
}

export function Button({ variant = 'primary', block = false, type = 'button', ...props }: ButtonProps) {
  const className = [styles.button, styles[variant], block ? styles.block : '']
    .filter(Boolean)
    .join(' ')

  return <button type={type} className={className} {...props} />
}
```

```text
==================================================
FILE: src/shared/components/BookHero/BookHero.tsx
==================================================
Purpose: Portada grande del libro con botón volver y título superpuesto.
Responsibilities: mostrar la imagen (o un ícono placeholder), el botón "Volver" y una franja con título y categoría.
Exports: BookHero
Imports: MdChevronLeft, MdMenuBook, toHttps, styles
Props: { title: string; category?: string; imageUrl: string | null; onBack: () => void; headingLevel?: 1 | 2 (1) }
State/Hooks: —
Components used: —
Components that use this file: BookDetailPage (h1), BookReviewsPage (h2, porque esa página ya tiene su propio <h1>)
API calls: —
Styles: BookHero.module.css (300px → 360px (768) → aspect-ratio 3/4 con max-height calc(100vh - 160px), radio y --shadow-lg en ≥1024; z-index 1 en el botón volver)
Important behavior: imagen con toHttps y alt "Portada de {title}"; sin imagen → MdMenuBook con fondo --color-primary-hover; el botón (aria-label "Volver") solo llama a onBack (no navega); el título se corta a 2 líneas.
Notes: antes vivía en features/book-detail/components/; se movió a shared al reutilizarlo reseñas. El `top` del botón es calc(--spacing-xl * 2) en mobile para librar el Navbar en overlay.
```
**COMPLETE CURRENT FILE** — `src/shared/components/BookHero/BookHero.tsx`

```tsx
import { MdChevronLeft, MdMenuBook } from 'react-icons/md'
import { toHttps } from '../../utils/book'
import styles from './BookHero.module.css'

interface BookHeroProps {
  title: string
  category?: string
  imageUrl: string | null
  onBack: () => void
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

## 26.3 Feature home

```text
==================================================
FILE: src/features/home/pages/HomePage.tsx
==================================================
Purpose: Página de inicio.
Responsibilities: orquestar datos (categorías, top-rated global y por categoría), estado de categoría seleccionada y navegación.
Exports: HomePage
Imports: useState, useNavigate, BookGrid, CategoryShortcuts, HeroCarousel, useCategories, useTopRatedBooks, styles
Props: ninguna
State: selectedCategoryId (number|undefined)
Hooks: useState, useNavigate, useCategories, useTopRatedBooks ×2
Components used: HeroCarousel, CategoryShortcuts, BookGrid
Components that use this file: routes/index.tsx
API calls (indirectas): GET /api/categories; GET /api/books/top-rated; GET /api/books/top-rated?categoryId=N (solo con categoría)
Styles: HomePage.module.css (.main, .kicker, .title, .accent, .lineBreak)
Important behavior: sin categoría reutiliza la data del hero para la grilla (evita un 2.º request). El hero SIEMPRE usa la lista global. Solo se muestra list.error en la grilla (no el error del hero ni de categorías).
Notes: títulos con <span class=accent> en verde primario; <br> condicional solo en desktop (.lineBreak).
```
**COMPLETE CURRENT FILE** — `src/features/home/pages/HomePage.tsx`

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
  const mostVisited = useTopRatedBooks() 
  const byCategory = useTopRatedBooks(selectedCategoryId, selectedCategoryId !== undefined)

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

```text
==================================================
FILE: src/features/home/components/HeroCarousel/HeroCarousel.tsx
==================================================
Purpose: Hero de libros destacados.
Responsibilities: mobile = carrusel con scroll-snap + dots; desktop = libro destacado con info, "Start Reading" y "Next".
Exports: HeroCarousel
Imports: useRef/useState, tipo Book, RatingStars, formatAuthors/toHttps, styles
Props: { books: Book[]; loading: boolean; onOpen?: (bookId:number)=>void }
State: active (índice); trackRef
Hooks: useRef, useState
Components used: RatingStars
Components that use this file: HomePage
API calls: —
Styles: HeroCarousel.module.css (único con #111 literal; skeleton propio; z/posicionamiento absoluto de capas bg/shade/cover)
Important behavior: loading → skeleton; sin libros → null; `active` lo actualiza el scroll del track (mobile) y el botón Next (desktop, rota con módulo); Next NO desplaza el track móvil; ambas ramas (mobile/desktop) están en el DOM y CSS alterna con display; descripción con line-clamp 5; botón Next deshabilitado si <2 libros.
Notes: imágenes del hero sin loading="lazy".
```
**COMPLETE CURRENT FILE** — `src/features/home/components/HeroCarousel/HeroCarousel.tsx`

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

```text
==================================================
FILE: src/features/home/components/CategoryShortcuts/CategoryShortcuts.tsx
==================================================
Purpose: Fila de tiles de categorías destacadas + "See more".
Responsibilities: resolver FEATURED_CATEGORIES contra las categorías del API y manejar selección tipo toggle.
Exports: CategoryShortcuts
Imports: CSSProperties, MdGridView, tipo Category, constantes de ../../constants, styles
Props: { categories: Category[]; selectedId?: number; onSelect: (categoryId?: number)=>void; onSeeMore: () => void }
State/Hooks: —
Components used: —
Components that use this file: HomePage
API calls: —
Styles: CategoryShortcuts.module.css (grid con --cols/--cols-desktop inline)
Important behavior: solo muestran tiles las categorías cuyo name coincide EXACTAMENTE con FEATURED_CATEGORIES; los índices ≥ MOBILE_VISIBLE_CATEGORIES (4) llevan .desktopOnly; click sobre la activa → onSelect(undefined); aria-pressed y aria-label por tile.
Notes: className se arma con array.join(' ') (deja espacios sobrantes, inofensivo).
```
**COMPLETE CURRENT FILE** — `src/features/home/components/CategoryShortcuts/CategoryShortcuts.tsx`

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

  const mobileColumns = Math.min(featured.length, MOBILE_VISIBLE_CATEGORIES) + 1
  const desktopColumns = featured.length + 1

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

```text
==================================================
FILE: src/features/home/components/BookList/BookList.tsx   (DEAD CODE)
==================================================
Purpose: Versión antigua de la lista de libros de Home.
Exports: BookList
Imports: tipo Book, BookCard, styles
Props: { books?: Book[]; loading: boolean; error?: string; onOpen? }
State/Hooks: —
Components used: BookCard
Components that use this file: NINGUNO
Styles: BookList.module.css (duplicado casi idéntico de BookGrid.module.css)
Important behavior: error → loading → vacío → grilla (mensaje vacío fijo "No hay libros en esta categoría todavía.").
Notes: reemplazado por shared/BookGrid. No reutilizar; candidato a eliminar (solo con confirmación del usuario).
```
**COMPLETE CURRENT FILE** — `src/features/home/components/BookList/BookList.tsx`

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

## 26.4 Feature categories

```text
==================================================
FILE: src/features/categories/pages/CategoriesPage.tsx
==================================================
Purpose: Búsqueda, filtro y paginación del catálogo.
Responsibilities: sincronizar estado ↔ URL, debounce de texto, componer filtro + grilla + paginador.
Exports: CategoriesPage
Imports: useEffect/useState, MdSearch, useNavigate/useSearchParams, BookGrid, Pagination, useCategories, useDebouncedValue, CategoryFilter, useBookSearch, styles
Props: ninguna
State: text (useState, inicial = q de la URL); estado derivado de la URL: search, page, categoryIds
Hooks: useNavigate, useSearchParams, useState, useDebouncedValue, useCategories, useBookSearch, useEffect
Components used: CategoryFilter, BookGrid, Pagination
Components that use this file: routes/index.tsx
API calls (indirectas): GET /api/categories; GET /api/books?search=&categoryIds=..&page=&pageSize=10
Styles: CategoriesPage.module.css
Important behavior: PAGE_SIZE=10; updateParams(changes, replace=true) escribe q/categories/page en la URL (page=1 se omite); cambios de texto y de filtro usan replace y reinician page=1; cambio de página usa push + scrollTo top; parseIds descarta valores no enteros/≤0, de-duplica y ordena; muestra "N book(s) found".
Notes: `text` solo se inicializa desde la URL al montar (no se resincroniza si q cambia por navegación atrás/adelante) — comportamiento observado, posible mejora. Un `eslint-disable` para exhaustive-deps en el efecto del debounce. Textos de UI en inglés.
```
**COMPLETE CURRENT FILE** — `src/features/categories/pages/CategoriesPage.tsx`

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

  const search = params.get('q') ?? ''
  const page = Math.max(1, parseInt(params.get('page') ?? '', 10) || 1)
  const categoryIds = parseIds(params.get('categories'))

  const [text, setText] = useState(search)
  const debouncedText = useDebouncedValue(text.trim(), 400)

  const categories = useCategories()
  const results = useBookSearch({ search, categoryIds, page, pageSize: PAGE_SIZE })

  const books = results.data?.data
  const pagination = results.data?.pagination

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

```text
==================================================
FILE: src/features/categories/components/CategoryFilter/CategoryFilter.tsx
==================================================
Purpose: Dropdown de selección múltiple de categorías.
Responsibilities: abrir/cerrar panel, alternar ids, limpiar filtros, accesibilidad básica.
Exports: CategoryFilter
Imports: useEffect/useId/useRef/useState, MdCheck/MdExpandLess/MdExpandMore, tipo Category, styles
Props: { categories: Category[]; selectedIds: number[]; onChange: (ids:number[])=>void; loading?: boolean }
State: open; rootRef; labelId/panelId (useId)
Hooks: useState, useRef, useId, useEffect (pointerdown fuera + Escape, solo si open)
Components used: —
Components that use this file: CategoriesPage
API calls: —
Styles: CategoryFilter.module.css (panel absoluto z-index 20; lista 1 col → 5 cols en ≥1024)
Important behavior: etiqueta del trigger por getTriggerLabel; checkbox nativo oculto con foco visible vía :has(); "Clear filters" visible si hay selección; mensajes "Loading categories…" / "No categories available".
Notes: componente controlado (la verdad vive en la URL de CategoriesPage).
```
**COMPLETE CURRENT FILE** — `src/features/categories/components/CategoryFilter/CategoryFilter.tsx`

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

## 26.5 Feature book-detail

```text
==================================================
FILE: src/features/book-detail/pages/BookDetailPage.tsx
==================================================
Purpose: Detalle de un libro (portada, pestañas Summary / Basic Info, botón de préstamo).
Responsibilities: validar el id de la URL, cargar detalle y estado de préstamo, decidir loading / error / contenido, y navegar a reseñas o al login.
Exports: BookDetailPage
Imports: useState; Link/useNavigate/useParams (react-router); tipo LoanStatus; BookHero, Skeleton; useBookDetail, useBookLoanStatus, useGoBack; BookInfoList, BookTabs, ExpandableText, LoanButton; DEFAULT_DESCRIPTION, getPanelId, getTabId, BookTabId; styles
Props: ninguna
State: tab (BookTabId, inicial 'summary')
Hooks: useNavigate, useParams, useState, useBookDetail(id, validId), useBookLoanStatus(id, validId), useGoBack('/')
Components used: BookHero, BookTabs, ExpandableText, BookInfoList, LoanButton, Skeleton, Link
Components that use this file: routes/index.tsx
API calls (indirectas): GET /api/books/{id}; GET /api/books/{id}/loan-status (con JWT si existe)
Styles: BookDetailPage.module.css (columna única hasta 1024px; en ≥1024 grid 4fr/7fr con la portada a la izquierda; skeletons de hero/tabs/texto)
Important behavior: validId = entero > 0; con id inválido los hooks quedan `enabled=false` y se muestra el estado de error ("We couldn't load this book…" + link "Back to home"). Loading → skeletons con aria-busy. Resumen = description.trim() o DEFAULT_DESCRIPTION, mostrado entre comillas dentro de ExpandableText. La pestaña "Reviews" NO cambia de panel: handleTabChange hace navigate(`/books/${id}/reviews`), así que `tab` nunca vale 'reviews'. handleLoanAction solo actúa con status 'LoginRequired' (→ /login); Borrow y Deliver todavía no hacen nada. La categoría mostrada en la portada es la primera de detail.categories.
Notes: AppLayout pone el Navbar en overlay solo en esta ruta. Un error de loan-status NO rompe la página: el botón muestra "Status unavailable".
```
**COMPLETE CURRENT FILE** — `src/features/book-detail/pages/BookDetailPage.tsx`

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

  // El login aún no existe en el front: va a su página provisional
  const handleLoanAction = (status: LoanStatus) => {
    if (status === 'LoginRequired') navigate('/login')
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

```text
==================================================
FILE: src/features/book-detail/components/BookTabs/BookTabs.tsx
==================================================
Purpose: Barra de pestañas del libro.
Responsibilities: renderizar BOOK_TABS como tablist accesible y avisar el cambio.
Exports: BookTabs
Imports: BOOK_TABS, getPanelId, getTabId, BookTabId, styles
Props: { active: BookTabId; onChange: (tab: BookTabId) => void }
State/Hooks: —
Components used: —
Components that use this file: BookDetailPage
API calls: —
Styles: BookTabs.module.css (activa = --color-primary + semibold; inactiva muted; focus-visible)
Important behavior: role="tablist"/"tab", aria-selected, id por pestaña; aria-controls solo en la seleccionada (apunta al panel que renderiza la página).
Notes: sin navegación con flechas (solo click/Tab); la página trata 'reviews' como navegación, no como panel.
```
**COMPLETE CURRENT FILE** — `src/features/book-detail/components/BookTabs/BookTabs.tsx`

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

```text
==================================================
FILE: src/features/book-detail/components/BookInfoList/BookInfoList.tsx
==================================================
Purpose: Lista de datos básicos del libro (pestaña Basic Info).
Responsibilities: mostrar autores, editorial, año, páginas y categorías con un ícono cada uno.
Exports: BookInfoList
Imports: IconType, MdApartment/MdCalendarMonth/MdGroups/MdLayers/MdLocalOffer, tipo BookDetail, styles
Props: { book: BookDetail }
State/Hooks: —
Components used: —
Components that use this file: BookDetailPage
API calls: —
Styles: BookInfoList.module.css
Important behavior: filas Author/Authors (según cantidad), Editorial (publisher), Publication (publishedYear), Pages (pageCount), Categories; valor ausente → 'Unknown'; <ul> con <li> por fila.
Notes: el tipo local InfoRow {icon,label,value}; los labels están en inglés ("Editorial" es el texto de la UI para publisher).
```
**COMPLETE CURRENT FILE** — `src/features/book-detail/components/BookInfoList/BookInfoList.tsx`

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

```text
==================================================
FILE: src/features/book-detail/components/ExpandableText/ExpandableText.tsx
==================================================
Purpose: Párrafo largo con corte a N líneas y botón "See more / See less".
Responsibilities: recortar el texto con line-clamp, detectar si desborda y alternar expandido.
Exports: ExpandableText
Imports: useEffect/useId/useRef/useState, tipo CSSProperties, styles
Props: { text: string; lines?: number (14) }
State: expanded, overflowing; textRef (useRef), textId (useId)
Hooks: useState, useRef, useId, useEffect (ResizeObserver)
Components used: —
Components that use this file: BookDetailPage
API calls: —
Styles: ExpandableText.module.css (`.clamped` usa -webkit-line-clamp: var(--lines); texto justificado, line-height 1.5)
Important behavior: pasa `lines` al CSS por la variable inline `--lines`; el ResizeObserver compara scrollHeight vs clientHeight (+1) y se desconecta en el cleanup; el botón solo aparece si desborda o ya está expandido; aria-expanded + aria-controls.
Notes: con expanded=true el efecto no observa (retorna temprano).
```
**COMPLETE CURRENT FILE** — `src/features/book-detail/components/ExpandableText/ExpandableText.tsx`

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

```text
==================================================
FILE: src/features/book-detail/components/LoanButton/LoanButton.tsx
==================================================
Purpose: Botón de acción de préstamo según el estado del usuario con el libro.
Responsibilities: traducir LoanStatus (o carga/error) a etiqueta y estado deshabilitado, y reportar el click.
Exports: LoanButton
Imports: tipo LoanStatus, Button (shared)
Props: { status?: LoanStatus; loading?: boolean (false); hasError?: boolean (false); onAction?: (status: LoanStatus) => void }
State/Hooks: —
Components used: Button (block)
Components that use this file: BookDetailPage
API calls: —
Styles: ninguno propio (usa Button.module.css; antes tenía LoanButton.module.css)
Important behavior: loading → "Loading…" (deshabilitado); error o sin status → "Status unavailable" (deshabilitado); LoginRequired → "Log in to borrow"; Available → "Borrow"; BorrowedByYou → "Deliver"; Unavailable (o status desconocido) → "Unavailable" (deshabilitado). aria-busy={loading}. El click llama onAction(status).
Notes: la acción real de Borrow/Deliver no existe todavía en el front (la página solo maneja LoginRequired).
```
**COMPLETE CURRENT FILE** — `src/features/book-detail/components/LoanButton/LoanButton.tsx`

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

## 26.6 Feature book-reviews

```text
==================================================
FILE: src/features/book-reviews/pages/BookReviewsPage.tsx
==================================================
Purpose: Pantalla de reseñas de un libro (promedio, lista paginada y formulario para reseñar).
Responsibilities: cargar detalle, estado de préstamo (para saber si hay sesión) y reseñas; abrir/cerrar el formulario; enviar la reseña; refrescar la lista tras publicar.
Exports: BookReviewsPage
Imports: useState, ReactNode; MdChevronLeft; Link/useNavigate/useParams; createRating; BookHero, Button, Pagination, Skeleton; useBookDetail, useBookLoanStatus, useGoBack; RatingSummary, ReviewForm (+ ReviewFormValues), ReviewList; RATINGS_PAGE_SIZE; useBookRatings; getSubmitErrorMessage; styles
Props: ninguna
State: page (1), version (0), formOpen, submitting, submitError, notice
Hooks: useNavigate, useParams, useState ×6, useGoBack(`/books/${id}`), useBookDetail, useBookLoanStatus, useBookRatings(id, page, version, validId)
Components used: BookHero (headingLevel 2), RatingSummary, Button, ReviewForm, ReviewList, Pagination, Skeleton, Link
Components that use this file: routes/index.tsx
API calls (indirectas): GET /api/books/{id}; GET /api/books/{id}/loan-status; GET /api/books/{id}/ratings?page=&pageSize=5; POST /api/users/{userId}/ratings (con JWT)
Styles: BookReviewsPage.module.css (columna única; en ≥1024 grid 4fr/7fr con BookHero a la izquierda; en mobile el libro no se muestra y hay un botón volver propio)
Important behavior: la sesión se infiere de loan.data?.userId (null = sin sesión). "Add Review": sin sesión → navigate('/login'); con sesión abre ReviewForm. handleSubmit llama createRating(userId, {bookId, stars, comment: comment || null}); éxito → cierra el form, muestra `res.message` en un <p role="status">, vuelve a page 1 y hace version+1 (cambia la key de useBookRatings y fuerza recarga de lista y promedio); error → setSubmitError(getSubmitErrorMessage(error)). totalPages = ceil(numberOfRatings / 5) porque el endpoint no devuelve `pagination`. Cambio de página: scrollTo top suave. Estados: skeleton si el libro carga; error con link a home si no hay libro; <p role="alert"> si falla la lista; "No reviews yet. Be the first to share your thoughts." si no hay reseñas; sección atenuada (.stale) mientras llega otra página.
Notes: hay un <h1> "{título} - Reviews" y el BookHero usa <h2> para no duplicar h1. El botón "Add Review" se deshabilita mientras carga el estado de préstamo o el formulario está abierto.
```
**COMPLETE CURRENT FILE** — `src/features/book-reviews/pages/BookReviewsPage.tsx`

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
      navigate('/login') // El login aún no existe en el front: va a su página provisional
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

```text
==================================================
FILE: src/features/book-reviews/components/RatingSummary/RatingSummary.tsx
==================================================
Purpose: Bloque con el promedio y el número de reseñas.
Responsibilities: mostrar "General Rate", el promedio con 1 decimal, estrellas grandes y "rate counter: N".
Exports: RatingSummary
Imports: RatingStars, styles
Props: { average: number; count: number }
State/Hooks: —
Components used: RatingStars (size="lg")
Components that use this file: BookReviewsPage
API calls: —
Styles: RatingSummary.module.css (promedio en --font-size-3xl bold color primary; estrellas dentro de un pill --color-surface con --shadow-md)
Important behavior: <section aria-label="General rate">; average.toFixed(1).
Notes: único consumidor de --font-size-3xl.
```
**COMPLETE CURRENT FILE** — `src/features/book-reviews/components/RatingSummary/RatingSummary.tsx`

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

```text
==================================================
FILE: src/features/book-reviews/components/ReviewCard/ReviewCard.tsx
==================================================
Purpose: Tarjeta de una reseña.
Responsibilities: mostrar avatar genérico, nombre, comentario (si existe), fecha y estrellas.
Exports: ReviewCard
Imports: MdPerson, tipo BookRatingUser, RatingStars, formatReviewDate, styles
Props: { review: BookRatingUser }
State/Hooks: —
Components used: RatingStars (size="lg")
Components that use this file: ReviewList
API calls: —
Styles: ReviewCard.module.css (card --color-surface + --radius-md + --shadow-md; avatar circular --color-primary-light)
Important behavior: nombre = review.fullName; el comentario solo se pinta si no es null/vacío; la fecha usa `updatedAt` (no createdAt) dentro de <time dateTime>; el avatar es un ícono (no hay foto).
Notes: <article> semántico.
```
**COMPLETE CURRENT FILE** — `src/features/book-reviews/components/ReviewCard/ReviewCard.tsx`

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

```text
==================================================
FILE: src/features/book-reviews/components/ReviewList/ReviewList.tsx
==================================================
Purpose: Lista de reseñas.
Responsibilities: renderizar una ReviewCard por reseña dentro de <ul>.
Exports: ReviewList
Imports: tipo BookRatingUser, ReviewCard, styles
Props: { reviews: BookRatingUser[] }
State/Hooks: —
Components used: ReviewCard
Components that use this file: BookReviewsPage
API calls: —
Styles: ReviewList.module.css (columna con gap --spacing-md, sin viñetas)
Important behavior: key = review.idUser (una reseña por usuario y libro dentro de la lista).
Notes: no maneja vacío ni loading: lo hace la página.
```
**COMPLETE CURRENT FILE** — `src/features/book-reviews/components/ReviewList/ReviewList.tsx`

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
        <li key={review.idUser}>
          <ReviewCard review={review} />
        </li>
      ))}
    </ul>
  )
}
```

```text
==================================================
FILE: src/features/book-reviews/components/ReviewForm/ReviewForm.tsx
==================================================
Purpose: Formulario para crear o actualizar la reseña propia (estrellas + comentario).
Responsibilities: capturar stars (1–5) y comentario, validar mínimos, mostrar contador y error, y emitir onSubmit/onCancel.
Exports: ReviewForm, ReviewFormValues (interface exportada {stars, comment})
Imports: useId/useState, tipo FormEvent, MdStar, Button, MAX_COMMENT_LENGTH, STAR_VALUES, styles
Props: { submitting: boolean; error: string | null; onSubmit: (values: ReviewFormValues) => void; onCancel: () => void }
State: stars (0), comment ('')
Hooks: useState ×2, useId ×2
Components used: Button (primary submit, secondary cancel)
Components that use this file: BookReviewsPage
API calls: — (la página llama a createRating)
Styles: ReviewForm.module.css (card --color-surface; estrellas --color-disabled → --color-secondary al activarse; textarea con borde --color-border; error --color-error)
Important behavior: es el ÚNICO <form> de la app. handleSubmit hace preventDefault y no envía si stars===0 o submitting; envía comment.trim(). Selector de estrellas como role="radiogroup" con 5 botones role="radio" + aria-checked + aria-label "{n} de 5 estrellas". Textarea rows=4, maxLength=1000, contador "{n}/1000", label "Comment (optional)". Submit deshabilitado sin estrellas o mientras envía ("Sending…").
Notes: el estado es local y se pierde al cerrar el formulario; no precarga la reseña previa del usuario.
```
**COMPLETE CURRENT FILE** — `src/features/book-reviews/components/ReviewForm/ReviewForm.tsx`

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

## 26.7 Login provisional y stub de historial

```text
==================================================
FILE: src/features/login/pages/LoginPage.tsx
==================================================
Purpose: Página provisional de login.
Responsibilities: informar que el login aún no existe y ofrecer volver al inicio.
Exports: LoginPage
Imports: Link, styles
Props/State/Hooks: —
Components that use this file: routes/index.tsx
API calls: ninguna
Styles: LoginPage.module.css (columna centrada, max-width 480px)
Important behavior: texto fijo "Login is coming soon…" + link "Back to home". No hay formulario, ni escritura del token.
Notes: es el destino de los botones que exigen sesión (Borrow/"Log in to borrow" y "Add Review").
```
**COMPLETE CURRENT FILE** — `src/features/login/pages/LoginPage.tsx`

```tsx
import { Link } from 'react-router'
import styles from './LoginPage.module.css'

export function LoginPage() {
  return (
    <main className={styles.main}>
      <h1 className={styles.title}>Log in</h1>
      <p className={styles.message}>
        Login is coming soon. Once it is ready you will be able to borrow books and leave reviews.
      </p>
      <Link to="/" className={styles.link}>
        Back to home
      </Link>
    </main>
  )
}
```

```text
==================================================
FILE: src/features/history/pages/HistoryPage.tsx   (STUB)
==================================================
Purpose: Historial (pendiente).
Exports: HistoryPage · Imports: ninguno · Props/State/Hooks: —
Components that use this file: routes/index.tsx
Important behavior: renderiza <h1>History</h1>.
```
**COMPLETE CURRENT FILE** — `src/features/history/pages/HistoryPage.tsx`

```tsx
export function HistoryPage() {
  return <h1>History</h1>
}
```

---

# 27. COMPLETE .TS FILES

Los **27 archivos `.ts`** de `src/`, cada uno con su ficha y su código completo (insertado por script desde el repo, sin editar).

## 27.1 Capa api y config

```text
==================================================
FILE: src/api/http.ts
==================================================
Purpose: Wrapper único de fetch (GET/POST, con JWT opcional).
Exports: http<T>(path, signal?, options?), ApiError (class)
Imports: env, getAccessToken
Functions: http; readErrorDetail (local, async)
Hooks: — · Types: genérico T; HttpOptions (local: { auth?: boolean; method?: 'GET' | 'POST'; body?: unknown })
API calls: cualquier GET/POST a ${env.apiUrl}${path}
Dependencies: config/env, api/token
Used by: endpoints/books.ts, endpoints/categories.ts, endpoints/ratings.ts (ApiError también lo importa features/book-reviews/utils.ts)
Important behavior: con `auth: true` y token presente envía `Authorization: Bearer <token>` (si no hay token, la petición sale sin header, sin fallar); con `body` define Content-Type application/json y serializa con JSON.stringify. Si !res.ok lanza ApiError(status, path, serverMessage): `message` sigue siendo 'Error {status} al consultar {path}' y `serverMessage` es el texto que manda el backend (`detail` de problem+json; si no, el primer string de `errors`; si no, `title`; si no, null; cualquier fallo al leer el cuerpo → null). Devuelve res.json() tipado como T (sin validación en runtime).
Notes: no hay PUT/PATCH/DELETE ni timeouts; una respuesta OK sin cuerpo JSON haría fallar res.json() (inferencia). El `signal` va como 2.º parámetro para que las funciones de endpoint puedan reenviarlo.
```
**COMPLETE CURRENT FILE** — `src/api/http.ts`

```ts
import { env } from '../config/env'
import { getAccessToken } from './token'

interface HttpOptions {
  auth?: boolean
  method?: 'GET' | 'POST'
  body?: unknown
}

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

export async function http<T>(
  path: string,
  signal?: AbortSignal,
  { auth = false, method = 'GET', body }: HttpOptions = {},
): Promise<T> {
  const token = auth ? getAccessToken() : null

  const headers: Record<string, string> = {}
  if (token) headers.Authorization = `Bearer ${token}`
  if (body !== undefined) headers['Content-Type'] = 'application/json'

  const res = await fetch(`${env.apiUrl}${path}`, {
    method,
    signal,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })
  if (!res.ok) throw new ApiError(res.status, path, await readErrorDetail(res))
  return res.json() as Promise<T>
}
```

```text
==================================================
FILE: src/api/token.ts
==================================================
Purpose: Lectura del JWT guardado en el navegador.
Exports: getAccessToken(): string | null
Imports: —
Functions: getAccessToken (try/catch por si localStorage no está disponible)
API calls: —
Used by: api/http.ts (cuando auth=true), shared/hooks/useBookLoanStatus.ts (para distinguir 'auth' / 'anon' en la key)
Important behavior: lee localStorage['lectio:accessToken'].
Notes: **no existe ninguna función que escriba o borre el token** (no hay login): hoy solo se podría poblar manualmente en el navegador. No se documenta ningún valor.
```
**COMPLETE CURRENT FILE** — `src/api/token.ts`

```ts
const ACCESS_TOKEN_KEY = 'lectio:accessToken'

export function getAccessToken(): string | null {
  try {
    return localStorage.getItem(ACCESS_TOKEN_KEY)
  } catch {
    return null
  }
}
```

```text
==================================================
FILE: src/api/endpoints/books.ts
==================================================
Purpose: Funciones de endpoints de libros.
Exports: BookRatingsParams, BookSearchParams (interfaces), getTopRatedBooks, searchBooks, getBookById, getBookLoanStatus, getBookRatings
Imports: http, tipos Book/BookDetail/BookSummary, ApiResponse/PagedResponse, BookLoanStatus, BookRatings
Functions: getTopRatedBooks (async, desempaqueta res.data), searchBooks (devuelve PagedResponse completo), getBookById (objeto directo), getBookLoanStatus (objeto directo, auth: true), getBookRatings (objeto directo)
Types: BookSearchParams, BookRatingsParams
API calls: GET /api/books/top-rated[?categoryId]; GET /api/books?search&categoryIds&page&pageSize; GET /api/books/{id}; GET /api/books/{id}/loan-status; GET /api/books/{id}/ratings?page&pageSize
Dependencies: api/http, api/types
Used by: features/home/hooks.ts, features/categories/hooks.ts, shared/hooks/useBookDetail.ts, shared/hooks/useBookLoanStatus.ts, features/book-reviews/hooks.ts
Important behavior: categoryIds se serializa como parámetros repetidos; `search` solo si no está vacío. getBookLoanStatus siempre pide `auth: true` (con token el resultado depende del usuario; sin token el tipo prevé 'LoginRequired').
Notes: tres convenciones de respuesta conviven: `{ data }` desempaquetado (top-rated), `{ data, pagination }` completo (búsqueda) y objeto directo sin envoltorio (detalle, loan-status, ratings). Respetar la del endpoint al consumirlo.
```
**COMPLETE CURRENT FILE** — `src/api/endpoints/books.ts`

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

export function getBookById(bookId: number, signal?: AbortSignal) {
  return http<BookDetail>(`${BOOKS_PATH}/${bookId}`, signal)
}

export function getBookLoanStatus(bookId: number, signal?: AbortSignal) {
  return http<BookLoanStatus>(`${BOOKS_PATH}/${bookId}/loan-status`, signal, { auth: true })
}

export function getBookRatings(
  bookId: number,
  { page, pageSize }: BookRatingsParams,
  signal?: AbortSignal,
) {
  const query = new URLSearchParams({ page: String(page), pageSize: String(pageSize) })
  return http<BookRatings>(`${BOOKS_PATH}/${bookId}/ratings?${query.toString()}`, signal)
}
```

```text
==================================================
FILE: src/api/endpoints/categories.ts
==================================================
Purpose: Endpoint de categorías.
Exports: getCategories
Imports: http, ApiResponse, Category
Functions: getCategories (async, devuelve res.data)
API calls: GET /api/categories
Used by: shared/hooks/useCategories.ts
```
**COMPLETE CURRENT FILE** — `src/api/endpoints/categories.ts`

```ts
import { http } from '../http'
import type { ApiResponse } from '../types/common'
import type { Category } from '../types/category'

export async function getCategories(signal?: AbortSignal) {
  const res = await http<ApiResponse<Category[]>>('/api/categories', signal)
  return res.data
}
```

```text
==================================================
FILE: src/api/endpoints/ratings.ts
==================================================
Purpose: Endpoint para crear/actualizar la reseña de un usuario.
Exports: createRating(userId, body, signal?)
Imports: http, tipos CreateRatingBody/RatingResponse
Functions: createRating
API calls: POST /api/users/{userId}/ratings (JWT obligatorio en el cliente: auth: true)
Dependencies: api/http, api/types/rating
Used by: features/book-reviews/pages/BookReviewsPage.tsx
Important behavior: devuelve RatingResponse (incluye `message`, que la página muestra al usuario). USERS_PATH = '/api/users'.
Notes: la regla de negocio (una reseña por usuario y libro; requiere haber prestado el libro) vive en el backend: `[UNKNOWN / NEEDS VERIFICATION]` desde el front; el front solo traduce el error con getSubmitErrorMessage.
```
**COMPLETE CURRENT FILE** — `src/api/endpoints/ratings.ts`

```ts
import { http } from '../http'
import type { CreateRatingBody, RatingResponse } from '../types/rating'

const USERS_PATH = '/api/users'

export function createRating(userId: number, body: CreateRatingBody, signal?: AbortSignal) {
  return http<RatingResponse>(`${USERS_PATH}/${userId}/ratings`, signal, {
    auth: true,
    method: 'POST',
    body,
  })
}
```

```text
==================================================
FILE: src/api/types/book.ts
==================================================
Purpose: DTOs de libro.
Exports: Author, BookSummary, Book (extends BookSummary + description), BookDetail
Imports: Category
Used by: endpoints/books.ts, BookCard, BookGrid, BookList, HeroCarousel, BookInfoList, shared/utils/book.ts
Notes: BookDetail NO extiende BookSummary: tiene description, pageCount, publishedYear, publisher, language y no trae `rating`. `language` existe en el tipo pero ninguna pantalla lo muestra.
```
**COMPLETE CURRENT FILE** — `src/api/types/book.ts`

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

```text
==================================================
FILE: src/api/types/category.ts
==================================================
Purpose: DTO de categoría.
Exports: Category { id, name }
Used by: types/book.ts, endpoints/categories.ts, CategoryFilter, CategoryShortcuts
```
**COMPLETE CURRENT FILE** — `src/api/types/category.ts`

```ts
export interface Category {
  id: number
  name: string
}
```

```text
==================================================
FILE: src/api/types/common.ts
==================================================
Purpose: Envoltorios genéricos de respuesta.
Exports: ApiResponse<T> { data }, Pagination { page, pageSize, totalCount, totalPages }, PagedResponse<T> { data: T[], pagination }
Used by: endpoints/books.ts, endpoints/categories.ts, CategoriesPage
```
**COMPLETE CURRENT FILE** — `src/api/types/common.ts`

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

```text
==================================================
FILE: src/api/types/loan.ts
==================================================
Purpose: Estado de préstamo de un libro para el usuario actual.
Exports: LoanStatus ('LoginRequired' | 'Available' | 'BorrowedByYou' | 'Unavailable'), BookLoanStatus { bookId, userId: number|null, status, loanId: number|null, dueDate: string|null }
Used by: endpoints/books.ts, LoanButton, BookDetailPage
Notes: `userId` null equivale a "sin sesión"; BookReviewsPage lo usa para saber si hay usuario. `loanId` y `dueDate` aún no se muestran en la UI. Es un union de strings (no enum, por erasableSyntaxOnly).
```
**COMPLETE CURRENT FILE** — `src/api/types/loan.ts`

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

```text
==================================================
FILE: src/api/types/rating.ts
==================================================
Purpose: DTOs de reseñas.
Exports: BookRatingUser { idUser, fullName, comment|null, rating, createdAt, updatedAt }, BookRatings { averageRating, numberOfRatings, users }, CreateRatingBody { bookId, stars, comment|null }, RatingResponse { message, ratingId, bookId, userId, stars, comment|null, createdAt, updatedAt }
Used by: endpoints/books.ts, endpoints/ratings.ts, ReviewCard, ReviewList
Notes: BookRatings no trae `pagination`: el total de páginas se calcula con numberOfRatings. Las fechas son strings ISO.
```
**COMPLETE CURRENT FILE** — `src/api/types/rating.ts`

```ts
export interface BookRatingUser {
  idUser: number
  fullName: string
  comment: string | null
  rating: number
  createdAt: string
  updatedAt: string
}

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

```text
==================================================
FILE: src/config/env.ts
==================================================
Purpose: Variables de entorno tipadas.
Exports: env { apiUrl }
Imports: import.meta.env (Vite)
Dependencies: variable VITE_API_URL (fallback 'http://localhost:5031')
Used by: api/http.ts
```
**COMPLETE CURRENT FILE** — `src/config/env.ts`

```ts
export const env = {
  apiUrl: (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:5031',
}
```

## 27.2 Hooks y utilidades shared

```text
==================================================
FILE: src/shared/hooks/useFetch.ts
==================================================
Purpose: Hook de datos con cache (ver sección 13).
Exports: useFetch
Imports: useEffect/useState, fetchCached/readCache/readStale
Functions/Hooks: useFetch
Types (locales): State<T>, UseFetchOptions
API calls: las que haga el fetcher recibido
Dependencies: shared/utils/cache
Used by: useCategories, useBookDetail, useBookLoanStatus, useTopRatedBooks, useBookSearch, useBookRatings
Important behavior: ver sección 13. Dependencias del efecto = [key, enabled] (con eslint-disable intencional).
Notes: la `key` debe incluir todos los inputs del fetcher; con ttl>0 la petición no se cancela al desmontar.
```
**COMPLETE CURRENT FILE** — `src/shared/hooks/useFetch.ts`

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
  ttl?: number
  persist?: boolean
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

```text
==================================================
FILE: src/shared/hooks/useCategories.ts
==================================================
Purpose: Categorías con cache de 10 min (memoria + localStorage).
Exports: useCategories · Imports: getCategories, useFetch · Used by: HomePage, CategoriesPage
```
**COMPLETE CURRENT FILE** — `src/shared/hooks/useCategories.ts`

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

```text
==================================================
FILE: src/shared/hooks/useDebouncedValue.ts
==================================================
Purpose: Debounce genérico. Exports: useDebouncedValue<T>(value, delay=400). Used by: CategoriesPage
```
**COMPLETE CURRENT FILE** — `src/shared/hooks/useDebouncedValue.ts`

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

```text
==================================================
FILE: src/shared/hooks/useBookDetail.ts
==================================================
Purpose: Detalle de un libro con cache de 5 min (solo memoria).
Exports: useBookDetail(bookId, enabled = true)
Imports: getBookById, useFetch
Hooks: useFetch → key `book:${bookId}`, ttl 5 min, sin persist
Return value: { data: BookDetail | undefined, error, loading }
Used by: BookDetailPage, BookReviewsPage
Notes: antes vivía en features/book-detail/hooks.ts; se movió a shared porque lo usan dos features. Ambas páginas comparten la misma key, así que la 2.ª página sale del cache.
```
**COMPLETE CURRENT FILE** — `src/shared/hooks/useBookDetail.ts`

```ts
import { getBookById } from '../../api/endpoints/books'
import { useFetch } from './useFetch'

const BOOK_DETAIL_TTL = 5 * 60 * 1000 

export function useBookDetail(bookId: number, enabled = true) {
  return useFetch(`book:${bookId}`, (signal) => getBookById(bookId, signal), {
    enabled,
    ttl: BOOK_DETAIL_TTL,
  })
}
```

```text
==================================================
FILE: src/shared/hooks/useBookLoanStatus.ts
==================================================
Purpose: Estado de préstamo del libro para el usuario actual (y, de paso, si hay sesión).
Exports: useBookLoanStatus(bookId, enabled = true)
Imports: getBookLoanStatus, getAccessToken, useFetch
Hooks: useFetch → key `loan-status:${bookId}:${'auth'|'anon'}`, SIN cache (ttl 0)
Return value: { data: BookLoanStatus | undefined, error, loading }
Used by: BookDetailPage, BookReviewsPage
Important behavior: la parte 'auth'/'anon' de la key sale de si hay token en localStorage; no hay cache porque el estado cambia al prestar/devolver. BookReviewsPage usa `data.userId !== null` como "hay sesión".
Notes: movido desde features/book-detail/hooks.ts a shared.
```
**COMPLETE CURRENT FILE** — `src/shared/hooks/useBookLoanStatus.ts`

```ts
import { getBookLoanStatus } from '../../api/endpoints/books'
import { getAccessToken } from '../../api/token'
import { useFetch } from './useFetch'

export function useBookLoanStatus(bookId: number, enabled = true) {
  const session = getAccessToken() ? 'auth' : 'anon'

  return useFetch(
    `loan-status:${bookId}:${session}`,
    (signal) => getBookLoanStatus(bookId, signal),
    { enabled },
  )
}
```

```text
==================================================
FILE: src/shared/hooks/useGoBack.ts
==================================================
Purpose: Botón "volver" que respeta el historial.
Exports: useGoBack(fallback: string): () => void
Imports: useCallback, useNavigate
Hooks: useNavigate, useCallback
Return value: función estable (deps [navigate, fallback])
Used by: BookDetailPage (fallback '/'), BookReviewsPage (fallback `/books/${id}`)
Important behavior: si `window.history.state?.idx > 0` hace navigate(-1); si no (entrada directa por URL) navega al fallback.
Notes: depende del campo `idx` que React Router guarda en history.state (inferencia sobre la implementación del router).
```
**COMPLETE CURRENT FILE** — `src/shared/hooks/useGoBack.ts`

```ts
import { useCallback } from 'react'
import { useNavigate } from 'react-router'

export function useGoBack(fallback: string) {
  const navigate = useNavigate()

  return useCallback(() => {
    if (window.history.state?.idx > 0) navigate(-1)
    else navigate(fallback)
  }, [navigate, fallback])
}
```

```text
==================================================
FILE: src/shared/utils/book.ts
==================================================
Purpose: Helpers de presentación de libros.
Exports: toHttps, formatAuthors · Imports: tipo Author · Used by: BookCard, HeroCarousel, BookHero
```
**COMPLETE CURRENT FILE** — `src/shared/utils/book.ts`

```ts
import type { Author } from '../../api/types/book'

export const toHttps = (url: string) => url.replace(/^http:\/\//, 'https://')

export function formatAuthors(authors: Author[]) {
  if (authors.length === 0) return 'Autor desconocido'
  const [first, ...others] = authors
  return others.length > 0 ? `${first.name} +${others.length}` : first.name
}
```

```text
==================================================
FILE: src/shared/utils/cache.ts
==================================================
Purpose: Cache con TTL (Map en memoria + localStorage opcional) y de-duplicación de requests.
Exports: readCache, readStale, writeCache, invalidateCache, fetchCached
Imports: —
Types (locales): Entry<T>
Used by: useFetch (readCache, readStale, fetchCached). writeCache solo internamente; invalidateCache SIN usos.
Important behavior: prefijo localStorage 'lectio:cache:'; try/catch silencioso si localStorage falla o el JSON está corrupto; fetchCached comparte la promesa en vuelo por key y la elimina en finally.
Notes: no hay límite de tamaño ni limpieza de entradas antiguas.
```
**COMPLETE CURRENT FILE** — `src/shared/utils/cache.ts`

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
      // localStorage no disponible
    }
  }

  return undefined
}

export function readCache<T>(key: string, persist = false): T | undefined {
  const entry = readEntry<T>(key, persist)
  return entry && entry.expiresAt > Date.now() ? entry.data : undefined
}

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

```text
==================================================
FILE: src/shared/components/Navbar/navItems.ts
==================================================
Purpose: Fuente única de ítems de navegación.
Exports: NavItem, NAV_ITEMS (Home '/', History '/history', Categories '/categories', Profile '/profile' [mobileOnly, disabled])
Imports: IconType, MdCategory/MdHistory/MdHome/MdPerson
Used by: Navbar, MobileMenu
Notes: para añadir una sección de menú, editar aquí y registrar la ruta en routes/index.tsx.
```
**COMPLETE CURRENT FILE** — `src/shared/components/Navbar/navItems.ts`

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
  { label: 'Profile', to: '/profile', icon: MdPerson, mobileOnly: true, disabled: true },
]
```

## 27.3 Hooks y constantes de features

```text
==================================================
FILE: src/features/home/hooks.ts
==================================================
Purpose: useTopRatedBooks (TTL 2 min, persist). Imports: getTopRatedBooks, useFetch. Used by: HomePage
```
**COMPLETE CURRENT FILE** — `src/features/home/hooks.ts`

```ts
import { getTopRatedBooks } from '../../api/endpoints/books'
import { useFetch } from '../../shared/hooks/useFetch'

const TOP_RATED_TTL = 2 * 60 * 1000 

export function useTopRatedBooks(categoryId?: number, enabled = true) {
  return useFetch(
    `top-rated:${categoryId ?? 'all'}`,
    (signal) => getTopRatedBooks(categoryId, signal),
    { enabled, ttl: TOP_RATED_TTL, persist: true },
  )
}
```

```text
==================================================
FILE: src/features/home/constants.ts
==================================================
Purpose: Configuración visual/contenido de las categorías destacadas.
Exports: FEATURED_CATEGORIES, MOBILE_VISIBLE_CATEGORIES, DEFAULT_CATEGORY_ICON, CATEGORY_LABELS, CATEGORY_ICONS
Imports: IconType; íconos de react-icons/fa, io5 y md
Used by: CategoryShortcuts
Important behavior: los nombres deben coincidir con /api/categories (case-sensitive); CATEGORY_ICONS cubre 20 nombres (más que los 8 destacados); sin entrada → DEFAULT_CATEGORY_ICON.
Notes: aquí conviven fa/io5/md (excepción: el resto de la app usa solo md).
```
**COMPLETE CURRENT FILE** — `src/features/home/constants.ts`

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

```text
==================================================
FILE: src/features/categories/hooks.ts
==================================================
Purpose: useBookSearch (TTL 2 min, solo memoria, keepPrevious). Imports: searchBooks, BookSearchParams, useFetch. Used by: CategoriesPage
```
**COMPLETE CURRENT FILE** — `src/features/categories/hooks.ts`

```ts
import { searchBooks, type BookSearchParams } from '../../api/endpoints/books'
import { useFetch } from '../../shared/hooks/useFetch'

const SEARCH_TTL = 2 * 60 * 1000 

export function useBookSearch(params: BookSearchParams) {
  const key = `books:${params.search}|${params.categoryIds.join(',')}|${params.page}|${params.pageSize}`

  return useFetch(key, (signal) => searchBooks(params, signal), {
    ttl: SEARCH_TTL,
    keepPrevious: true,
  })
}
```

```text
==================================================
FILE: src/features/book-detail/constants.ts
==================================================
Purpose: Constantes de la pantalla de detalle.
Exports: BOOK_TABS (summary, basic-info, reviews), BookTabId (type), getTabId, getPanelId, DEFAULT_DESCRIPTION
Imports: —
Used by: BookTabs (BOOK_TABS, ids), BookDetailPage (DEFAULT_DESCRIPTION, ids, BookTabId)
Important behavior: BOOK_TABS es `as const` y BookTabId se deriva de él; los ids de pestaña/panel son `book-tab-{id}` / `book-panel-{id}`. DEFAULT_DESCRIPTION es un texto largo en inglés que se muestra cuando el libro no tiene descripción.
Notes: la pestaña 'reviews' existe en la barra pero navega a otra ruta.
```
**COMPLETE CURRENT FILE** — `src/features/book-detail/constants.ts`

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

```text
==================================================
FILE: src/features/book-reviews/constants.ts
==================================================
Purpose: Constantes de reseñas.
Exports: RATINGS_PAGE_SIZE = 5, MAX_COMMENT_LENGTH = 1000, STAR_VALUES = [1,2,3,4,5] as const
Used by: hooks.ts, BookReviewsPage (RATINGS_PAGE_SIZE); ReviewForm (MAX_COMMENT_LENGTH, STAR_VALUES)
Notes: MAX_COMMENT_LENGTH replica el límite del backend (según el comentario original del repo; verificar contra el backend).
```
**COMPLETE CURRENT FILE** — `src/features/book-reviews/constants.ts`

```ts
export const RATINGS_PAGE_SIZE = 5
export const MAX_COMMENT_LENGTH = 1000 // límite del backend
export const STAR_VALUES = [1, 2, 3, 4, 5] as const
```

```text
==================================================
FILE: src/features/book-reviews/hooks.ts
==================================================
Purpose: useBookRatings — reseñas paginadas de un libro.
Exports: useBookRatings(bookId, page, version, enabled = true)
Imports: getBookRatings, useFetch, RATINGS_PAGE_SIZE
Hooks: useFetch → key `ratings:${bookId}:${page}:${RATINGS_PAGE_SIZE}:v${version}`, ttl 1 min, solo memoria, keepPrevious: true
Return value: { data: BookRatings | undefined, error, loading }
Used by: BookReviewsPage
Important behavior: `version` se incrementa al publicar una reseña: cambia la key y fuerza una consulta nueva (el cache de 1 min no la oculta). keepPrevious mantiene la página anterior visible mientras llega la siguiente.
```
**COMPLETE CURRENT FILE** — `src/features/book-reviews/hooks.ts`

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

```text
==================================================
FILE: src/features/book-reviews/utils.ts
==================================================
Purpose: Helpers de la pantalla de reseñas.
Exports: formatReviewDate(iso), getSubmitErrorMessage(error)
Imports: ApiError
Functions: formatReviewDate (dd-mm-aaaa en UTC; fecha inválida → ''), getSubmitErrorMessage
Used by: ReviewCard (formatReviewDate), BookReviewsPage (getSubmitErrorMessage)
Important behavior: la fecha se formatea en UTC para que `…T00:00:00Z` no se corra al día anterior en zonas como UTC-5. getSubmitErrorMessage: ApiError 401/403 → 'Your session has expired. Please log in again.'; ApiError <500 con serverMessage → ese mensaje; cualquier otro caso → 'Something went wrong while saving your review. Please try again.' (en 5xx nunca se muestra el detalle del servidor).
Notes: textos en inglés.
```
**COMPLETE CURRENT FILE** — `src/features/book-reviews/utils.ts`

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

---

# 28. COMPLETE CSS FILES

Los **26 archivos `.css`** de `src/` (25 CSS Modules + `index.css`), con código completo insertado por script desde el repo.

## 28.1 `src/index.css`

Propósito: tokens + reset mínimo. Afecta: toda la app. Variables: ver secciones 5–8. Responsive: override de `--font-size-*` en ≥1024px.

**COMPLETE CURRENT FILE** — `src/index.css`

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

## 28.2 `src/App.css`

**Eliminado** en esta iteración (estaba vacío y no se importaba). Ya no existe en el repo.

## 28.3 CSS Modules de `shared/components`

**Navbar.module.css** — `.navbar`, `.inner`, `.menu` (hamburguesa), `.brand`, `.logo` (placeholder con `--color-primary-hover`), `.brandName`, `.right`, `.links`/`.linkActive`, `.user`, `.name`, `.avatar`. Responsive: ≥1024 oculta `.menu`, muestra `.brand` y `.links`, `.inner` a ancho completo con padding `clamp`. **Nuevo:** `.overlay` (header absoluto, `z-index: 10`, fondo transparente, degradado `linear-gradient(var(--overlay-dark), transparent)` en `::before` con `z-index: -1`; hamburguesa y nombre en `--color-text-inverse`); en ≥1024px `.overlay` vuelve a `position: static` con `--color-bg`.
**COMPLETE CURRENT FILE** — `src/shared/components/Navbar/Navbar.module.css`

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

**MobileMenu.module.css** — overlay (z 100), drawer (z 101, ancho `min(80vw,320px)`), header, lista, ítems, activo, deshabilitado, badge `.soon`. Oculto ≥1024px. Tokens: overlay-dark, bg, shadow-lg, primary-light, primary, surface, text-muted, disabled, radius-md/full, font-size-xs/md/lg.
**COMPLETE CURRENT FILE** — `src/shared/components/MobileMenu/MobileMenu.module.css`

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

**BookCard.module.css** — card (`.card`), portada con aspect-ratio 2/3 (3/4 en desktop) (`.cover`, `.placeholder`), título superpuesto con overlay y line-clamp 2 (`.title`), pill de rating (`.rating`), autor truncado (`.author`), botón (`.open`, `.openArrow`, `.openLabel`). Tokens: surface, radius-md/sm/full, shadow-md/sm, overlay-dark, text-inverse, primary(-hover), disabled, font-size-xs/sm, spacing-xs/sm/md. Responsive: ≥1024 oculta flecha, muestra texto, botón ancho pill. Literales: `40px` (placeholder), `28×22px`/`18px` (botón móvil), `2px`.
**COMPLETE CURRENT FILE** — `src/shared/components/BookCard/BookCard.module.css`

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

**BookGrid.module.css** — grilla 2→4(768)→5(1024) columnas, `.stale`, `.skeleton`, `.message`, `@keyframes pulse`, `transition: opacity .15s ease`. Tokens: spacing-md/lg, radius-md, disabled, text-muted, font-size-sm.
**COMPLETE CURRENT FILE** — `src/shared/components/BookGrid/BookGrid.module.css`

```css
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

**Pagination.module.css** — `.pagination`, `.page`/`.arrow` (32px, pill), `.current`, `.ellipsis`. Tokens: primary(-light), text, text-inverse, disabled, text-muted, radius-full.
**COMPLETE CURRENT FILE** — `src/shared/components/Pagination/Pagination.module.css`

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

**RatingStars.module.css** — capas `.base` (gris) y `.fill` (amarillo, `overflow:hidden`, ancho inline) y `.lg` (tamaño `--font-size-lg`, nuevo). Tokens: disabled, secondary, font-size-sm/lg.
**COMPLETE CURRENT FILE** — `src/shared/components/RatingStars/RatingStars.module.css`

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

**Skeleton.module.css** — `.skeleton` (fondo `--color-disabled`, `animation: pulse 1.2s ease-in-out infinite`) y `@keyframes pulse` (50% → opacity .5). Es la versión compartida del skeleton; sin tamaño propio (lo da el `className` del consumidor).
**COMPLETE CURRENT FILE** — `src/shared/components/Skeleton/Skeleton.module.css`

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

**Button.module.css** — `.button` (min-height 44px, padding sm/lg, borde 2px transparente, radio full, font-size sm, semibold), `.block` (`width: min(100%, 260px)`), `.primary` (fondo primary, texto inverse, `--shadow-md`; hover → primary-hover), `.secondary` (borde y texto primary sobre `--color-bg`; hover → primary-light), `.button:disabled` (fondo disabled, texto muted, sin sombra, `cursor: not-allowed`) y `:focus-visible` (outline 2px primary, offset 2px). Sin colores literales.
**COMPLETE CURRENT FILE** — `src/shared/components/Button/Button.module.css`

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

**BookHero.module.css** — `.hero` (relativo, 300px → 360px a 768 → `aspect-ratio: 3/4` con `max-height: calc(100vh - 160px)`, radio md y `--shadow-lg` a ≥1024; fondo `--color-primary-hover`), `.cover` (object-fit cover), `.placeholder` (ícono 64px), `.back` (círculo 36px con borde `--color-border`, `z-index: 1`, `top: calc(--spacing-xl * 2)` en mobile y `--spacing-md` en desktop), `.caption` (franja inferior `--overlay-dark`), `.title` (clamp 2 líneas), `.category`. Literales de tamaño: 36px, 24px, 64px. Sin colores literales.
**COMPLETE CURRENT FILE** — `src/shared/components/BookHero/BookHero.module.css`

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

## 28.4 CSS Modules de `features`

### 28.4.1 home

**HomePage.module.css** — `.main` (contenedor 960px; ≥1024 full + clamp), `.kicker`, `.title` (line-height 1.3), `.accent` (primary), `.lineBreak` (solo desktop).
**COMPLETE CURRENT FILE** — `src/features/home/pages/HomePage.module.css`

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

**HeroCarousel.module.css** — ramas `.mobile`/`.desktop` (alternadas por media query 1024), `.track` (scroll-snap, scrollbar oculta), `.slide`, `.bg`/`.shade`/`.cover`, `.badge`, `.caption`, `.dots`/`.dot`/`.dotActive`, `.skeleton`, `.media`, `.mediaCaption`, `.mediaTitle`, `.ratingPill`, `.info`, `.title`, `.by`, `.description` (clamp 5), `.actions`/`.action`. **Literales de color: `#111` ×2.** Alturas: 200px → 280px (768) → 320px (desktop `.media`/`.skeleton`).
**COMPLETE CURRENT FILE** — `src/features/home/components/HeroCarousel/HeroCarousel.module.css`

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

**CategoryShortcuts.module.css** — `.row` (grid con `--cols`/`--cols-desktop` inline), `.tile`, `.desktopOnly`, `.icon` (fondo disabled; seleccionado → primary-light + borde primary), `.label`. Responsive: ≥1024 `desktopOnly` pasa a flex, alturas con `clamp`.
**COMPLETE CURRENT FILE** — `src/features/home/components/CategoryShortcuts/CategoryShortcuts.module.css`

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

**BookList.module.css** — **(DEAD)** duplicado de BookGrid.module.css sin `.stale`; incluye su propia copia de `@keyframes pulse`.
**COMPLETE CURRENT FILE** — `src/features/home/components/BookList/BookList.module.css`

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

### 28.4.2 categories

**CategoriesPage.module.css** — `.main`, `.filters`, `.label`, `.searchBox`, `.input` (borde `--color-border`, radio `--radius-lg`, `:focus-visible` outline primary), `.searchIcon`, `.count`.
**COMPLETE CURRENT FILE** — `src/features/categories/pages/CategoriesPage.module.css`

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

**CategoryFilter.module.css** — `.root`, `.label`, `.trigger`, `.panel` (absoluto, z 20, max-height 360), `.list` (1→5 columnas), `.item`/`.itemSelected`, `.checkbox` (oculto accesible), `.check`, `.clear`, `.message`.
**COMPLETE CURRENT FILE** — `src/features/categories/components/CategoryFilter/CategoryFilter.module.css`

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

### 28.4.3 book-detail

**BookDetailPage.module.css** — `.main`, `.content` (columna, gap lg, `max-width: 960px` centrado), `.message`, `.actions`, `.state` (error centrado), `.link`, y skeletons `.heroSkeleton` (300px → 360px → `aspect-ratio 3/4` en desktop), `.tabsSkeleton`, `.textSkeleton`. En ≥1024: `.main` pasa a grid `minmax(0,4fr) minmax(0,7fr)` con gap xl y padding `clamp(var(--spacing-xl), 4vw, 80px)`; `.content` sin max-width. Tokens: spacing, radius-sm/md, text-muted, primary, font-size-sm, semibold. Sin colores literales.
**COMPLETE CURRENT FILE** — `src/features/book-detail/pages/BookDetailPage.module.css`

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

**BookTabs.module.css** — `.tabs` (flex, gap xl), `.tab` (sin borde ni fondo, color muted, regular), `.tabActive` (primary + semibold), `.tab:hover`, `.tab:focus-visible` (outline 2px primary, offset 1px). Tokens: spacing-xs/xl, text-muted, primary, font-weight-regular/semibold.
**COMPLETE CURRENT FILE** — `src/features/book-detail/components/BookTabs/BookTabs.module.css`

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

**BookInfoList.module.css** — `.list` (columna sin viñetas, gap md), `.row` (flex, gap md), `.icon` (24px, no se encoge), `.text` (font-size sm, `overflow-wrap: anywhere`). Literal: `24px`.
**COMPLETE CURRENT FILE** — `src/features/book-detail/components/BookInfoList/BookInfoList.module.css`

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

**ExpandableText.module.css** — `.text` (sm, line-height 1.5, justificado), `.clamped` (`-webkit-line-clamp: var(--lines)`, variable inline), `.toggle` (botón-texto primary semibold; hover → primary-hover + subrayado; focus-visible). Sin colores literales.
**COMPLETE CURRENT FILE** — `src/features/book-detail/components/ExpandableText/ExpandableText.module.css`

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

### 28.4.4 book-reviews

**BookReviewsPage.module.css** — `.main`, `.hero` (**oculto en mobile**, visible solo en ≥1024), `.content` (columna, `max-width: 960px`), `.header` (título centrado + botón `.back` absoluto de 32px, oculto en ≥1024), `.title` (primary, sm semibold; en desktop lg y alineado a la izquierda), `.actions`, `.notice` (`--color-success`), `.message` (muted), `.error` (`--color-error`), `.stale` (opacity .5), `.state`, `.link`, y skeletons (`.skeletonList`, `.heroSkeleton`, `.titleSkeleton`, `.summarySkeleton`, `.cardSkeleton`). En ≥1024: grid `minmax(0,4fr) minmax(0,7fr)` igual que el detalle. Literales de tamaño: 28px, 32px, 24px, 220px, 160px, 112px. Sin colores literales. **Primer consumidor de `--color-success` y `--color-error`.**
**COMPLETE CURRENT FILE** — `src/features/book-reviews/pages/BookReviewsPage.module.css`

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

**RatingSummary.module.css** — `.summary` (columna centrada), `.label` (muted, md, medium), `.average` (primary, `--font-size-3xl`, bold, line-height 1), `.stars` (pill con `--color-surface`, radio sm, `--shadow-md`), `.count` (muted, xs). **Único uso de `--font-size-3xl`.**
**COMPLETE CURRENT FILE** — `src/features/book-reviews/components/RatingSummary/RatingSummary.module.css`

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

**ReviewCard.module.css** — `.card` (surface, radio md, `--shadow-md`, padding md), `.header`, `.avatar` (círculo 36px, fondo `--color-primary-light`, ícono `--color-primary` de 22px), `.name` (md semibold), `.comment` (sm regular, `overflow-wrap: anywhere`), `.footer` (fecha + estrellas), `.date` (muted, xs, cursiva). Literales: 36px, 22px.
**COMPLETE CURRENT FILE** — `src/features/book-reviews/components/ReviewCard/ReviewCard.module.css`

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

**ReviewList.module.css** — `.list` (columna, gap md, sin viñetas ni márgenes).
**COMPLETE CURRENT FILE** — `src/features/book-reviews/components/ReviewList/ReviewList.module.css`

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

**ReviewForm.module.css** — `.form` (card surface, radio md, sombra md, gap md), `.title`, `.field`, `.label`, `.stars` (flex, gap xs), `.star` (botón-ícono 32px, color `--color-disabled`), `.starActive` (`--color-secondary`), `.textarea` (borde `--color-border`, radio sm, `resize: vertical`), `.counter` (muted, xs, a la derecha), `.error` (`--color-error`), `.actions` (flex-wrap). `:focus-visible` (outline 2px primary) en `.star` y `.textarea`. Literal: 32px. Sin colores literales.
**COMPLETE CURRENT FILE** — `src/features/book-reviews/components/ReviewForm/ReviewForm.module.css`

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

### 28.4.5 login

**LoginPage.module.css** — `.main` (columna centrada, `max-width: 480px`, texto centrado), `.title` (primary, xl, bold), `.message` (muted, sm), `.link` (primary, sm, semibold). Sin colores literales.
**COMPLETE CURRENT FILE** — `src/features/login/pages/LoginPage.module.css`

```css
.main {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-md);
  max-width: 480px;
  margin: 0 auto;
  padding: var(--spacing-xl) var(--spacing-md);
  text-align: center;
}

.title {
  margin: 0;
  color: var(--color-primary);
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-bold);
}

.message {
  margin: 0;
  color: var(--color-text-muted);
  font-size: var(--font-size-sm);
}

.link {
  color: var(--color-primary);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
}
```

## 28.5 Archivos de configuración (código completo)

**index.html**
**COMPLETE CURRENT FILE** — `index.html`

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

**package.json**
**COMPLETE CURRENT FILE** — `package.json`

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

**tsconfig.json**
**COMPLETE CURRENT FILE** — `tsconfig.json`

```json
{
  "files": [],
  "references": [
    { "path": "./tsconfig.app.json" },
    { "path": "./tsconfig.node.json" }
  ]
}
```

**tsconfig.app.json**
**COMPLETE CURRENT FILE** — `tsconfig.app.json`

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

**tsconfig.node.json**
**COMPLETE CURRENT FILE** — `tsconfig.node.json`

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

**vite.config.ts**
**COMPLETE CURRENT FILE** — `vite.config.ts`

```ts
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
})
```

**eslint.config.js**
**COMPLETE CURRENT FILE** — `eslint.config.js`

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

**.gitignore**
**COMPLETE CURRENT FILE** — `.gitignore`

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

**`.env`** — existe en el zip pero **no se incluye** (puede contener valores sensibles). Solo se cita el nombre de la variable usada: `VITE_API_URL`.

---

# 29. FILE DEPENDENCY MAP

```text
main.tsx
 ├── index.css (global)
 └── routes/index.tsx (router)
      └── AppLayout (useMatch → overlay)
           ├── Navbar
           │    ├── MobileMenu ── navItems (NAV_ITEMS)
           │    └── navItems (NAV_ITEMS)
           └── <Outlet/>
                ├── "/"            HomePage
                │    ├── HeroCarousel ── RatingStars, utils/book
                │    ├── CategoryShortcuts ── home/constants
                │    ├── BookGrid ── BookCard ── RatingStars, utils/book
                │    ├── useCategories ── useFetch ── utils/cache
                │    │                   └── api/endpoints/categories ── api/http ── config/env
                │    └── useTopRatedBooks ── useFetch
                │                           └── api/endpoints/books.getTopRatedBooks ── api/http
                ├── "/categories"  CategoriesPage
                │    ├── CategoryFilter
                │    ├── BookGrid ── BookCard ── RatingStars, utils/book
                │    ├── Pagination
                │    ├── useDebouncedValue
                │    ├── useCategories ── useFetch ── utils/cache
                │    └── useBookSearch ── useFetch
                │                         └── api/endpoints/books.searchBooks ── api/http
                ├── "/history"     HistoryPage (stub)
                ├── "/books/:bookId" BookDetailPage
                │    ├── BookHero ── utils/book
                │    ├── Skeleton
                │    ├── BookTabs ── book-detail/constants
                │    ├── ExpandableText
                │    ├── BookInfoList
                │    ├── LoanButton ── Button
                │    ├── useGoBack ── react-router
                │    ├── useBookDetail ── useFetch
                │    │                     └── api/endpoints/books.getBookById ── api/http
                │    └── useBookLoanStatus ── useFetch, api/token
                │                              └── api/endpoints/books.getBookLoanStatus ── api/http (auth)
                ├── "/books/:bookId/reviews" BookReviewsPage
                │    ├── BookHero, Skeleton, Button, Pagination
                │    ├── RatingSummary ── RatingStars
                │    ├── ReviewList ── ReviewCard ── RatingStars, book-reviews/utils (formatReviewDate)
                │    ├── ReviewForm ── Button, book-reviews/constants
                │    ├── useGoBack, useBookDetail, useBookLoanStatus   (compartidos con el detalle)
                │    ├── useBookRatings ── useFetch
                │    │                      └── api/endpoints/books.getBookRatings ── api/http
                │    ├── createRating ── api/endpoints/ratings ── api/http (POST + auth)
                │    └── book-reviews/utils (getSubmitErrorMessage) ── api/http (ApiError)
                └── "/login"       LoginPage (provisional)

api/http ── api/token (getAccessToken)

(Huérfano) BookList → BookCard      [nadie lo importa]
```

---

# 30. COMPONENT REUSE MAP

```text
BookCard   (afecta Home + Categories)
        ↓
-------------------------
Used by:
- BookGrid → HomePage (grilla), CategoriesPage (resultados)
- BookList (muerto)

BookGrid   (afecta Home + Categories)
        ↓
Used by:
- HomePage
- CategoriesPage

RatingStars   (afecta Home, Categories, Hero, Reseñas)
        ↓
Used by:
- BookCard
- HeroCarousel (pill de rating en desktop)
- RatingSummary (size="lg")
- ReviewCard (size="lg")

Button   (afecta Detalle + Reseñas)
        ↓
Used by:
- LoanButton → BookDetailPage
- BookReviewsPage ("Add Review")
- ReviewForm (Submit, Cancel)

BookHero   (afecta Detalle + Reseñas en desktop)
        ↓
Used by:
- BookDetailPage (h1)
- BookReviewsPage (h2, solo visible en ≥1024px)

Skeleton
        ↓
Used by:
- BookDetailPage
- BookReviewsPage

Pagination
        ↓
Used by:
- CategoriesPage
- BookReviewsPage

Navbar   (afecta TODAS las rutas)
        ↓
Used by:
- AppLayout → todas las páginas (overlay solo en /books/:bookId)
 └── MobileMenu (solo Navbar)
 └── NAV_ITEMS (Navbar + MobileMenu: cambiar aquí afecta ambos menús)

useFetch + utils/cache   (afecta TODOS los datos de la app)
        ↓
Used by:
- useCategories (Home, Categories)
- useTopRatedBooks (Home)
- useBookSearch (Categories)
- useBookDetail (Detalle, Reseñas)
- useBookLoanStatus (Detalle, Reseñas)
- useBookRatings (Reseñas)

useBookDetail / useBookLoanStatus / useGoBack   (afectan Detalle + Reseñas)

useCategories
        ↓
Used by: HomePage, CategoriesPage

utils/book (toHttps, formatAuthors)
        ↓
Used by: BookCard, HeroCarousel, BookHero

api/http + api/token   (afectan TODAS las llamadas)
        ↓
Used by: endpoints/books, endpoints/categories, endpoints/ratings (token solo si auth: true)

index.css (tokens)  → afecta TODO el CSS
```

**Modificar con especial cuidado:** `BookCard`, `BookGrid`, `Button`, `BookHero`, `RatingStars`, `useFetch`, `cache.ts`, `navItems.ts`, `index.css`, `api/http.ts`, `api/types/book.ts` (varios afectan a todos los consumidores). Al tocar `useBookDetail`, `useBookLoanStatus` o `useGoBack` se afectan **dos** pantallas.

---

# 31. VISUAL RULES — MUST FOLLOW

**Colores (solo vía `var(--…)`):**
`--color-primary #236A34` (acción/activo/acento) · `--color-primary-hover #1B4F27` · `--color-primary-light #E3F0E6` (fondo suave/seleccionado) · `--color-secondary #F3F02B` (estrellas: relleno de `RatingStars` y estrella activa del formulario) · `--color-bg #FFFFFF` · `--color-surface #F9FAFE` (cards) · `--color-border #747474` (inputs) · `--color-text #000000` · `--color-text-muted #6B7280` · `--color-disabled #E0E0E0` (inactivo/skeleton) · `--color-text-inverse #FFFFFF` · `--overlay-dark rgba(0,0,0,.55)` · estados: `--color-error #FF1226` (errores de reseñas), `--color-success #236A34` (aviso de éxito de reseñas), `--color-warning #F59E0B` y `--color-info #3B82F6` (definidos, sin uso).

**Tipografía:** Poppins (`var(--font-family-base)`) · pesos 400/500/600/700 · tamaños solo con `--font-size-xs|sm|md|lg|xl|3xl` (ya cambian solos a ≥1024px; **no** escribir media queries para tamaños de fuente). `2xl` existe sin uso.

**Spacing:** `--spacing-xs 4 · sm 8 · md 16 · lg 24 · xl 32`. Padding de página: `0 var(--spacing-md)` (+`--spacing-xl` abajo); desktop: `clamp(var(--spacing-xl), 4vw, 80px)` horizontal. Gap de grillas: `md` (mobile) / `lg` (desktop).

**Borders/Radius:** `--radius-sm 8` (badges, portada, textarea) · `md 12` (cards, incl. reseñas) · `lg 16` (inputs, trigger) · `full 999` (pills, avatares, dots). Borde de input: `1px solid var(--color-border)`.

**Sombras:** `sm` (pill de rating, botón volver del hero) · `md` (cards, `Button` primario, botones del hero, `RatingSummary`) · `lg` (drawer, dropdown, portada del hero).

**Botones:** usar el componente `Button`. Primario = relleno `--color-primary` + texto `--color-text-inverse` + `--radius-full` + semibold + `--shadow-md`; hover → `--color-primary-hover`; secundario = borde y texto `--color-primary` sobre `--color-bg`, hover `--color-primary-light`; disabled → `--color-disabled` + `--color-text-muted` + `cursor:not-allowed`; `block` = `width: min(100%, 260px)`. Botones icono: `border:0; background:none` (con `aria-label`). Todos `type="button"` salvo el submit del formulario.

**Selección/activo:** `--color-primary-light` + `--color-primary` (tiles/drawer) o `--color-primary` + `--color-text-inverse` (filtro/página actual); pestañas activas: texto `--color-primary` + semibold.

**Foco:** `:focus-visible { outline: 2px solid var(--color-primary); outline-offset: 1px | 2px }` (patrón existente; el de `Button` usa offset 2px).

**Skeleton/loading:** componente `Skeleton` (fondo `--color-disabled` + `pulse 1.2s ease-in-out infinite`); contenedor con `aria-busy`. No añadir más copias de `@keyframes pulse`.

**Cards de contenido (reseñas, formulario, libros):** `--color-surface` + `--radius-md` + `--shadow-md` + padding `--spacing-md` (o `--spacing-sm` en BookCard).

**Layout/breakpoints:** mobile-first; `@media (min-width: 768px)` y `(min-width: 1024px)` únicamente; contenedor `max-width: 960px; margin: 0 auto` en <1024 y ancho completo en ≥1024; grilla de libros 2/4/5 columnas; detalle y reseñas = grid `minmax(0,4fr) minmax(0,7fr)` en ≥1024 con `BookHero` a la izquierda.

**Íconos:** `react-icons/md` por defecto, tamaño con `font-size` en el contenedor; `aria-hidden="true"` si son decorativos.

**Z-index existentes:** -1 (degradado del Navbar overlay), 1 (botón volver de BookHero), 10 (Navbar overlay), 20 (dropdown), 100 (overlay del drawer), 101 (drawer): no inventar otros sin revisar.

**Transiciones:** `.15s–.25s ease`; respetar que no hay token.

**Idioma de textos:** mezcla actual EN (títulos/labels visibles y mensajes de reseñas/detalle) / ES (aria y mensajes por defecto de listas); seguir al componente vecino.

---

# 32. CURRENT FRONTEND STATUS

### Completed (hechos verificados)
- Entry + router con layout (`main.tsx`, `routes/`), 6 rutas.
- Navbar responsive + drawer móvil con navegación activa; modo `overlay` sobre la portada del detalle.
- Home: hero (carrusel móvil / destacado desktop), atajos de categorías con toggle, grilla top-rated por categoría.
- Categories: búsqueda con debounce, filtro multi-categoría, paginación, estado en URL, contador de resultados.
- **Detalle de libro:** portada, pestañas Summary / Basic Info, texto expandible, datos básicos, botón de préstamo que refleja el estado del backend, estados de carga y error.
- **Reseñas:** promedio, lista paginada (5 por página), formulario (estrellas 1–5 + comentario opcional de hasta 1000 caracteres) con POST autenticado, mensajes de error y de éxito, recarga de lista y promedio tras publicar.
- Capa de datos: `http` (GET/POST, JWT opcional, `ApiError`), 6 endpoints de lectura + 1 de escritura, `useFetch` con cache (TTL, persistencia, de-dup, stale-on-error, keepPrevious).
- Componentes compartidos nuevos: `Button`, `BookHero`, `Skeleton`; hooks compartidos nuevos: `useBookDetail`, `useBookLoanStatus`, `useGoBack`.
- Tokens de diseño centralizados y CSS Modules por componente (25 módulos).
- Build limpio: `tsc -b`, `eslint .` y `vite build` sin errores (177 módulos).

### In Progress (stubs/placeholder en el código)
- `HistoryPage` (solo `<h1>`).
- `LoginPage`: página provisional "coming soon".
- Perfil: ítem "Profile" deshabilitado ("Soon"), sin ruta.
- Borrow / Deliver: el botón existe y muestra el estado, pero no hay acción ni endpoint en el front (solo `LoginRequired` navega a `/login`).
- Logo del Navbar (TODO) y avatar/nombre de usuario (valores por defecto).

### Known Issues (hechos)
- `README.md` describe una estructura que no existe.
- Código muerto: `features/home/components/BookList/`; `invalidateCache` sin usos.
- `index.html`: `<title>lectiofrontend</title>` (nombre de plantilla) y `<link href="/favicon.svg">` sin archivo en el zip (no hay `public/`).
- `.env` no está en `.gitignore`.
- **Nada escribe el token JWT** (`lectio:accessToken`): sin login no hay forma de obtener sesión desde la app; los botones que la exigen llevan a `/login` (provisional).
- La paginación de reseñas no está en la URL (al recargar vuelve a la página 1); `BookReviewsPage` y `BookDetailPage` repiten el bloque de error/"Back to home".
- `ReviewForm` no precarga la reseña previa del usuario (el POST crea o actualiza según el backend).
- Sin 404 / `errorElement` / ErrorBoundary.
- El error de `useCategories` y el del hero no se muestran en UI.
- 5 tokens sin uso: `--color-warning`, `--color-info`, `--color-secondary-hover`, `--color-text-inverse-muted` y `--font-size-2xl`.
- Idiomas mezclados en textos de UI.
- Sin tests (no hay runner en `package.json`).

### Technical Debt (hechos)
- `@keyframes pulse` y estilos de skeleton en 4 sitios: `Skeleton` (compartido) y copias en `BookGrid`, `BookList`, `HeroCarousel`.
- Patrón `padding: clamp(var(--spacing-xl), 4vw, 80px)` y contenedor `max-width: 960px` repetidos en Navbar/HomePage/CategoriesPage/BookDetailPage/BookReviewsPage; el grid 4fr/7fr está duplicado entre detalle y reseñas.
- `Button` existe pero `BookCard .open` y `HeroCarousel .action` siguen con su propio botón pill.
- `HeroCarousel` duplica el JSX de portada difuminada en dos ramas y mantiene `active` desde dos fuentes (scroll y botón).
- `#111` y `border-radius: 4px` literales en HeroCarousel.
- Respuestas sin validación en runtime; tres convenciones de envoltorio de respuesta (`{data}`, `{data,pagination}`, objeto directo).
- `useGoBack` depende de `window.history.state.idx` (detalle interno de React Router) **(inferencia)**.
- Dos `eslint-disable react-hooks/exhaustive-deps`.
- Comillas/`;` inconsistentes en `home/constants.ts`.
- `searchBooks` y `getTopRatedBooks` difieren en si desempaquetan `data`.

### Missing Features (no existen en el código)
Login/registro y manejo de sesión, acciones de préstamo (Borrow/Deliver), historial, perfil, favoritos/reservas, página 404, i18n, dark mode, tests.

### Possible Improvements (**sugerencias, no hechos**)
- Eliminar `BookList` (con confirmación del usuario) y reescribir/borrar el README.
- Reemplazar las copias de skeleton por `Skeleton` y los botones pill de `BookCard`/`HeroCarousel` por `Button`; extraer un `PageContainer`/layout de dos columnas común a detalle y reseñas; extraer el bloque de error de libro no encontrado.
- Cuando se implemente el login: función de escritura/borrado del token en `api/token.ts`, estado de usuario (p. ej. un contexto) y `userName`/`avatarUrl` reales en el Navbar.
- Añadir token para el fondo oscuro del hero.
- Mostrar errores de categorías/hero; añadir `errorElement` y ruta `*`.
- Añadir `.env` a `.gitignore`; crear `public/favicon.svg` y ajustar `<title>`.
- Resincronizar `text` con `q` de la URL ante navegación atrás/adelante; guardar la página de reseñas en la URL.
- Unificar idioma de la UI; focus-trap en el drawer; navegación con flechas en `BookTabs` y en las estrellas; `prefers-reduced-motion`.

---

# 33. QUICK START FOR THE NEXT CLAUDE

# Instructions for the next Claude

1. **Lee primero este documento completo**, luego abre el repo (o `FRONTEND_FILES.md`, que trae el código literal).
2. **El código del repositorio es la fuente de verdad.** Si algo aquí difiere del repo, manda el repo (y avisa de la diferencia).
3. **Respeta las convenciones visuales existentes** (sección 31).
4. **No hardcodees colores.** Usa `var(--color-*)`/`var(--overlay-dark)`. Si necesitas uno nuevo, añádelo primero a `:root` en `src/index.css`.
5. **Reutiliza componentes antes de crear nuevos** (secciones 9, 10 y 30): `BookGrid`/`BookCard` para libros, `Button`, `Skeleton`, `BookHero`, `RatingStars`, `Pagination`, `NAV_ITEMS`.
6. **Revisa `src/index.css` antes de implementar UI** (tokens de color, spacing, radius, shadow, tipografía).
7. **Revisa `src/shared/components/` antes de crear componentes nuevos**; lo exclusivo de una feature va en `features/<x>/components/`.
8. **Respeta la arquitectura actual:** `api/ → shared/hooks (useFetch) → features/hooks.ts → page → componentes`. Las páginas llaman hooks y navegan; los componentes reciben props.
9. **Verifica el código antes de asumir que algo existe.** No te fíes del `README.md` ni de `BookList`.
10. **No cambies decisiones arquitectónicas sin justificación** ni agregues librerías si ya existe solución.

**Reglas específicas descubiertas en el análisis:**

- **Datos (lectura):** nuevas llamadas = función en `api/endpoints/*.ts` (con `signal?: AbortSignal` como último parámetro y `http<T>()`), tipos en `api/types/`, hook con `useFetch(key, fetcher, {ttl, persist?, keepPrevious?})`. **La `key` debe incluir todos los parámetros** (y lo que cambie el resultado: `version`, sesión). Elige TTL deliberadamente (categorías 10 min; detalle 5 min; listas 2 min; reseñas 1 min; préstamo sin cache). No uses `fetch`/`useEffect` a mano.
- **Datos (escritura):** `http<T>(path, signal, { auth: true, method: 'POST', body })` desde una función en `api/endpoints/`; la página maneja `submitting`/error con `useState` y traduce errores con `ApiError` (`status`, `serverMessage`). Para refrescar una lista tras mutar, sigue el patrón `version` de `useBookRatings`.
- **Endpoints existentes:** `GET /api/books/top-rated`, `GET /api/books`, `GET /api/books/{id}`, `GET /api/books/{id}/loan-status` (JWT), `GET /api/books/{id}/ratings`, `GET /api/categories`, `POST /api/users/{userId}/ratings` (JWT). Para cualquier otro (login, préstamo, historial) **confirma con el backend**: `[UNKNOWN / NEEDS VERIFICATION]`. `http()` solo soporta GET y POST.
- **Sesión:** `getAccessToken()` solo lee `localStorage['lectio:accessToken']`; no hay login ni escritura del token. La UI deduce la sesión de `useBookLoanStatus().data.userId`. Los botones que exigen sesión navegan a `/login` (provisional).
- **Tipos:** `import type` siempre para tipos (`verbatimModuleSyntax`); sin `enum`/`namespace` (`erasableSyntaxOnly`); TS estricto, `noUnusedLocals/Parameters` (el build falla si sobra algo).
- **Componentes:** carpeta `Nombre/Nombre.tsx` + `Nombre.module.css`, **named export**, `interface NombreProps` local, estilos con CSS Modules y clases camelCase. Rutas de import relativas (no hay alias `@/`).
- **Estado:** `useState` local; estado compartible de listados en la **URL** (`useSearchParams`) como en CategoriesPage; no añadir Redux/Zustand/Context sin justificarlo (el login futuro sí podría justificar uno).
- **Responsive:** mobile-first, solo `min-width: 768px` y `1024px`; los tamaños de fuente cambian solos por tokens.
- **Accesibilidad:** mantener el nivel actual (`aria-label` en botones de ícono, `aria-pressed/expanded/current/busy/checked/selected`, `role="alert"` en errores, `role="status"` en avisos, `alt` en imágenes, `type="button"`).
- **Idioma:** los textos visibles están en inglés y los `aria`/mensajes por defecto en español; sigue al componente vecino y pregunta si hay duda.
- **Home:** `FEATURED_CATEGORIES` debe coincidir con los nombres de `/api/categories` (case-sensitive).
- **Nav:** añadir pantallas = ruta en `routes/index.tsx` + ítem en `NAV_ITEMS` (si va en el menú). Si la pantalla necesita Navbar superpuesto, ajustar el `useMatch` de `AppLayout`.
- **Pantallas de libro:** `BookDetailPage` y `BookReviewsPage` comparten `useBookDetail`, `useBookLoanStatus`, `useGoBack`, `BookHero`, `Skeleton` y `Button`; un cambio en cualquiera afecta a ambas.
- **Validación obligatoria tras cambios:** `npm run build` (incluye `tsc -b`) y `npm run lint`. Ambos pasan hoy.
- **Antes de borrar código muerto** (`BookList`) pide confirmación al usuario.
- **Seguridad:** no escribas valores de `.env`, tokens ni secretos en archivos versionados.

---

# 34. FINAL QUALITY CHECK

- [x] Todos los features importantes están documentados (home, categories, book-detail, book-reviews, login provisional, history stub; profile inexistente) — secciones 1, 11, 12.
- [x] Todos los shared components están documentados (BookCard, BookGrid, BookHero, Button, Skeleton, Pagination, RatingStars, Navbar, MobileMenu + NAV_ITEMS) — secciones 9, 10, 26.2.
- [x] Todos los custom hooks están documentados (useFetch, useCategories, useDebouncedValue, useBookDetail, useBookLoanStatus, useGoBack, useTopRatedBooks, useBookSearch, useBookRatings) — sección 13.
- [x] `index.css` está documentado completamente y su código incluido — secciones 5–8 y 28.1.
- [x] Todos los colores están identificados (18 variables de color/overlay; 2 literales `#111` fuera de tokens) — secciones 5 y 8.
- [x] Todas las variables CSS importantes están documentadas (colores, fuentes, pesos, tamaños, radios, spacing, sombras, overlay) — secciones 5–8.
- [x] Las tipografías están documentadas — sección 6.
- [x] El árbol del proyecto está documentado (real, con código muerto marcado) — sección 3.
- [x] Las rutas están documentadas (6 rutas; sin auth real, 404 ni redirects) — sección 12.
- [x] Los servicios están documentados (http + token + 3 archivos de endpoints con 7 funciones) — sección 15.
- [x] Los tipos importantes están documentados — sección 16.
- [x] Los archivos `.tsx` (30/30) están documentados — sección 26.
- [x] Los archivos `.ts` (27/27 en `src/`) están documentados — sección 27.
- [x] Los archivos críticos contienen su código completo (todos los `.ts`, `.tsx`, `.css` de `src/` y la configuración, insertados por script desde el repo) — secciones 26–28.
- [x] Las convenciones están documentadas — sección 22.
- [x] Las reglas visuales están documentadas — sección 31.
- [x] Los componentes compartidos están identificados con distinción shared real vs. por ubicación — sección 10.
- [x] Las dependencias importantes están documentadas (mapa + reuse map) — secciones 29–30.
- [x] No se incluyeron secretos (`.env` omitido; solo se cita el nombre `VITE_API_URL`; ningún valor de token).
- [x] No se inventó información: lo no verificable está marcado `[UNKNOWN / NEEDS VERIFICATION]` (stack del backend, reglas de negocio del backend, favicon fuera del zip) y las deducciones están marcadas como **(inferencia)**.

**Verificaciones ejecutadas sobre este snapshot:** `tsc -b` → 0 errores · `eslint .` → 0 errores · `vite build` → OK (177 módulos) · mapa de imports, usos de tokens CSS, colores literales y exports sin uso verificados con scripts · el código de las secciones 26–28 se insertó por script desde el repo y se re-comparó contra el repo tras generar el documento.
