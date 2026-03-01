let abortController = new AbortController();

/** @type {number} */
let timeout;

document.querySelector('.uploadForm')?.addEventListener('input', checkNameClassCombination);

document.querySelector('#screenshot')?.addEventListener('change', async (e) => {
    const target = /** @type {HTMLInputElement} */ (e.target);

    if (target?.files?.[0]) {
        await handleFile(target.files[0]);
    }
});

document.addEventListener('paste', async (e) => {
    const item = e.clipboardData?.items[0];

    if (item?.type.startsWith('image/')) {
        const file = item.getAsFile();
        if (file) {
            const dataTransfer = new DataTransfer();
            dataTransfer.items.add(file);
            /** @type {HTMLInputElement} */ (document.querySelector('#screenshot')).files = dataTransfer.files;

            await handleFile(file);
        }
    }
});

function checkNameClassCombination() {
    const formData = new FormData(/** @type {HTMLFormElement} */ (document.querySelector('.uploadForm')));
    const wowName = /** @type {string | null} */ (formData.get('Input.WowName'))?.trim().toLowerCase();
    const wowClassId = /** @type {string | null} */ (formData.get('Input.WowClassId'));

    clearTimeout(timeout);

    abortController.abort();
    abortController = new AbortController();
    const signal = abortController.signal;


    if (!wowName || !wowClassId || wowName.length < 2) {
        console.log(111)
        return;
    }


    timeout = setTimeout(async () => {
        try {
            const res = await fetch(`/Screenshots/Search?wowName=${wowName}&wowClassId=${wowClassId}`, {
                signal,
            });
            const data = await res.json();

            const nameInput = /** @type {HTMLInputElement} */ (document.querySelector('#Input_WowName'));
            const nameLabel = /** @type {HTMLLabelElement} */ (document.querySelector('label.input'));
            if (data.length) {
                nameLabel.classList.remove('input-success');
                nameLabel.classList.add('input-error');
                nameInput.setCustomValidity('Screenshot with that name and class already exists');
                nameInput.reportValidity();
            } else {
                nameLabel.classList.add('input-success');
                nameLabel.classList.remove('input-error');
                nameInput.setCustomValidity('');
            }
        } catch (err) {
            if (/** @type {DOMException} */ (err).name === 'AbortError') {
                return;
            }

            console.error(err);
        }
    }, 200);
}

/**
 * @param {File} file
 */
async function handleFile(file) {
    const reader = new FileReader();
    reader.onload = (e) => {
        if (typeof e.target?.result === 'string') {
            /** @type {HTMLImageElement} */ (document.querySelector('img.screenshot')).src = e.target.result;
        }
    };
    reader.readAsDataURL(file);

    const worker = await Tesseract.createWorker('eng');

    await worker.setParameters({
        // https://github.com/tesseract-ocr/tesseract/blob/4.0.0/src/ccstruct/publictypes.h#L163
        tessedit_pageseg_mode: Tesseract.PSM.SINGLE_WORD,

        tessedit_char_whitelist: 'abcdefghijklmnopqrstuvwxyz' + 'ABCDEFGHIJKLMNOPQRSTUVWXYZ',
        // 'áÁàÀâÂäÄåÅªæÆçÇœŒéÉèÈêÊëËƒíÍìÌîÎïÏñÑóÓòÒôÔöÖºúÚùÙûÛÜýÝÿ',
    });

    const result = await worker.recognize(file);
    const name = result.data.text.trim().toLowerCase();

    const nameInput = /** @type {HTMLInputElement} */ (document.querySelector('#Input_WowName'));

    nameInput.value = name;
    nameInput.focus();

    await worker.terminate();
}