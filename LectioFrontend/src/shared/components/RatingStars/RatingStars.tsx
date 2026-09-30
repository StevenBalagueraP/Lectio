import styles from './RatingStars.module.css'

interface RatingStarsProps {
  value: number
}

export function RatingStars({ value }: RatingStarsProps) {
  const safe = Math.max(0, Math.min(5, value))

  return (
    <span className={styles.stars} role="img" aria-label={`${safe} de 5 estrellas`}>
      <span className={styles.base}>★★★★★</span>
      <span className={styles.fill} style={{ width: `${(safe / 5) * 100}%` }}>
        ★★★★★
      </span>
    </span>
  )
}