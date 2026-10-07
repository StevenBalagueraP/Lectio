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