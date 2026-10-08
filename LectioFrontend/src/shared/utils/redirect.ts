// Destino al que volver tras iniciar sesión (`state.from`). Solo rutas internas: evita redirecciones abiertas.
export function getRedirectTarget(state: unknown, fallback = '/'): string {
  if (typeof state === 'object' && state !== null && 'from' in state) {
    const { from } = state as { from: unknown }
    if (typeof from === 'string' && from.startsWith('/') && !from.startsWith('//')) return from
  }
  return fallback
}