import { Link } from 'react-router'
import styles from './LoginPage.module.css'

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