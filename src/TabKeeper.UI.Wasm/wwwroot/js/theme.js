let key = "theme";
let auto = "auto";
let dark = "dark";
let light = "light";

export function configure(options) {
  if (options == null) {
    return;
  }
  if (options.key) {
    key = options.key;
  }
  if (options.auto) {
    auto = options.auto;
  }
  if (options.dark) {
    dark = options.dark;
  }
  if (options.light) {
    light = options.light;
  }
}

export function updateDom() {
  const theme = localStorage[key];
  if (theme != null && theme != auto) {
    document.documentElement.setAttribute(key, theme)
  }
  else if (window.matchMedia('(prefers-color-scheme: light)').matches) {
    document.documentElement.setAttribute(key, light)
  }
  else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
    document.documentElement.setAttribute(key, dark)
  }
}

export function getTheme() {
  const theme = localStorage[key];
  if (theme != null) {
    return theme;
  }
  else if (window.matchMedia('(prefers-color-scheme: light)').matches) {
    return light;
  }
  else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
    return dark;
  }
  return "";
}

export function setTheme(theme) {
  localStorage[key] = theme;
  //document.documentElement.setAttribute(key, theme)
  updateDom();
}

export function setAuto() {
  localStorage.removeItem(key);
  document.documentElement.removeAttribute(key);
  updateDom();
}

export function setDark() {
  setTheme(dark);
}

export function setLight() {
  setTheme(light);
}

export function toggle() {
  const theme = localStorage[key];
  if (theme != null) {
    theme === light ? setDark() : setLight();
  }
  else if (window.matchMedia('(prefers-color-scheme: light)').matches) {
    setDark();
  }
  else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
    setLight();
  }
}
