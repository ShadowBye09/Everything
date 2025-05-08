fetch('/files/manifest.json')
  .then(res => res.json())
  .then(json => {
    const listEl = document.querySelector('#file-list ul');
    json.files.forEach(f => {
      const li = document.createElement('li');
      const a  = document.createElement('a');
      a.href = `/files/${f}`;
      a.textContent = f;
      li.appendChild(a);
      listEl.appendChild(li);
    });
  })
  .catch(err => console.error('Could not load manifest.json', err));
