import { decode } from 'steganography.js'

export function readStego(buffer) {
  try {
    // decode takes an <img> element; but steganography.js can decode from canvas
    // we’ll send the raw data back and decode client-side
    return null // server does nothing; decode in browser
  } catch (err) {
    return null
  }
}
