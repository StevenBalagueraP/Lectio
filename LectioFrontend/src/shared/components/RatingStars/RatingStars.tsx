import { MdStar } from 'react-icons/md'
import styles from './RatingStars.module.css'

interface RatingStarsProps {
  value: number
}

function FiveStars() {
  return (
    <>
      {Array.from({ length: 5 }, (_, i) => (
        <MdStar key={i} />
      ))}
    </>
  )
}

export function RatingStars({ value }: RatingStarsProps) {
  const safe = Math.max(0, Math.min(5, value))

  return (
    <span className={styles.stars} role="img" aria-label={`${safe} de 5 estrellas`}>
      <span className={styles.base} aria-hidden="true">
        <FiveStars />
      </span>
      <span className={styles.fill} aria-hidden="true" style={{ width: `${(safe / 5) * 100}%` }}>
        <FiveStars />
      </span>
    </span>
  )
}