import { useEffect, useId, useRef, useState, type CSSProperties } from 'react'
import styles from './ExpandableText.module.css'

interface ExpandableTextProps {
  text: string
  lines?: number
}

export function ExpandableText({ text, lines = 14 }: ExpandableTextProps) {
  const textId = useId()
  const textRef = useRef<HTMLParagraphElement>(null)
  const [expanded, setExpanded] = useState(false)
  const [overflowing, setOverflowing] = useState(false)

  useEffect(() => {
    const el = textRef.current
    if (!el || expanded) return

    const observer = new ResizeObserver(() => {
      setOverflowing(el.scrollHeight > el.clientHeight + 1)
    })
    observer.observe(el)

    return () => observer.disconnect()
  }, [text, lines, expanded])

  return (
    <div>
      <p
        id={textId}
        ref={textRef}
        className={`${styles.text} ${expanded ? '' : styles.clamped}`}
        style={{ '--lines': lines } as CSSProperties}
      >
        {text}
      </p>

      {(overflowing || expanded) && (
        <button
          type="button"
          className={styles.toggle}
          aria-expanded={expanded}
          aria-controls={textId}
          onClick={() => setExpanded((value) => !value)}
        >
          {expanded ? 'See less' : 'See more'}
        </button>
      )}
    </div>
  )
}