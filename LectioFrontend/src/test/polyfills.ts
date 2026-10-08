// jsdom no expone TextEncoder/TextDecoder y react-router los necesita al cargarse.
import { TextDecoder, TextEncoder } from 'node:util'

Object.assign(globalThis, { TextEncoder, TextDecoder })
