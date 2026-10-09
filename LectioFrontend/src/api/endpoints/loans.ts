import { http } from '../http'

export interface CreateLoanBody {
  bookId: number
  checkoutDate: string
  dueDate: string
}

export interface CreateLoanResponse {
  message: string
  loanId: number
  bookId: number
  userId: number
  checkoutDate: string
  dueDate: string
}

export interface ReturnLoanResponse {
  message: string
  loanId: number
  bookId: number
  userId: number
  returnDate: string
}

export function createLoan(userId: number, body: CreateLoanBody, signal?: AbortSignal): Promise<CreateLoanResponse> {
  return http<CreateLoanResponse>(
    `/api/users/${userId}/loans`,
    signal,
    { auth: true, method: 'POST', body },
  )
}

export function returnLoan(userId: number, bookId: number, signal?: AbortSignal): Promise<ReturnLoanResponse> {
  return http<ReturnLoanResponse>(
    `/api/users/${userId}/loans/${bookId}/return`,
    signal,
    { auth: true, method: 'POST' },
  )
}
