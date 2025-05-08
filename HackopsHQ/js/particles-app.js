window.addEventListener('DOMContentLoaded', () => {
  tsParticles.load('tsparticles', {
    fpsLimit: 60,
    particles: {
      number: { value: 100, density: { enable: true, area: 800 } },
      color: { value: '#00ff6a' },
      shape: { type: 'circle' },
      opacity: { value: 0.4 },
      size: { value: { min: 1, max: 4 } },
      move: { enable: true, speed: 3 },
      links: { enable: true, distance: 150, color: '#00ff6a', opacity: 0.2, width: 1 }
    },
    interactivity: {
      events: {
        onHover: { enable: true, mode: 'grab' },
        onClick: { enable: true, mode: 'repulse' }
      },
      modes: {
        grab: { distance: 200, links: { opacity: 0.8 } },
        repulse: { distance: 100, duration: 0.4 }
      }
    },
    retina_detect: true
  });
});
