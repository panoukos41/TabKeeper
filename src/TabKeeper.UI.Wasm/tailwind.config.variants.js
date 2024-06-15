export function variants({ matchVariant }) {
  const defaults = {
    // Positional
    first: ':first-child',
    last: ':last-child',
    only: ':only-child',
    odd: ':nth-child(odd)',
    even: ':nth-child(even)',
    'first-of-type': 'first-of-type',
    'last-of-type': 'last-of-type',
    'only-of-type': 'only-of-type',
    // State
    visited: 'visited',
    target: 'target',
    open: '[open]',
    // Forms
    default: 'default',
    checked: 'checked',
    indeterminate: 'indeterminate',
    'placeholder-shown': 'placeholder-shown',
    autofill: 'autofill',
    required: 'required',
    valid: 'valid',
    invalid: 'invalid',
    'in-range': 'in-range',
    'out-of-range': 'out-of-range',
    'read-only': 'read-only',
    // Content
    empty: 'empty',
    // Interactive
    'focus-within': 'focus-within',
    hover: 'hover',
    focus: 'focus',
    'focus-visible': 'focus-visible',
    active: 'active',
    disabled: 'disabled',
  }
  matchVariant(`parent`, (value) => `:merge(.parent)${value} > &`, { values: defaults })
  matchVariant(`next-input`, (value) => `&:has(+ input:${value})`, { values: defaults })
  matchVariant(`peer-input`, (value) => `input:${value} ~ &`, { values: defaults })
};