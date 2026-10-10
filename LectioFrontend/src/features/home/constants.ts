import type { IconType } from 'react-icons'
import { FaGhost, FaFingerprint, FaHourglassStart, FaRocket } from 'react-icons/fa'
import { IoSparkles } from "react-icons/io5";
import {
  MdAutoStories,
  MdBolt,
  MdBusinessCenter,
  MdChildCare,
  MdComputer,
  MdEditNote,
  MdFavorite,
  MdFlight,
  MdFormatQuote,
  MdLocalFireDepartment,
  MdMenuBook,
  MdPerson,
  MdPsychology,
  MdRestaurant,
  MdScience,
  MdSelfImprovement,
} from 'react-icons/md'


export const FEATURED_CATEGORIES = [
  'Fiction',
  'Romance',
  'Technology',
  'History',
  'Fantasy',
  'Mystery',
  'Science',
  'Adventure',
]

export const MOBILE_VISIBLE_CATEGORIES = 4

export const DEFAULT_CATEGORY_ICON: IconType = MdMenuBook

export const CATEGORY_LABELS: Record<string, string> = {
  Technology: 'Tech',
  Romance: 'Love',
}

export const CATEGORY_ICONS: Record<string, IconType> = {
  Fiction: MdAutoStories,
  Romance: MdFavorite,
  Technology: MdComputer,
  History: FaHourglassStart,
  Fantasy: IoSparkles,
  Horror: FaGhost,
  Mystery: FaFingerprint,
  Adventure: FaRocket,
  Science: MdScience,
  Biography: MdPerson,
  Poetry: MdFormatQuote,
  'Self-help': MdSelfImprovement,
  Essay: MdEditNote,
  Philosophy: MdPsychology,
  Comics: MdBolt,
  "Children's": MdChildCare,
  Travel: MdFlight,
  'Food & Cooking': MdRestaurant,
  Business: MdBusinessCenter,
  Popular: MdLocalFireDepartment,
}