import { render, screen } from '@testing-library/react'
import { RatingStars } from './RatingStars'

describe('RatingStars (unit)', () => {
  it('renders an img role with an accessible label showing the rating', () => {
    render(<RatingStars value={4} />)
    expect(screen.getByRole('img', { name: '4 de 5 estrellas' })).toBeInTheDocument()
  })

  it('clamps values below 0 to 0', () => {
    render(<RatingStars value={-1} />)
    expect(screen.getByRole('img', { name: '0 de 5 estrellas' })).toBeInTheDocument()
  })

  it('clamps values above 5 to 5', () => {
    render(<RatingStars value={10} />)
    expect(screen.getByRole('img', { name: '5 de 5 estrellas' })).toBeInTheDocument()
  })

  it('accepts decimal values', () => {
    render(<RatingStars value={3.5} />)
    expect(screen.getByRole('img', { name: '3.5 de 5 estrellas' })).toBeInTheDocument()
  })

  it('accepts value 0', () => {
    render(<RatingStars value={0} />)
    expect(screen.getByRole('img', { name: '0 de 5 estrellas' })).toBeInTheDocument()
  })

  it('accepts value 5', () => {
    render(<RatingStars value={5} />)
    expect(screen.getByRole('img', { name: '5 de 5 estrellas' })).toBeInTheDocument()
  })

  it('applies the lg class modifier when size="lg"', () => {
    const { container } = render(<RatingStars value={3} size="lg" />)
    expect(container.firstChild).toHaveClass('lg')
  })

  it('does NOT apply lg class when size="sm" (default)', () => {
    const { container } = render(<RatingStars value={3} />)
    expect(container.firstChild).not.toHaveClass('lg')
  })
})
