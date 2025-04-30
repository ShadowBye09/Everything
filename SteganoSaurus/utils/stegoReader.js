// utils/stegoReader.js
import { decode as decodeNode } from '@masknet/stego-js/cjs/node'

export function readStego(buffer) {
  try {
    const hidden = decodeNode(buffer)
    return hidden || null
  } catch (err) {
    return null
  }
}
