import type { IconType } from 'react-icons'
import { MdCategory, MdHistory, MdHome, MdPerson } from 'react-icons/md'

export interface NavItem {
  label: string
  to: string
  icon: IconType
  mobileOnly?: boolean
  disabled?: boolean
}

export const NAV_ITEMS: NavItem[] = [
  { label: 'Home', to: '/', icon: MdHome },
  { label: 'History', to: '/history', icon: MdHistory },
  { label: 'Categories', to: '/categories', icon: MdCategory },
  { label: 'Profile', to: '/profile', icon: MdPerson, mobileOnly: true},
]