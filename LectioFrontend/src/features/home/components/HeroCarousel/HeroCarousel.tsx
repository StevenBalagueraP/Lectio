import { useRef, useState } from 'react'
import type { Book } from '../../../../api/types/book'
import { RatingStars } from '../../../../shared/components/RatingStars/RatingStars'
import { formatAuthors, toHttps } from '../../../../shared/utils/book'
import styles from './HeroCarousel.module.css'

interface HeroCarouselProps {
  books: Book[]
  loading: boolean
  onOpen?: (bookId: number) => void
}

export function HeroCarousel({ books, loading, onOpen }: HeroCarouselProps) {
  const trackRef = useRef<HTMLDivElement>(null)
  const [active, setActive] = useState(0)

  const handleScroll = () => {
    const el = trackRef.current
    if (el) setActive(Math.round(el.scrollLeft / el.clientWidth))
  }

  const goTo = (index: number) => {
    const el = trackRef.current
    el?.scrollTo({ left: index * el.clientWidth, behavior: 'smooth' })
  }

  if (loading) return <div className={styles.skeleton} aria-busy="true" />
  if (books.length === 0) return null

  const current = books[active] ?? books[0]
  const next = () => setActive((i) => (i + 1) % books.length)

  return (
    <section aria-label="Libros más visitados">
      {/* MOBILE: swipe con puntos */}
      <div className={styles.mobile}>
        <div ref={trackRef} className={styles.track} onScroll={handleScroll}>
          {books.map((book) => (
            <button
              key={book.id}
              type="button"
              className={styles.slide}
              onClick={() => onOpen?.(book.id)}
            >
              {book.imageUrl && (
                <>
                  <img
                    className={styles.bg}
                    src={toHttps(book.imageUrl)}
                    alt=""
                    aria-hidden="true"
                  />
                  <div className={styles.shade} />
                  <img
                    className={styles.cover}
                    src={toHttps(book.imageUrl)}
                    alt={`Portada de ${book.title}`}
                  />
                </>
              )}
              {book.categories[0] && (
                <span className={styles.badge}>{book.categories[0].name}</span>
              )}
              <span className={styles.caption}>{book.title}</span>
            </button>
          ))}
        </div>

        <div className={styles.dots}>
          {books.map((book, i) => (
            <button
              key={book.id}
              type="button"
              aria-label={`Ir al libro ${i + 1}`}
              className={`${styles.dot} ${i === active ? styles.dotActive : ''}`}
              onClick={() => goTo(i)}
            />
          ))}
        </div>
      </div>

      {/* DESKTOP: libro destacado + botón Next */}
      <div className={styles.desktop}>
        <div className={styles.media}>
          {current.imageUrl && (
            <>
              <img
                className={styles.bg}
                src={toHttps(current.imageUrl)}
                alt=""
                aria-hidden="true"
              />
              <div className={styles.shade} />
              <img
                className={styles.cover}
                src={toHttps(current.imageUrl)}
                alt={`Portada de ${current.title}`}
              />
            </>
          )}
          {current.categories[0] && (
            <span className={styles.badge}>{current.categories[0].name}</span>
          )}
          <div className={styles.mediaCaption}>
            <span className={styles.mediaTitle}>{current.title}</span>
            <span className={styles.ratingPill}>
              <RatingStars value={current.rating?.stars ?? 0} />
            </span>
          </div>
        </div>

        <div className={styles.info}>
          <h3 className={styles.title}>{current.title}</h3>
          <p className={styles.by}>By: {formatAuthors(current.authors)}</p>
          <p className={styles.description}>
            {current.description ?? 'Sin descripción disponible.'}
          </p>
          <div className={styles.actions}>
            <button type="button" className={styles.action} onClick={() => onOpen?.(current.id)}>
              Start Reading
            </button>
            <button
              type="button"
              className={styles.action}
              onClick={next}
              disabled={books.length < 2}
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </section>
  )
}