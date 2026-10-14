export function formatShortDate(dateString: string | null | undefined): string {
  if (!dateString) return ''
  const date = new Date(dateString)
  if (isNaN(date.getTime())) return ''

  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
  })
}

export function getDaysUntilDue(dueDateString: string): number {
  const today = new Date()
  const due = new Date(dueDateString)

  const todayDate = new Date(today.getFullYear(), today.getMonth(), today.getDate())
  const dueDate = new Date(due.getFullYear(), due.getMonth(), due.getDate())

  const diffMs = dueDate.getTime() - todayDate.getTime()
  return Math.round(diffMs / (1000 * 60 * 60 * 24))
}
