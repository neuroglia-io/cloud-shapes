﻿/**
 * Sets a checkbox tri-state
 * @param {any} checkboxEl The validation schema to add
 * @param {any} state 0 = unchecked, 1 = checked, -1 = indeterminate
 * @returns
 */
export function setCheckboxState(checkboxEl, state) {
    if (!checkboxEl) return;
    switch (state) {
        case 1:
            checkboxEl.checked = true;
            checkboxEl.indeterminate = false;
            break;
        case -1:
            checkboxEl.checked = false;
            checkboxEl.indeterminate = true;
            break;
        default:
            checkboxEl.checked = false;
            checkboxEl.indeterminate = false;
            break;
    }
}

/**
 * Scrolls down the provided element
 * @param {any} el The element to scroll
 * @param {int} height The height to scroll, or the total scroll if not provided
 */
export function scrollDown(el, height) {
    if (!el) return;
    el.scrollTop = height || el.scrollHeight;
}

/**
 * Sets the theme
 * @param {any} theme "light" or "dark"
 */
export function setTheme(theme) {
    if (theme == 'light') {
        document.documentElement.removeAttribute('data-bs-theme');
    }
    else {
        document.documentElement.setAttribute('data-bs-theme', 'dark');
    }
}