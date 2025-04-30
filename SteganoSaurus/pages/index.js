import { useState } from 'react'

export default function Home() {
  const [info, setInfo]     = useState(null)
  const [hiddenMsg, setMsg] = useState('')

  async function handleUpload(e) {
    e.preventDefault()
    const file = e.target.image.files[0]
    if (!file) return
    const form = new FormData()
    form.append('image', file)
    const res  = await fetch('/api/upload', { method: 'POST', body: form })
    const json = await res.json()
    setInfo(json)
    setMsg(json.hiddenMessage ?? 'No hidden message found')
  }

  return (
    <main style={{ padding: '2rem', fontFamily: 'sans-serif' }}>
      <h1>Image Inspector</h1>
      <form onSubmit={handleUpload}>
        <input type="file" name="image" accept="image/*" />
        <button type="submit" style={{ marginLeft: '1rem' }}>
          Upload & Inspect
        </button>
      </form>

      {info && (
        <section style={{ marginTop: '2rem' }}>
          <h2>EXIF Metadata</h2>
          <pre style={{ background: '#f0f0f0', padding: '1rem', borderRadius: '8px' }}>
            {JSON.stringify(info.exif, null, 2)}
          </pre>

          <h2>Hidden Message</h2>
          <p style={{ background: '#f9f9f9', padding: '1rem', borderRadius: '8px' }}>
            {hiddenMsg}
          </p>

          <h2>Preview</h2>
          <img
            src={info.dataUrl}
            alt="Uploaded file"
            style={{ maxWidth: '100%', borderRadius: '8px' }}
          />
        </section>
      )}
    </main>
  )
}
