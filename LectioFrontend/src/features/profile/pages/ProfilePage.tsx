import { MdChevronRight } from 'react-icons/md'
import { Navigate } from 'react-router'
import { Button } from '../../../shared/components/Button/Button'
import { useSession } from '../../../shared/hooks/useSession'
import { PROFILE_MENU } from '../constants'
import { useLogout } from '../hooks'
import styles from './ProfilePage.module.css'

export function ProfilePage() {
  const user = useSession()
  const { loggingOut, logout } = useLogout()

  if (!user) {
    return <Navigate to="/login" replace state={loggingOut ? undefined : { from: '/profile' }} />
  }

  return (
    <main className={styles.main}>
      <section className={styles.profile}>
        <span className={styles.avatar} aria-hidden="true">
          {user.fullName.charAt(0).toUpperCase()}
        </span>
        <h1 className={styles.greeting}>
          Hola, <span className={styles.name}>{user.fullName}</span>
        </h1>
      </section>

      <ul className={styles.menu}>
        {PROFILE_MENU.map((label) => (
          <li key={label} className={styles.item}>
            <span>{label}</span>
            <MdChevronRight className={styles.chevron} aria-hidden="true" />
          </li>
        ))}
      </ul>

      <div className={styles.actions}>
        <Button block disabled={loggingOut} onClick={logout}>
          {loggingOut ? 'Logging out…' : 'Log Out'}
        </Button>
      </div>
    </main>
  )
}