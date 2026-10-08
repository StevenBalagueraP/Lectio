import { Outlet, useMatch } from 'react-router'
import { Navbar } from '../shared/components/Navbar/Navbar'
import { useSession } from '../shared/hooks/useSession'

export function AppLayout() {
  const user = useSession()

  const overlayNavbar = useMatch('/books/:bookId') !== null

  return (
    <>
      <Navbar userName={user?.fullName.split(' ')[0]} overlay={overlayNavbar} />
      <Outlet />
    </>
  )
}