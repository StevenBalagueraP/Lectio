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