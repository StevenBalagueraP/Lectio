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