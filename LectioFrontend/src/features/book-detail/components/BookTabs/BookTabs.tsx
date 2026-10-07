import { BOOK_TABS, getPanelId, getTabId, type BookTabId } from '../../constants'
import styles from './BookTabs.module.css'

interface BookTabsProps {
  active: BookTabId
  onChange: (tab: BookTabId) => void
}

export function BookTabs({ active, onChange }: BookTabsProps) {
  return (
    <div className={styles.tabs} role="tablist" aria-label="Secciones del libro">
      {BOOK_TABS.map((tab) => {
        const selected = tab.id === active

        return (
          <button
            key={tab.id}
            type="button"
            role="tab"
            id={getTabId(tab.id)}
            aria-selected={selected}
            aria-controls={selected ? getPanelId(tab.id) : undefined}
            className={`${styles.tab} ${selected ? styles.tabActive : ''}`}
            onClick={() => onChange(tab.id)}
          >
            {tab.label}
          </button>
        )
      })}
    </div>
  )
}

