const { withMaterialColors } = require('./tailwind-material-colors.esm');

module.exports = withMaterialColors(
  {
    darkMode: 'class',
    content: [
      "./**/*.{razor,html,cshtml}"
    ],
    plugins: [
      require('@tailwindcss/forms')({ strategy: 'base' }),
      require('@tailwindcss/typography')
    ],
    theme: {
      extend: {
        screens: {}, // '2xl': {'max': '1535px'}
      }
    },
    safelist: [
      'material-group'
    ]
  },
  {
    // Here, your base colors as HEX values
    // primary is required
    primary: '#ff0000',
    // secondary and/or tertiary are optional, if not set they will be derived from the primary color
    //secondary: '#ffff00',
    //tertiary: '#0000ff'
  },
  {
    extend: true
  }
);
