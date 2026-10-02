import { useState } from 'react'
import { useNavigate } from 'react-router'
import { BookGrid } from '../../../shared/components/BookGrid/BookGrid'
import { CategoryShortcuts } from '../components/CategoryShortcuts/CategoryShortcuts'
import { HeroCarousel } from '../components/HeroCarousel/HeroCarousel'
import { useCategories } from '../../../shared/hooks/useCategories'
import { useTopRatedBooks } from '../hooks'
import styles from './HomePage.module.css'

export function HomePage() {
  const navigate = useNavigate()
  const [selectedCategoryId, setSelectedCategoryId] = useState<number>()

  const categories = useCategories()
  const mostVisited = useTopRatedBooks() // sin categoryId
  const byCategory = useTopRatedBooks(selectedCategoryId, selectedCategoryId !== undefined)

  // Sin categoría seleccionada reutilizamos la data del hero (evita un segundo request)
  const list = selectedCategoryId === undefined ? mostVisited : byCategory

  const handleOpen = (bookId: number) => navigate(`/books/${bookId}`)
  const handleSeeMore = () => navigate('/categories')

  return (
    <main className={styles.main}>
      <section>
        <h2 className={styles.kicker}>
          Most <span className={styles.accent}>Visited</span> in Our{' '}
          <span className={styles.accent}>App</span>
        </h2>
        <HeroCarousel
          books={mostVisited.data ?? []}
          loading={mostVisited.loading}
          onOpen={handleOpen}
        />
      </section>

      <section>
        <h2 className={styles.title}>
          What do you <span className={styles.accent}>plan</span>
          <br className={styles.lineBreak} /> to <span className={styles.accent}>read</span>{' '}
          today?
        </h2>
        <CategoryShortcuts
          categories={categories.data ?? []}
          selectedId={selectedCategoryId}
          onSelect={setSelectedCategoryId}
          onSeeMore={handleSeeMore}
        />
      </section>

      <BookGrid
        books={list.data}
        loading={list.loading}
        error={list.error}
        emptyMessage="No hay libros en esta categoría todavía."
        onOpen={handleOpen}
      />
    </main>
  )
}