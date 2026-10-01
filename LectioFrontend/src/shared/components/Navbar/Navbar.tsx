import styles from './Navbar.module.css'
import { MdMenu } from 'react-icons/md'

interface NavbarProps {
  userName?: string
  avatarUrl?: string
  onMenuClick?: () => void
}

const NAV_LINKS = [
  { label: 'Home', href: '/' },
  { label: 'History', href: '/history' },
  { label: 'Categories', href: '/categories' },
]

export function Navbar({ userName = 'User', avatarUrl, onMenuClick }: NavbarProps) {
  return (
    <header className={styles.navbar}>
      <div className={styles.inner}>
        <button type="button" className={styles.menu} aria-label="Abrir menú" onClick={onMenuClick}>
          <MdMenu />
        </button>

        <div className={styles.brand}>
          {/* TODO: reemplazar por <img src={logo} alt="Lectio" className={styles.logo} /> */}
          <span className={styles.logo} aria-hidden="true" />
          <span className={styles.brandName}>Lectio</span>
        </div>

        <div className={styles.right}>
          <nav className={styles.links} aria-label="Principal">
            {NAV_LINKS.map((link) => (
              <a key={link.href} href={link.href}>
                {link.label}
              </a>
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
    </header>
  )
}