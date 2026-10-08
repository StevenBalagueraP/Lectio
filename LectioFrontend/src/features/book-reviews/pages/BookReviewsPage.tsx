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
      navigate('/login', { state: { from: `/books/${id}/reviews` } }) // Al iniciar sesión vuelve a esta pantalla
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