import { MdChevronLeft, MdMenuBook } from 'react-icons/md'
import { toHttps } from '../../../../shared/utils/book'
import styles from './BookHero.module.css'

interface BookHeroProps {
  title: string
  category?: string
  imageUrl: string | null
  onBack: () => void
}

export function BookHero({ title, category, imageUrl, onBack }: BookHeroProps) {
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
        <h1 className={styles.title}>{title}</h1>
        {category && <p className={styles.category}>{category}</p>}
      </div>
    </section>
  )
}