window.addEventListener('DOMContentLoaded', () => {
  tsParticles.load('tsparticles', {
    fpsLimit: 60,
    particles: {
      number: { value: 80, density: { enable: true, area: 800 } },
      color: { value: '#ffffff' },
      shape: { type: 'circle' },
      opacity: { value: 0.5 },
      size: { value: { min: 1, max: 3 } },
      move: { enable: true, speed: 2 },
      links: { enable: true, distance: 150, color: '#ffffff', opacity: 0.4, width: 1 }
    },
    interactivity: {
      events: {
        onHover: { enable: true, mode: 'grab' },
        onClick: { enable: true, mode: 'push' }
      },
      modes: {
        grab: { distance: 200, links: { opacity: 0.7 } },
        push: { quantity: 4 }
      }
    },
    retina_detect: true
  });
});
