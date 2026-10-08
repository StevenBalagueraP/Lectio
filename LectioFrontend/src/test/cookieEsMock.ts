// react-router importa `cookie-es` (solo publica .mjs, que Jest no puede transformar a CommonJS).
// Solo se usa en las cookies de servidor, que esta app no usa; basta con un reemplazo vacío.
export const parse = () => ({})
export const serialize = () => ''
export const splitSetCookieString = (value: string) => [value]
