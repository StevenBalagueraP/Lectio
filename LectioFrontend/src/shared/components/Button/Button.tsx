import type { ButtonHTMLAttributes } from 'react'
import styles from './Button.module.css'

interface ButtonProps extends Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'className'> {
  variant?: 'primary' | 'secondary' | 'danger'
  block?: boolean
}

export function Button({ variant = 'primary', block = false, type = 'button', ...props }: ButtonProps) {
  const className = [styles.button, styles[variant], block ? styles.block : '']
    .filter(Boolean)
    .join(' ')

  return <button type={type} className={className} {...props} />
}