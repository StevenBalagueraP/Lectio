import { useId, useState, type FormEvent } from 'react'
import { MdStar } from 'react-icons/md'
import { Button } from '../../../../shared/components/Button/Button'
import { MAX_COMMENT_LENGTH, STAR_VALUES } from '../../constants'
import styles from './ReviewForm.module.css'

export interface ReviewFormValues {
  stars: number
  comment: string
}

interface ReviewFormProps {
  submitting: boolean
  error: string | null
  onSubmit: (values: ReviewFormValues) => void
  onCancel: () => void
}

export function ReviewForm({ submitting, error, onSubmit, onCancel }: ReviewFormProps) {
  const [stars, setStars] = useState(0)
  const [comment, setComment] = useState('')
  const starsLabelId = useId()
  const commentId = useId()

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (stars === 0 || submitting) return
    onSubmit({ stars, comment: comment.trim() })
  }

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <h2 className={styles.title}>Your review</h2>

      <div className={styles.field}>
        <span id={starsLabelId} className={styles.label}>
          Rating
        </span>
        <div className={styles.stars} role="radiogroup" aria-labelledby={starsLabelId}>
          {STAR_VALUES.map((value) => (
            <button
              key={value}
              type="button"
              role="radio"
              aria-checked={value === stars}
              aria-label={`${value} de 5 estrellas`}
              className={`${styles.star} ${value <= stars ? styles.starActive : ''}`}
              onClick={() => setStars(value)}
            >
              <MdStar aria-hidden="true" />
            </button>
          ))}
        </div>
      </div>

      <div className={styles.field}>
        <label htmlFor={commentId} className={styles.label}>
          Comment (optional)
        </label>
        <textarea
          id={commentId}
          className={styles.textarea}
          rows={4}
          maxLength={MAX_COMMENT_LENGTH}
          value={comment}
          onChange={(event) => setComment(event.target.value)}
        />
        <span className={styles.counter}>
          {comment.length}/{MAX_COMMENT_LENGTH}
        </span>
      </div>

      {error && (
        <p role="alert" className={styles.error}>
          {error}
        </p>
      )}

      <div className={styles.actions}>
        <Button type="submit" disabled={stars === 0 || submitting}>
          {submitting ? 'Sending…' : 'Submit'}
        </Button>
        <Button variant="secondary" disabled={submitting} onClick={onCancel}>
          Cancel
        </Button>
      </div>
    </form>
  )
}