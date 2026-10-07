import type { BookRatingUser } from '../../../../api/types/rating'
import { ReviewCard } from '../ReviewCard/ReviewCard'
import styles from './ReviewList.module.css'

interface ReviewListProps {
  reviews: BookRatingUser[]
}

export function ReviewList({ reviews }: ReviewListProps) {
  return (
    <ul className={styles.list}>
      {reviews.map((review) => (
        <li key={review.idUser}>
          <ReviewCard review={review} />
        </li>
      ))}
    </ul>
  )
}