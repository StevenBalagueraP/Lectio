// Formats a date string (ISO or date-only) as a short locale date in en-US.
// The year is omitted if it matches the current year.
// Examples: "Sep 25" (current year), "Sep 25, 2024" (past year).
export function formatShortDate(dateString: string): string {
  const date = new Date(dateString)
  if (isNaN(date.getTime())) return ''

  const now = new Date()
  const includeYear = date.getFullYear() !== now.getFullYear()

  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    ...(includeYear ? { year: 'numeric' } : {}),
  })
}

// Returns the difference in calendar days (local time) between dueDate and today.
// Positive = days remaining; 0 = due today; negative = overdue.
export function getDaysUntilDue(dueDateString: string): number {
  const today = new Date()
  const due = new Date(dueDateString)

  // Compare by calendar date only (strip time)
  const todayDate = new Date(today.getFullYear(), today.getMonth(), today.getDate())
  const dueDate = new Date(due.getFullYear(), due.getMonth(), due.getDate())

  const diffMs = dueDate.getTime() - todayDate.getTime()
  return Math.round(diffMs / (1000 * 60 * 60 * 24))
}
