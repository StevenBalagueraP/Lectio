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
