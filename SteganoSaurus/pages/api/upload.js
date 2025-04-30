import nextConnect from 'next-connect'
import multer from 'multer'
import { readExif } from '../../utils/exifReader'
import { readStego } from '../../utils/stegoReader'

export const config = {
  api: {
    bodyParser: false,
  },
}

const upload = multer({
  storage: multer.memoryStorage(),
  limits: { fileSize: 5 * 1024 * 1024 }, // 5MB max
})

const handler = nextConnect({
  onError(error, req, res) {
    res.status(500).json({ error: error.message })
  },
  onNoMatch(req, res) {
    res.status(405).json({ error: `Method ${req.method} not allowed` })
  },
})

handler.use(upload.single('image'))

handler.post((req, res) => {
  if (!req.file) return res.status(400).json({ error: 'No file uploaded' })
  const exif = readExif(req.file.buffer)
  const hiddenMessage = readStego(req.file.buffer)
  const dataUrl = `data:${req.file.mimetype};base64,${req.file.buffer.toString('base64')}`

  res.status(200).json({ exif, hiddenMessage, dataUrl })
})

export default handler
