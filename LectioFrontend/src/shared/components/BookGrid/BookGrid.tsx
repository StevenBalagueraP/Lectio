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