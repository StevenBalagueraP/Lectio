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