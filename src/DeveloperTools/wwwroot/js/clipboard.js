// Simple clipboard helper with robust fallbacks for non-secure contexts and older browsers
// Exposes window.clipboardCopy.copyText(text) -> Promise<boolean>
(function () {
    const ns = (window.clipboardCopy = window.clipboardCopy || {});

    ns.copyText = async function (text) {
        // Prefer Async Clipboard API when available and in secure contexts
        try {
            if (navigator && navigator.clipboard && window.isSecureContext) {
                await navigator.clipboard.writeText(String(text ?? ""));
                return true;
            }
        } catch (e) {
            // fall through to legacy path
        }

        // Legacy fallback using a hidden textarea and execCommand('copy')
        try {
            const textarea = document.createElement('textarea');
            textarea.value = String(text ?? "");
            // Avoid zoom/scroll jumps
            textarea.setAttribute('readonly', '');
            textarea.style.position = 'fixed';
            textarea.style.top = '-10000px';
            textarea.style.left = '-10000px';
            textarea.style.opacity = '0';
            document.body.appendChild(textarea);

            try {
                textarea.focus();
                textarea.select();
                textarea.setSelectionRange(0, textarea.value.length);

                const successful = document.execCommand('copy');
                if (successful) return true;
            } finally {
                document.body.removeChild(textarea);
            }
        } catch (e) {
            // iOS/Safari fallback using a contenteditable element
            let container;
            try {
                container = document.createElement('div');
                container.style.position = 'fixed';
                container.style.top = '-10000px';
                container.style.left = '-10000px';
                container.style.opacity = '0';
                container.setAttribute('contenteditable', '');
                container.textContent = String(text ?? "");
                document.body.appendChild(container);

                const range = document.createRange();
                range.selectNodeContents(container);
                const selection = window.getSelection();
                selection.removeAllRanges();
                selection.addRange(range);

                const successful = document.execCommand('copy');

                selection.removeAllRanges();
                return !!successful;
            } catch (e) {
                return false;
            } finally {
                if (container && container.parentNode) {
                    container.parentNode.removeChild(container);
                }
            }
        }
        // If all methods fail, return false
        return false;
    };
})();
