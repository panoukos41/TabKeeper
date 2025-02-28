const dark = "dark";
const light = "light";
const auto = "auto";
const key = "theme";

export function updateDom() {
  if (localStorage[key] === dark || (!(key in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    document.documentElement.setAttribute('dark', true)
  } else {
    document.documentElement.removeAttribute('dark')
  }
}

export function toggle() {
  if (!(key in localStorage)) return;

  localStorage[key] === light ? setDark() : setLight();
  updateDom();
}

export function setDark() {
  localStorage[key] = dark;
  updateDom();
}

export function setLight() {
  localStorage[key] = light;
  updateDom();
}

export function setAuto() {
  localStorage.removeItem(key);
  updateDom();
}

export function getTheme() {
  const theme = localStorage[key];

  if ([dark, light, auto].some(v => v === theme))
    return theme;

  return auto;
}

