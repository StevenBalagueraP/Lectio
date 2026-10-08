// Utilidades para simular `fetch`. jsdom no trae fetch ni Response, así que
// se devuelve un objeto con la parte de la API que usa src/api/http.ts.

export function jsonResponse(body: unknown, status = 200): Response {
  return {
    ok: status >= 200 && status < 300,
    status,
    json: async () => body,
  } as unknown as Response
}

export function emptyResponse(status: number): Response {
  return {
    ok: false,
    status,
    json: async () => {
      throw new SyntaxError('Unexpected end of JSON input')
    },
  } as unknown as Response
}

type FetchHandler = (url: string, init: RequestInit) => Response | Promise<Response>

/** Reemplaza `fetch` global por un jest.fn con el handler indicado. */
export function mockFetch(handler: FetchHandler) {
  const fn = jest.fn((input: RequestInfo | URL, init: RequestInit = {}) =>
    Promise.resolve(handler(String(input), init)),
  )
  globalThis.fetch = fn as unknown as typeof fetch
  return fn
}
