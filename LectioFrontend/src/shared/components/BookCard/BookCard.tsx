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