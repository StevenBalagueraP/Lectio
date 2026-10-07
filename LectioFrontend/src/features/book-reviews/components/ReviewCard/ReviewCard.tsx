import { MdPerson } from 'react-icons/md'
import type { BookRatingUser } from '../../../../api/types/rating'
import { RatingStars } from '../../../../shared/components/RatingStars/RatingStars'
import { formatReviewDate } from '../../utils'
import styles from './ReviewCard.module.css'

interface ReviewCardProps {
  review: BookRatingUser
}

export function ReviewCard({ review }: ReviewCardProps) {
  return (
    <article className={styles.card}>
      <div className={styles.header}>
        <span className={styles.avatar} aria-hidden="true">
          <MdPerson />
        </span>
        {/* El backend no expone nombres, solo el id del autor */}
        <p className={styles.name}>User #{review.idUser}</p>
      </div>

      {review.comment && <p className={styles.comment}>{review.comment}</p>}

      <div className={styles.footer}>
        <time className={styles.date} dateTime={review.updatedAt}>
          {formatReviewDate(review.updatedAt)}
        </time>
        <RatingStars value={review.rating} size="lg" />
      </div>
    </article>
  )
}