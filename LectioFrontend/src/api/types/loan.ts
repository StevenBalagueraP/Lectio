export type LoanStatus = 'LoginRequired' | 'Available' | 'BorrowedByYou' | 'Unavailable'

export interface BookLoanStatus {
  bookId: number
  userId: number | null
  status: LoanStatus
  loanId: number | null
  dueDate: string | null
}