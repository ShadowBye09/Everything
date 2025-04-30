import ExifParser from 'exif-parser'

export function readExif(buffer) {
  try {
    const parser = ExifParser.create(buffer)
    const result = parser.parse()
    return result.tags || {}
  } catch (err) {
    return { error: 'No EXIF data found' }
  }
}
