/** @type {import('tailwindcss').Config} */

function generator(obj, color) {
  const nums = [0, 10, 20, 25, 30, 35, 40, 50, 60, 70, 80, 90, 95, 98, 99, 100];
  const props = nums.map(i => `var(--md-ref-palette-${color}${i})`);
  for (let i = 0; i < nums.length; i++) {
    obj[nums[i]] = props[i];
  }
}

let primary = { DEFAULT: "var(--md-sys-color-primary)" };
let secondary = { DEFAULT: "var(--md-sys-color-secondary)" };
let tertiary = { DEFAULT: "var(--md-sys-color-tertiary)" };
let error = { DEFAULT: "var(--md-sys-color-error)" };
let neutral = {}

generator(primary, "primary");
generator(secondary, "secondary");
generator(tertiary, "tertiary");
generator(error, "error");
generator(neutral, "error");

module.exports = {
  darkMode: 'class',
  content: [
    "./**/*.{razor,html,cshtml}"
  ],
  plugins: [
    require('@tailwindcss/forms')({ strategy: 'base' })
  ],
  theme: {
    extend: {
      screens: {}, // '2xl': {'max': '1535px'},
      colors: {
        primary,
        "primary-container": "var(--md-sys-color-primary-container)",
        "primary-inverse": "var(--md-sys-color-inverse-primary)",
        "on-primary": "var(--md-sys-color-on-primary)",
        "on-primary-container": "var(--md-sys-color-on-primary-container)",
        secondary,
        "secondary-container": "var(--md-sys-color-secondary-container)",
        "on-secondary": "var(--md-sys-color-on-secondary)",
        "on-secondary-container": "var(--md-sys-color-on-secondary-container)",
        tertiary,
        "tertiary-container": "var(--md-sys-color-tertiary-container)",
        "on-tertiary": "var(--md-sys-color-on-tertiary)",
        "on-tertiary-container": "var(--md-sys-color-on-tertiary-container)",
        error,
        "error-container": "var(--md-sys-color-error-container)",
        "on-error": "var(--md-sys-color-on-error)",
        "on-error-container": "var(--md-sys-color-on-error-container)",
        "background": "var(--md-sys-color-background)",
        "on-background": "var(--md-sys-color-on-background)",
        "surface": "var(--md-sys-color-surface)",
        "surface-inverse": "var(--md-sys-color-inverse-surface)",
        "surface-variant": "var(--md-sys-color-surface-variant)",
        "surface-tint": "var(--md-sys-color-surface-tint)",
        "on-surface": "var(--md-sys-color-on-surface)",
        "on-surface-inverse": "var(--md-sys-color-inverse-on-surface)",
        "on-surface-variant": "var(--md-sys-color-on-surface-variant)",
        "outline": "var(--md-sys-color-outline)",
        "outline-variant": "var(--md-sys-color-outline-variant)",
        "shadow": "var(--md-sys-color-shadow)",
        "scrim": "var(--md-sys-color-scrim)",
        neutral
      }
    }
  }
}
