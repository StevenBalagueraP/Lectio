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
        // Una reseña por usuario y libro: `idUser` es único dentro de la lista
        <li key={review.idUser}>
          <ReviewCard review={review} />
        </li>
      ))}
    </ul>
  )
}