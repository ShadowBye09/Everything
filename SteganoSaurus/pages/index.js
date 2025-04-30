import { useState } from 'react'

export default function Home() {
  const [info, setInfo] = useState(null)
  const [hiddenMsg, setHiddenMsg] = useState('')

  async function handleUpload(e) {
    e.preventDefault()
    const file = e.target.image.files[0]
    if (!file) return
    const form = new FormData()
    form.append('image', file)
    const res = await fetch('/api/upload', { method: 'POST', body: form })
    const json = await res.json()
    setInfo(json)
    // decode stego on client
    const img = new Image()
    img.src = json.dataUrl
    img.onload = () => {
      const msg = window.steganography.decode(img)
      setHiddenMsg(msg || 'No hidden message found')
    }
  }

  return (
    <main style={{ padding: '2rem', fontFamily: 'sans-serif' }}>
      <h1>Image Inspector</h1>
      <form onSubmit={handleUpload}>
        <input type="file" name="image" accept="image/*" />
        <button type="submit">Upload & Inspect</button>
      </form>
      {info && (
        <section style={{ marginTop: '2rem' }}>
          <h2>EXIF Metadata</h2>
          <pre>{JSON.stringify(info.exif, null, 2)}</pre>
          <h2>Hidden Message</h2>
          <p>{hiddenMsg}</p>
        </section>
      )}
    </main>
  )
}
