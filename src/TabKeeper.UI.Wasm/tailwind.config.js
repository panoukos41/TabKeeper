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
    primary: "#32EBDE",
    secondary: "#779793",
    tertiary: "#7594B4",
    error: "#FF5449",
  },
  {
    scheme: "content",
    extend: true
  }
);
