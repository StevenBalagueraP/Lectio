export type LoanStatus = 'LoginRequired' | 'Available' | 'BorrowedByYou' | 'Unavailable'

export interface BookLoanStatus {
  bookId: number
  userId: number | null
  status: LoanStatus
  loanId: number | null
  dueDate: string | null
}

// Status of a user's loan entry (distinct from LoanStatus which is per-book availability)
export type UserLoanStatus = 'Active' | 'Overdue' | 'Returned'

export interface LoanBookSummary {
  id: number
  title: string
  imageUrl: string | null
  averageRating: number
  authors: { id: number; name: string }[]
}

export interface UserLoan {
  id: number
  book: LoanBookSummary
  checkoutDate: string
  dueDate: string
  returnDate: string | null
  status: UserLoanStatus
}