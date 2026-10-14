import { useCallback, useState } from 'react'
import { MdMenu, MdPerson } from 'react-icons/md'
import { Link, NavLink } from 'react-router'
import { MobileMenu } from '../MobileMenu/MobileMenu'
import styles from './Navbar.module.css'
import { NAV_ITEMS } from './navItems'

interface NavbarProps {
  userName?: string
  avatarUrl?: string
  overlay?: boolean
}

const DESKTOP_LINKS = NAV_ITEMS.filter((item) => !item.mobileOnly)

export function Navbar({ userName, avatarUrl, overlay = false }: NavbarProps) {
  const [menuOpen, setMenuOpen] = useState(false)
  const closeMenu = useCallback(() => setMenuOpen(false), [])

  return (
    <header className={`${styles.navbar} ${overlay ? styles.overlay : ''}`}>
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

          <Link to={userName ? '/profile' : '/login'} className={styles.user}>
            <span className={styles.name}>{userName ?? 'Log in'}</span>
            {avatarUrl ? (
              <img className={styles.avatar} src={avatarUrl} alt="" />
            ) : (
              <span className={styles.avatar} aria-hidden="true">
                {userName ? userName.charAt(0).toUpperCase() : <MdPerson />}
              </span>
            )}
          </Link>
        </div>
      </div>

      <MobileMenu open={menuOpen} onClose={closeMenu} />
    </header>
  )
}