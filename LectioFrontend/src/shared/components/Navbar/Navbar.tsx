import { useCallback, useState } from 'react'
import { MdMenu } from 'react-icons/md'
import { Link, NavLink } from 'react-router'
import { MobileMenu } from '../MobileMenu/MobileMenu'
import styles from './Navbar.module.css'
import { NAV_ITEMS } from './navItems'

interface NavbarProps {
  userName?: string
  avatarUrl?: string
}

const DESKTOP_LINKS = NAV_ITEMS.filter((item) => !item.mobileOnly)

export function Navbar({ userName = 'User', avatarUrl }: NavbarProps) {
  const [menuOpen, setMenuOpen] = useState(false)
  const closeMenu = useCallback(() => setMenuOpen(false), [])

  return (
    <header className={styles.navbar}>
      <div className={styles.inner}>
        <button
          type="button"
          className={styles.menu}
          aria-label="Abrir menú"
          aria-expanded={menuOpen}
          aria-controls="mobile-menu"
          onClick={() => setMenuOpen(true)}
        >
          <MdMenu />
        </button>

        <Link to="/" className={styles.brand}>
          {/* TODO: reemplazar por <img src={logo} alt="Lectio" className={styles.logo} /> */}
          <span className={styles.logo} aria-hidden="true" />
          <span className={styles.brandName}>Lectio</span>
        </Link>

        <div className={styles.right}>
          <nav className={styles.links} aria-label="Principal">
            {DESKTOP_LINKS.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                end={link.to === '/'}
                className={({ isActive }) => (isActive ? styles.linkActive : undefined)}
              >
                {link.label}
              </NavLink>
            ))}
          </nav>

          <div className={styles.user}>
            <span className={styles.name}>{userName}</span>
            {avatarUrl ? (
              <img className={styles.avatar} src={avatarUrl} alt={`Avatar de ${userName}`} />
            ) : (
              <span className={styles.avatar} aria-hidden="true">
                {userName.charAt(0).toUpperCase()}
              </span>
            )}
          </div>
        </div>
      </div>

      <MobileMenu open={menuOpen} onClose={closeMenu} />
    </header>
  )
}