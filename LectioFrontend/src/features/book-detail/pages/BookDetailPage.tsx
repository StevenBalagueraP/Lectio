import { useParams } from 'react-router'

export function BookDetailPage() {
  const { bookId } = useParams()

  return <h1>Detalle del libro {bookId}</h1>
}