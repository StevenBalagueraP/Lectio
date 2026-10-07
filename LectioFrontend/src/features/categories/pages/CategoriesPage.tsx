import { useEffect, useState } from 'react'
import { MdSearch } from 'react-icons/md'
import { useNavigate, useSearchParams } from 'react-router'
import { BookGrid } from '../../../shared/components/BookGrid/BookGrid'
import { Pagination } from '../../../shared/components/Pagination/Pagination'
import { useCategories } from '../../../shared/hooks/useCategories'
import { useDebouncedValue } from '../../../shared/hooks/useDebouncedValue'
import { CategoryFilter } from '../components/CategoryFilter/CategoryFilter'
import { useBookSearch } from '../hooks'
import styles from './CategoriesPage.module.css'

const PAGE_SIZE = 10

interface ParamChanges {
  q?: string
  categories?: number[]
  page?: number
}

function parseIds(value: string | null): number[] {
  if (!value) return []
  const ids = value
    .split(',')
    .map(Number)
    .filter((n) => Number.isInteger(n) && n > 0)
  return [...new Set(ids)].sort((a, b) => a - b)
}

function setOrDelete(params: URLSearchParams, key: string, value: string) {
  if (value) {
    params.set(key, value)
  } else {
    params.delete(key)
  }
}

export function CategoriesPage() {
  const navigate = useNavigate()
  const [params, setParams] = useSearchParams()

  const search = params.get('q') ?? ''
  const page = Math.max(1, parseInt(params.get('page') ?? '', 10) || 1)
  const categoryIds = parseIds(params.get('categories'))

  const [text, setText] = useState(search)
  const debouncedText = useDebouncedValue(text.trim(), 400)

  const categories = useCategories()
  const results = useBookSearch({ search, categoryIds, page, pageSize: PAGE_SIZE })

  const books = results.data?.data
  const pagination = results.data?.pagination

  const updateParams = (changes: ParamChanges, replace = true) => {
    setParams(
      (prev) => {
        const next = new URLSearchParams(prev)
        if (changes.q !== undefined) setOrDelete(next, 'q', changes.q)
        if (changes.categories !== undefined) {
          setOrDelete(next, 'categories', changes.categories.join(','))
        }
        if (changes.page !== undefined) {
          setOrDelete(next, 'page', changes.page > 1 ? String(changes.page) : '')
        }
        return next
      },
      { replace },
    )
  }

  useEffect(() => {
    if (debouncedText !== search) updateParams({ q: debouncedText, page: 1 })
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [debouncedText])

  const handleCategories = (ids: number[]) => updateParams({ categories: ids, page: 1 })

  const handlePage = (next: number) => {
    updateParams({ page: next }, false)
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  return (
    <main className={styles.main}>
      <div className={styles.filters}>
        <div>
          <label className={styles.label} htmlFor="book-search">
            Search by a specific book
          </label>
          <div className={styles.searchBox}>
            <input
              id="book-search"
              type="text"
              enterKeyHint="search"
              autoComplete="off"
              className={styles.input}
              placeholder="Type a book title…"
              value={text}
              onChange={(event) => setText(event.target.value)}
            />
            <MdSearch className={styles.searchIcon} aria-hidden="true" />
          </div>
        </div>

        <CategoryFilter
          categories={categories.data ?? []}
          selectedIds={categoryIds}
          onChange={handleCategories}
          loading={categories.loading}
        />
      </div>

      {pagination && (
        <p className={styles.count}>
          {pagination.totalCount} {pagination.totalCount === 1 ? 'book' : 'books'} found
        </p>
      )}

      <BookGrid
        books={books}
        loading={results.loading}
        error={results.error}
        skeletonCount={PAGE_SIZE}
        emptyMessage="No books found. Try another search or remove some filters."
        onOpen={(bookId) => navigate(`/books/${bookId}`)}
      />

      {pagination && (
        <Pagination page={page} totalPages={pagination.totalPages} onChange={handlePage} />
      )}
    </main>
  )
}