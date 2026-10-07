import { Outlet, useMatch } from 'react-router'
import { Navbar } from '../shared/components/Navbar/Navbar'

export function AppLayout() {
  const overlayNavbar = useMatch('/books/:bookId') !== null

  return (
    <>
      <Navbar overlay={overlayNavbar} />
      <Outlet />
    </>
  )
}