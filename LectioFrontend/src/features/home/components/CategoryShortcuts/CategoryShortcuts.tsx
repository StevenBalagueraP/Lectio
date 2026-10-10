import type { CSSProperties } from 'react'
import { MdGridView } from 'react-icons/md'
import type { Category } from '../../../../api/types/category'
import {
  CATEGORY_ICONS,
  CATEGORY_LABELS,
  DEFAULT_CATEGORY_ICON,
  FEATURED_CATEGORIES,
  MOBILE_VISIBLE_CATEGORIES,
} from '../../constants'
import styles from './CategoryShortcuts.module.css'

interface CategoryShortcutsProps {
  categories: Category[]
  selectedId?: number
  onSelect: (categoryId?: number) => void
  onSeeMore: () => void
}

export function CategoryShortcuts({
  categories,
  selectedId,
  onSelect,
  onSeeMore,
}: CategoryShortcutsProps) {
  const featured = FEATURED_CATEGORIES.map((name) => categories.find((c) => c.name === name)).filter(
    (c): c is Category => Boolean(c),
  )

  const mobileColumns = Math.min(featured.length, MOBILE_VISIBLE_CATEGORIES) + 1
  const desktopColumns = featured.length + 1

  const toggle = (id: number) => onSelect(id === selectedId ? undefined : id)

  return (
    <div
      className={styles.row}
      style={{ '--cols': mobileColumns, '--cols-desktop': desktopColumns } as CSSProperties}
    >
      {featured.map((c, index) => {
        const Icon = CATEGORY_ICONS[c.name] ?? DEFAULT_CATEGORY_ICON

        return (
          <button
            key={c.id}
            type="button"
            aria-pressed={c.id === selectedId}
            aria-label={c.name}
            className={[
              styles.tile,
              index >= MOBILE_VISIBLE_CATEGORIES ? styles.desktopOnly : '',
              c.id === selectedId ? styles.selected : '',
            ].join(' ')}
            onClick={() => toggle(c.id)}
          >
            <span className={styles.icon}>
              <Icon aria-hidden="true" />
            </span>
            <span className={styles.label}>{CATEGORY_LABELS[c.name] ?? c.name}</span>
          </button>
        )
      })}

      <button type="button" className={styles.tile} onClick={onSeeMore}>
        <span className={styles.icon}>
          <MdGridView aria-hidden="true" />
        </span>
        <span className={styles.label}>See all</span>
      </button>
    </div>
  )
}