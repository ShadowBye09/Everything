import nextConnect from 'next-connect'
import multer from 'multer'
import { readExif } from '../../utils/exifReader'

const upload = multer({
  storage: multer.memoryStorage(),
  limits: { fileSize: 5 * 1024 * 1024 }, // 5MB max
})

const apiRoute = nextConnect({
  onError(error, req, res) {
    res.status(500).json({ error: error.message })
  },
  onNoMatch(req, res) {
    res.status(405).json({ error: `Method ${req.method} not allowed` })
  },
})

apiRoute.use(upload.single('image'))

apiRoute.post((req, res) => {
  if (!req.file) return res.status(400).json({ error: 'No file uploaded' })
  const exif = readExif(req.file.buffer)
  // send back base64 for client-side stego decode
  const b64 = req.file.buffer.toString('base64')
  res.status(200).json({ exif, dataUrl: `data:${req.file.mimetype};base64,${b64}` })
})

export default apiRoute
