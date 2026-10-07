import type { IconType } from 'react-icons'
import { MdApartment, MdCalendarMonth, MdGroups, MdLayers, MdLocalOffer } from 'react-icons/md'
import type { BookDetail } from '../../../../api/types/book'
import styles from './BookInfoList.module.css'

interface BookInfoListProps {
  book: BookDetail
}

interface InfoRow {
  icon: IconType
  label: string
  value: string
}

const UNKNOWN = 'Unknown'

function joinNames(items: { name: string }[]) {
  return items.length > 0 ? items.map((item) => item.name).join(', ') : UNKNOWN
}

export function BookInfoList({ book }: BookInfoListProps) {
  const rows: InfoRow[] = [
    {
      icon: MdGroups,
      label: book.authors.length > 1 ? 'Authors' : 'Author',
      value: joinNames(book.authors),
    },
    { icon: MdApartment, label: 'Editorial', value: book.publisher ?? UNKNOWN },
    { icon: MdCalendarMonth, label: 'Publication', value: book.publishedYear?.toString() ?? UNKNOWN },
    { icon: MdLayers, label: 'Pages', value: book.pageCount?.toString() ?? UNKNOWN },
    { icon: MdLocalOffer, label: 'Categories', value: joinNames(book.categories) },
  ]

  return (
    <ul className={styles.list}>
      {rows.map(({ icon: Icon, label, value }) => (
        <li key={label} className={styles.row}>
          <Icon className={styles.icon} aria-hidden="true" />
          <span className={styles.text}>
            {label}: {value}
          </span>
        </li>
      ))}
    </ul>
  )
}