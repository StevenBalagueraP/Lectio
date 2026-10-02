import { useEffect, useId, useRef, useState } from 'react'
import { MdCheck, MdExpandLess, MdExpandMore } from 'react-icons/md'
import type { Category } from '../../../../api/types/category'
import styles from './CategoryFilter.module.css'

interface CategoryFilterProps {
  categories: Category[]
  selectedIds: number[]
  onChange: (ids: number[]) => void
  loading?: boolean
}

function getTriggerLabel(categories: Category[], selectedIds: number[]) {
  if (selectedIds.length === 0) return 'All categories'

  const names = categories.filter((c) => selectedIds.includes(c.id)).map((c) => c.name)
  if (names.length === 0) return `${selectedIds.length} selected`
  if (names.length <= 2) return names.join(', ')
  return `${names.length} categories`
}

export function CategoryFilter({ categories, selectedIds, onChange, loading }: CategoryFilterProps) {
  const [open, setOpen] = useState(false)
  const rootRef = useRef<HTMLDivElement>(null)
  const labelId = useId()
  const panelId = useId()

  useEffect(() => {
    if (!open) return

    const onPointerDown = (event: PointerEvent) => {
      if (!rootRef.current?.contains(event.target as Node)) setOpen(false)
    }
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') setOpen(false)
    }

    document.addEventListener('pointerdown', onPointerDown)
    document.addEventListener('keydown', onKeyDown)
    return () => {
      document.removeEventListener('pointerdown', onPointerDown)
      document.removeEventListener('keydown', onKeyDown)
    }
  }, [open])

  const toggle = (id: number) =>
    onChange(selectedIds.includes(id) ? selectedIds.filter((x) => x !== id) : [...selectedIds, id])

  const Chevron = open ? MdExpandLess : MdExpandMore

  return (
    <div className={styles.root} ref={rootRef}>
      <span id={labelId} className={styles.label}>
        Filter by category
      </span>

      <button
        type="button"
        className={styles.trigger}
        aria-expanded={open}
        aria-controls={panelId}
        aria-labelledby={labelId}
        onClick={() => setOpen((v) => !v)}
      >
        <span className={styles.triggerLabel}>{getTriggerLabel(categories, selectedIds)}</span>
        <Chevron className={styles.triggerIcon} aria-hidden="true" />
      </button>

      {open && (
        <div id={panelId} className={styles.panel} role="group" aria-labelledby={labelId}>
          {categories.length === 0 ? (
            <p className={styles.message}>
              {loading ? 'Loading categories…' : 'No categories available'}
            </p>
          ) : (
            <ul className={styles.list}>
              {categories.map((category) => {
                const selected = selectedIds.includes(category.id)

                return (
                  <li key={category.id}>
                    <label className={`${styles.item} ${selected ? styles.itemSelected : ''}`}>
                      <input
                        type="checkbox"
                        className={styles.checkbox}
                        checked={selected}
                        onChange={() => toggle(category.id)}
                      />
                      <span>{category.name}</span>
                      {selected && <MdCheck className={styles.check} aria-hidden="true" />}
                    </label>
                  </li>
                )
              })}
            </ul>
          )}

          {selectedIds.length > 0 && (
            <button type="button" className={styles.clear} onClick={() => onChange([])}>
              Clear filters
            </button>
          )}
        </div>
      )}
    </div>
  )
}