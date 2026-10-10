import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router'
import { createLoan } from '../../../api/endpoints/loans'
import { BookHero } from '../../../shared/components/BookHero/BookHero'
import { Button } from '../../../shared/components/Button/Button'
import { Skeleton } from '../../../shared/components/Skeleton/Skeleton'
import { useBookDetail } from '../../../shared/hooks/useBookDetail'
import { useBookLoanStatus } from '../../../shared/hooks/useBookLoanStatus'
import { useGoBack } from '../../../shared/hooks/useGoBack'
import { useSession } from '../../../shared/hooks/useSession'
import { LOAN_PERIOD_DAYS } from '../constants'
import { getLoanErrorMessage } from '../utils'
import styles from './LoanPage.module.css'

function formatDate(d: Date): string {
  return d.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })
}

export function LoanPage() {
  const navigate = useNavigate()
  const { bookId } = useParams()
  const id = Number(bookId)
  const validId = Number.isInteger(id) && id > 0

  const user = useSession()
  const handleBack = useGoBack(`/books/${id}`)

  // Dates are fixed at mount time — not recalculated on re-render
  const [checkoutDate] = useState(() => new Date())
  const [dueDate] = useState(() => {
    const d = new Date()
    d.setDate(d.getDate() + LOAN_PERIOD_DAYS)
    return d
  })

  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState<string | null>(null)
  const [notice, setNotice] = useState<string | null>(null)

  // Hooks must be called before any early returns
  const book = useBookDetail(id, validId && user !== null)
  const loan = useBookLoanStatus(id, validId && user !== null)

  // Redirect unauthenticated users to login; session is read from localStorage so it's synchronous
  useEffect(() => {
    if (user === null) {
      navigate('/login', { state: { from: `/books/${id}` }, replace: true })
    }
  }, [user, navigate, id])

  // While redirecting, render nothing to avoid a flash of content
  if (user === null) return null

  if (book.loading) {
    return (
      <main className={styles.main} aria-busy="true">
        <Skeleton className={styles.heroSkeleton} />
        <div className={styles.content}>
          <Skeleton className={styles.textSkeleton} />
          <Skeleton className={styles.dateSkeleton} />
          <Skeleton className={styles.dateSkeleton} />
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

  const handleBorrow = async () => {
    if (!user) return
    setSubmitting(true)
    setSubmitError(null)
    try {
      const res = await createLoan(user.id, {
        bookId: id,
        checkoutDate: checkoutDate.toISOString(),
        dueDate: dueDate.toISOString(),
      })
      setNotice(res.message)
    } catch (error) {
      setSubmitError(getLoanErrorMessage(error))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <main className={styles.main}>
      <BookHero
        title={detail.title}
        imageUrl={detail.imageUrl}
        onBack={handleBack}
        category={detail.categories[0]?.name}
      />

      <div className={styles.content}>
        <h2 className={styles.sectionTitle}>Date</h2>

        <dl className={styles.dateList}>
          <div className={styles.dateRow}>
            <dt className={styles.dateLabel}>Checkout date</dt>
            <dd className={styles.dateValue}>{formatDate(checkoutDate)}</dd>
          </div>
          <div className={styles.dateRow}>
            <dt className={styles.dateLabel}>Due Date</dt>
            <dd className={styles.dateValue}>{formatDate(dueDate)}</dd>
          </div>
        </dl>

        {loan.data?.status !== 'Available' && !loan.loading && (
          <p role="alert" className={styles.warning}>This book is not available for borrowing.</p>
        )}

        {notice && <p role="status" className={styles.notice}>{notice}</p>}

        {submitError && <p role="alert" className={styles.error}>{submitError}</p>}

        <div className={styles.actions}>
          {notice ? (
            <Button block onClick={() => navigate(`/books/${id}`)}>Back to book</Button>
          ) : (
            <Button
              block
              disabled={submitting || loan.data?.status !== 'Available' || loan.loading}
              aria-busy={submitting}
              onClick={handleBorrow}
            >
              {submitting ? 'Borrowing…' : 'Borrow'}
            </Button>
          )}
        </div>
      </div>
    </main>
  )
}
