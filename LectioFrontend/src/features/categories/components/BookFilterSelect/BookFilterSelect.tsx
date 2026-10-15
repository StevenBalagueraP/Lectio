import { useEffect, useId, useRef, useState } from 'react'
import { MdCheck, MdExpandLess, MdExpandMore } from 'react-icons/md'
import type { BookFilter } from '../../../../api/types/bookFilter'
import { BOOK_FILTER_OPTIONS, getBookFilterLabel, toggleBookFilter } from '../../bookFilters'
import styles from '../CategoryFilter/CategoryFilter.module.css'

interface BookFilterSelectProps {
  selected: BookFilter[]
  onChange: (filters: BookFilter[]) => void
}

function getTriggerLabel(selected: BookFilter[]) {
  if (selected.length === 0) return 'Default order'
  if (selected.length <= 2) return selected.map(getBookFilterLabel).join(', ')
  return `${selected.length} filters`
}

export function BookFilterSelect({ selected, onChange }: BookFilterSelectProps) {
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

  const Chevron = open ? MdExpandLess : MdExpandMore

  return (
    <div className={styles.root} ref={rootRef}>
      <span id={labelId} className={styles.label}>
        Sort and filter
      </span>

      <button
        type="button"
        className={styles.trigger}
        aria-expanded={open}
        aria-controls={panelId}
        aria-labelledby={labelId}
        onClick={() => setOpen((v) => !v)}
      >
        <span className={styles.triggerLabel}>{getTriggerLabel(selected)}</span>
        <Chevron className={styles.triggerIcon} aria-hidden="true" />
      </button>

      {open && (
        <div id={panelId} className={styles.panel} role="group" aria-labelledby={labelId}>
          <ul className={styles.list}>
            {BOOK_FILTER_OPTIONS.map((option) => {
              const isSelected = selected.includes(option.value)

              return (
                <li key={option.value}>
                  <label className={`${styles.item} ${isSelected ? styles.itemSelected : ''}`}>
                    <input
                      type="checkbox"
                      className={styles.checkbox}
                      checked={isSelected}
                      onChange={() => onChange(toggleBookFilter(selected, option.value))}
                    />
                    <span>{option.label}</span>
                    {isSelected && <MdCheck className={styles.check} aria-hidden="true" />}
                  </label>
                </li>
              )
            })}
          </ul>

          {selected.length > 0 && (
            <button type="button" className={styles.clear} onClick={() => onChange([])}>
              Clear filters
            </button>
          )}
        </div>
      )}
    </div>
  )
}