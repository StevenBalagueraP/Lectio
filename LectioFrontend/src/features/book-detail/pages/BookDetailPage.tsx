import { useEffect, useState } from 'react'
import { Link, useLocation, useNavigate, useParams } from 'react-router'
import type { LoanStatus } from '../../../api/types/loan'
import { returnLoan } from '../../../api/endpoints/loans'
import { BookHero } from '../../../shared/components/BookHero/BookHero'
import { ConfirmDialog } from '../../../shared/components/ConfirmDialog/ConfirmDialog'
import { Skeleton } from '../../../shared/components/Skeleton/Skeleton'
import { useBookDetail } from '../../../shared/hooks/useBookDetail'
import { useBookLoanStatus } from '../../../shared/hooks/useBookLoanStatus'
import { useGoBack } from '../../../shared/hooks/useGoBack'
import { getLoanErrorMessage } from '../../loan/utils'
import { BookInfoList } from '../components/BookInfoList/BookInfoList'
import { BookTabs } from '../components/BookTabs/BookTabs'
import { ExpandableText } from '../components/ExpandableText/ExpandableText'
import { LoanButton } from '../components/LoanButton/LoanButton'
import { DEFAULT_DESCRIPTION, getPanelId, getTabId, type BookTabId } from '../constants'
import styles from './BookDetailPage.module.css'

interface LocationState {
  openReturnDialog?: boolean
}

export function BookDetailPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const { bookId } = useParams()
  const id = Number(bookId)
  const validId = Number.isInteger(id) && id > 0

  // Derive initial dialog state directly from location.state (no setState in useEffect)
  const locationState = location.state as LocationState | null
  const fromHistory = locationState?.openReturnDialog === true

  const [tab, setTab] = useState<BookTabId>('summary')
  const [deliverOpen, setDeliverOpen] = useState(fromHistory)
  const [deliverSubmitting, setDeliverSubmitting] = useState(false)
  const [deliverError, setDeliverError] = useState<string | null>(null)
  const [deliverNotice, setDeliverNotice] = useState<string | null>(null)
  const [loanVersion, setLoanVersion] = useState(0)

  const book = useBookDetail(id, validId)
  const loan = useBookLoanStatus(id, validId, loanVersion)
  const handleBack = useGoBack('/')

  // Clear the navigation state once so reloading or pressing back does not reopen the dialog
  useEffect(() => {
    if (fromHistory) {
      navigate(location.pathname, { replace: true, state: null })
    }
    // Only run once on mount — `fromHistory` is derived from the initial render's location.state
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  // Reviews is a separate screen (/books/:bookId/reviews), not a panel on this page
  const handleTabChange = (next: BookTabId) => {
    if (next === 'reviews') navigate(`/books/${id}/reviews`)
    else setTab(next)
  }

  const handleLoanAction = (status: LoanStatus) => {
    if (status === 'LoginRequired') navigate('/login', { state: { from: `/books/${id}` } })
    else if (status === 'Available') navigate(`/books/${id}/loan`)
    else if (status === 'BorrowedByYou') {
      setDeliverError(null)
      setDeliverNotice(null)
      setDeliverOpen(true)
    }
  }

  const handleDeliver = async () => {
    if (!loan.data?.userId) return
    setDeliverSubmitting(true)
    setDeliverError(null)
    try {
      const res = await returnLoan(loan.data.userId, id)
      setDeliverOpen(false)
      setDeliverNotice(res.message)
      setLoanVersion((v) => v + 1)
    } catch (error) {
      setDeliverError(getLoanErrorMessage(error))
    } finally {
      setDeliverSubmitting(false)
    }
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

  // Only allow delivery when loan-status confirms BorrowedByYou (has the right dueDate)
  const canDeliver = loan.data?.status === 'BorrowedByYou'

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

        {deliverNotice && (
          <p role="status" className={styles.notice}>{deliverNotice}</p>
        )}
      </div>

      <ConfirmDialog
        open={deliverOpen && canDeliver}
        title="Return book"
        submitting={deliverSubmitting}
        error={deliverError}
        confirmLabel="Deliver"
        onConfirm={handleDeliver}
        onCancel={() => setDeliverOpen(false)}
      >
        <p><strong>{detail.title}</strong></p>
        {detail.authors.length > 0 && (
          <p>{detail.authors.map((a) => a.name).join(', ')}</p>
        )}
        {detail.categories.length > 0 && (
          <p>{detail.categories.map((c) => c.name).join(', ')}</p>
        )}
        {loan.data?.dueDate && (
          <p>Due: {new Date(loan.data.dueDate).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })}</p>
        )}
        <p>Are you sure you want to deliver this book?</p>
      </ConfirmDialog>
    </main>
  )
}