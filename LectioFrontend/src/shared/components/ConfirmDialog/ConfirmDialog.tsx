import { useEffect, useRef } from 'react'
import { Button } from '../Button/Button'
import styles from './ConfirmDialog.module.css'

interface ConfirmDialogProps {
  open: boolean
  title: string
  submitting?: boolean
  error?: string | null
  onConfirm: () => void
  onCancel: () => void
  confirmLabel?: string
  cancelLabel?: string
  children: React.ReactNode
}

export function ConfirmDialog({
  open,
  title,
  submitting = false,
  error,
  onConfirm,
  onCancel,
  confirmLabel = 'Confirm',
  cancelLabel = 'Cancel',
  children,
}: ConfirmDialogProps) {
  const dialogRef = useRef<HTMLDialogElement>(null)
  // Used to focus the first button (cancel) when dialog opens
  const actionsRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const dialog = dialogRef.current
    if (!dialog) return
    if (open) {
      dialog.showModal()
      // Focus the first button (Cancel) so Esc / Tab work intuitively
      actionsRef.current?.querySelector<HTMLButtonElement>('button')?.focus()
    } else {
      dialog.close()
    }
  }, [open])

  // Intercept the native `cancel` event (fired by Esc) so we control state
  useEffect(() => {
    const dialog = dialogRef.current
    if (!dialog) return
    const handleCancel = (e: Event) => {
      e.preventDefault()
      onCancel()
    }
    dialog.addEventListener('cancel', handleCancel)
    return () => dialog.removeEventListener('cancel', handleCancel)
  }, [onCancel])

  // Basic focus trap: cycle between the two action buttons
  useEffect(() => {
    if (!open) return
    const dialog = dialogRef.current
    if (!dialog) return
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key !== 'Tab') return
      const focusable = Array.from(
        dialog.querySelectorAll<HTMLElement>('button:not([disabled])'),
      )
      if (focusable.length === 0) return
      const first = focusable[0]
      const last = focusable[focusable.length - 1]
      if (e.shiftKey) {
        if (document.activeElement === first) { e.preventDefault(); last.focus() }
      } else {
        if (document.activeElement === last) { e.preventDefault(); first.focus() }
      }
    }
    dialog.addEventListener('keydown', handleKeyDown)
    return () => dialog.removeEventListener('keydown', handleKeyDown)
  }, [open])

  return (
    <dialog ref={dialogRef} className={styles.dialog} aria-modal="true" aria-labelledby="confirm-dialog-title">
      <h2 id="confirm-dialog-title" className={styles.title}>{title}</h2>
      <div className={styles.body}>{children}</div>
      {error && <p role="alert" className={styles.error}>{error}</p>}
      <div ref={actionsRef} className={styles.actions}>
        <Button variant="secondary" block disabled={submitting} onClick={onCancel}>
          {cancelLabel}
        </Button>
        <Button block disabled={submitting} aria-busy={submitting} onClick={onConfirm}>
          {submitting ? 'Delivering…' : confirmLabel}
        </Button>
      </div>
    </dialog>
  )
}
