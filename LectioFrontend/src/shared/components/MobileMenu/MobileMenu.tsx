import { useEffect, useRef } from 'react'
import { MdClose } from 'react-icons/md'
import { NavLink } from 'react-router'
import { NAV_ITEMS } from '../Navbar/navItems'
import styles from './MobileMenu.module.css'

interface MobileMenuProps {
  open: boolean
  onClose: () => void
}

export function MobileMenu({ open, onClose }: MobileMenuProps) {
  const closeRef = useRef<HTMLButtonElement>(null)

  useEffect(() => {
    if (!open) return

    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onClose()
    }

    document.addEventListener('keydown', onKeyDown)
    document.body.style.overflow = 'hidden'
    closeRef.current?.focus()

    return () => {
      document.removeEventListener('keydown', onKeyDown)
      document.body.style.overflow = ''
    }
  }, [open, onClose])

  return (
    <>
      <div
        className={`${styles.overlay} ${open ? styles.overlayOpen : ''}`}
        onClick={onClose}
        aria-hidden="true"
      />

      <aside
        id="mobile-menu"
        className={`${styles.drawer} ${open ? styles.drawerOpen : ''}`}
        role="dialog"
        aria-modal="true"
        aria-label="Menú"
      >
        <div className={styles.header}>
          <span className={styles.title}>Lectio</span>
          <button
            ref={closeRef}
            type="button"
            className={styles.close}
            aria-label="Cerrar menú"
            onClick={onClose}
          >
            <MdClose />
          </button>
        </div>

        <nav aria-label="Menú principal">
          <ul className={styles.list}>
            {NAV_ITEMS.map((item) => {
              const Icon = item.icon

              return (
                <li key={item.to}>
                  {item.disabled ? (
                    <span className={`${styles.item} ${styles.disabled}`} aria-disabled="true">
                      <Icon className={styles.icon} aria-hidden="true" />
                      <span className={styles.label}>{item.label}</span>
                      <span className={styles.soon}>Soon</span>
                    </span>
                  ) : (
                    <NavLink
                      to={item.to}
                      end={item.to === '/'}
                      onClick={onClose}
                      className={({ isActive }) =>
                        `${styles.item} ${isActive ? styles.itemActive : ''}`
                      }
                    >
                      <Icon className={styles.icon} aria-hidden="true" />
                      <span className={styles.label}>{item.label}</span>
                    </NavLink>
                  )}
                </li>
              )
            })}
          </ul>
        </nav>
      </aside>
    </>
  )
}