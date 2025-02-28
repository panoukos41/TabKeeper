(() => {
  const dark = "dark";
  const light = "light";
  const key = "theme";

  if (localStorage[key] === dark || (!(key in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    document.documentElement.setAttribute('dark', true)
  } else {
    document.documentElement.removeAttribute('dark')
  }
})();