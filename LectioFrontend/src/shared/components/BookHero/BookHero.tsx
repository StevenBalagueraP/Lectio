import { MdChevronLeft, MdMenuBook } from 'react-icons/md'
import { toHttps } from '../../utils/book'
import styles from './BookHero.module.css'

interface BookHeroProps {
  title: string
  category?: string
  imageUrl: string | null
  onBack: () => void
  headingLevel?: 1 | 2
}

export function BookHero({ title, category, imageUrl, onBack, headingLevel = 1 }: BookHeroProps) {
  const Heading = headingLevel === 1 ? 'h1' : 'h2'

  return (
    <section className={styles.hero}>
      {imageUrl ? (
        <img className={styles.cover} src={toHttps(imageUrl)} alt={`Portada de ${title}`} />
      ) : (
        <div className={styles.placeholder}>
          <MdMenuBook aria-hidden="true" />
        </div>
      )}

      <button type="button" className={styles.back} aria-label="Volver" onClick={onBack}>
        <MdChevronLeft aria-hidden="true" />
      </button>

      <div className={styles.caption}>
        <Heading className={styles.title}>{title}</Heading>
        {category && <p className={styles.category}>{category}</p>}
      </div>
    </section>
  )
}