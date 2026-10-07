import { RatingStars } from '../../../../shared/components/RatingStars/RatingStars'
import styles from './RatingSummary.module.css'

interface RatingSummaryProps {
  average: number
  count: number
}

export function RatingSummary({ average, count }: RatingSummaryProps) {
  return (
    <section className={styles.summary} aria-label="General rate">
      <p className={styles.label}>General Rate</p>
      <p className={styles.average}>{average.toFixed(1)}</p>
      <div className={styles.stars}>
        <RatingStars value={average} size="lg" />
      </div>
      <p className={styles.count}>rate counter: {count}</p>
    </section>
  )
}