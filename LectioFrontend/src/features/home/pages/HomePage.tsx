import { useState } from 'react'
import { Navbar } from '../../../shared/components/Navbar/Navbar'
import { BookList } from '../components/BookList/BookList'
import { CategoryShortcuts } from '../components/CategoryShortcuts/CategoryShortcuts'
import { HeroCarousel } from '../components/HeroCarousel/HeroCarousel'
import { useCategories, useTopRatedBooks } from '../hooks'
import styles from './HomePage.module.css'

export function HomePage() {
  const [selectedCategoryId, setSelectedCategoryId] = useState<number>()

  const categories = useCategories()
  const mostVisited = useTopRatedBooks() 
  const byCategory = useTopRatedBooks(selectedCategoryId, selectedCategoryId !== undefined)

  const list = selectedCategoryId === undefined ? mostVisited : byCategory

  const handleOpen = (bookId: number) => console.log('detalle', bookId)

  const handleSeeMore = () => console.log('ver todas las categorías')

  return (
    <div className={styles.page}>
      <Navbar />

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

        <BookList
          books={list.data}
          loading={list.loading}
          error={list.error}
          onOpen={handleOpen}
        />
      </main>
    </div>
  )
}