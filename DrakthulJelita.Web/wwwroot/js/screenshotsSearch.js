const searchInput = /** @type {HTMLInputElement | null} */ (document.querySelector('#search'));
const screenshots = /** @type {NodeListOf<HTMLElement>} */ (document.querySelectorAll('.screenshot'));

searchInput?.addEventListener('input', () => {
    const inputValue = toAsciiLowercase(searchInput.value);

    screenshots.forEach((screenshot) => {
        const wowName = toAsciiLowercase(screenshot.getAttribute('data-wow-name') ?? '');

        if (wowName.includes(inputValue) || inputValue === '') {
            screenshot.style.display = '';
        } else {
            screenshot.style.display = 'none';
        }
    });
});

/**
 * @param {string} str
 * @returns {string}
 */
function toAsciiLowercase(str) {
    return str
        .toLowerCase()
        // Convert diacritics to combining diacritical marks.
        .normalize("NFD")
        // Delete everything that is not a number or letter.
        .replace(/[^a-zA-Z0-9]/g, '');
}

