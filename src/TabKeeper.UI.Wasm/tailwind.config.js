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
    primary: "#0035BE",
    secondary: "#005A6F",
    tertiary: "#054C6C",
    error: "#D4352F",
    neutral: "#919094"
  },
  {
    scheme: "content",
    extend: true
  }
);
