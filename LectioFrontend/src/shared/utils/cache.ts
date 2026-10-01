interface Entry<T> {
  data: T
  expiresAt: number
}

const STORAGE_PREFIX = 'lectio:cache:'

const memory = new Map<string, Entry<unknown>>()
const inflight = new Map<string, Promise<unknown>>()

function readEntry<T>(key: string, persist: boolean): Entry<T> | undefined {
  const hit = memory.get(key) as Entry<T> | undefined
  if (hit) return hit

  if (persist) {
    try {
      const raw = localStorage.getItem(STORAGE_PREFIX + key)
      if (raw) {
        const entry = JSON.parse(raw) as Entry<T>
        memory.set(key, entry)
        return entry
      }
    } catch {
      // localStorage no disponible o JSON corrupto: se ignora
    }
  }

  return undefined
}

/** Devuelve el dato solo si sigue vigente. */
export function readCache<T>(key: string, persist = false): T | undefined {
  const entry = readEntry<T>(key, persist)
  return entry && entry.expiresAt > Date.now() ? entry.data : undefined
}

/** Devuelve el dato aunque haya expirado (para cuando el backend no responde). */
export function readStale<T>(key: string, persist = false): T | undefined {
  return readEntry<T>(key, persist)?.data
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

/** Una sola petición en vuelo por key; al terminar guarda el resultado en cache. */
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