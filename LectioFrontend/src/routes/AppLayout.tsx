import { Outlet } from 'react-router'
import { Navbar } from '../shared/components/Navbar/Navbar'

export function AppLayout() {
  return (
    <>
      <Navbar />
      <Outlet />
    </>
  )
}