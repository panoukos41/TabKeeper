import { variants } from "./tailwind.config.variants"
const { withMaterialColors } = require("tailwind-material-colors");

module.exports = withMaterialColors(
  {
    darkMode: ["selector", "[dark]"],
    content: [
      "./**/*.{razor,html,cshtml}"
    ],
    plugins: [
      require("@tailwindcss/typography"),
      require("@tailwindcss/forms"),
      variants,
    ],
    theme: {
      extend: {
        screens: {}, // "2xl": {"max": "1535px"}
      }
    }
  },
  {
    primary: "#E6E6FA",
    //secondary: "#005A6F",
    //tertiary: "#054C6C",
    //error: "#D4352F",
    //neutral: "#919094"
  },
  {
    /* one of 'content', 'expressive', 'fidelity', 'monochrome', 'neutral', 'tonalSpot' or 'vibrant' */
    scheme: "expressive",
    // contrast is optional and ranges from -1 (less contrast) to 1 (more contrast).
    contrast: 0,
    extend: true
  }
);
