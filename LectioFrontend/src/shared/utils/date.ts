// Formats a date string (ISO 8601) as "Mon Day" in en-US, without the year.
// Examples: "Oct 10", "Jan 14".
// Returns an empty string if the value is null, undefined, or an invalid date.
export function formatShortDate(dateString: string | null | undefined): string {
  if (!dateString) return ''
  const date = new Date(dateString)
  if (isNaN(date.getTime())) return ''

  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
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
