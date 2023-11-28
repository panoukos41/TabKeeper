/** @type {import('tailwindcss').Config} */

module.exports = {
  content: [
    "./**/*.{razor,html,cshtml}"
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        'primary': {
          DEFAULT: "94a3b8",
          '50': '#f8fafc',
          '100': '#f1f5f9',
          '200': '#e2e8f0',
          '300': '#cbd5e1',
          '400': '#94a3b8',
          '500': '#64748b',
          '600': '#475569',
          '700': '#334155',
          '800': '#1e293b',
          '900': '#0f172a',
          '950': '#020617',
        },
        'secondary': {
          '50': '#f6f6f7',
          '100': '#eeeff1',
          '200': '#e1e1e4',
          '300': '#cdcfd4',
          '400': '#b8bac1',
          '500': '#a5a6af',
          '600': '#868692',
          '700': '#7c7c86',
          '800': '#66666d',
          '900': '#55555a',
          '950': '#323135',
        }
      },
    },
  },
  plugins: []
}
