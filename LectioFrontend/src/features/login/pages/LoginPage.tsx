import { Link } from 'react-router'
import styles from './LoginPage.module.css'

// Página provisional: el login/registro aún no existe en el front.
// Aquí llegan los botones que exigen sesión (Borrow, Add Review).
export function LoginPage() {
  return (
    <main className={styles.main}>
      <h1 className={styles.title}>Log in</h1>
      <p className={styles.message}>
        Login is coming soon. Once it is ready you will be able to borrow books and leave reviews.
      </p>
      <Link to="/" className={styles.link}>
        Back to home
      </Link>
    </main>
  )
}