interface Entry<T> {
  data: T
  expiresAt: number
}

const STORAGE_PREFIX = 'lectio:cache:'

const memory = new Map<string, Entry<unknown>>()
const inflight = new Map<string, Promise<unknown>>()

export function readCache<T>(key: string, persist = false): T | undefined {
  const now = Date.now()

  const hit = memory.get(key)
  if (hit) {
    if (hit.expiresAt > now) return hit.data as T
    memory.delete(key)
  }

  if (persist) {
    try {
      const raw = localStorage.getItem(STORAGE_PREFIX + key)
      if (raw) {
        const entry = JSON.parse(raw) as Entry<T>
        if (entry.expiresAt > now) {
          memory.set(key, entry)
          return entry.data
        }
        localStorage.removeItem(STORAGE_PREFIX + key)
      }
    } catch {
      // localStorage no disponible o JSON corrupto: se ignora y se vuelve a pedir
    }
  }

  return undefined
}

export function writeCache<T>(key: string, data: T, ttl: number, persist = false) {
  const entry: Entry<T> = { data, expiresAt: Date.now() + ttl }
  memory.set(key, entry)

  if (persist) {
    try {
      localStorage.setItem(STORAGE_PREFIX + key, JSON.stringify(entry))
    } catch {
      // cuota llena o modo privado: queda solo en memoria
    }
  }
}

export function invalidateCache(key: string) {
  memory.delete(key)
  try {
    localStorage.removeItem(STORAGE_PREFIX + key)
  } catch {
    // nada que hacer
  }
}

export function fetchCached<T>(
  key: string,
  fetcher: () => Promise<T>,
  ttl: number,
  persist = false,
): Promise<T> {
  const pending = inflight.get(key) as Promise<T> | undefined
  if (pending) return pending

  const promise = fetcher()
    .then((data) => {
      writeCache(key, data, ttl, persist)
      return data
    })
    .finally(() => inflight.delete(key))

  inflight.set(key, promise)
  return promise
}