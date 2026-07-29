const QUILL_JS = "https://cdn.jsdelivr.net/npm/quill@2.0.2/dist/quill.js";
const QUILL_CSS = "https://cdn.jsdelivr.net/npm/quill@2.0.2/dist/quill.snow.css";

function loadScript(src) {
    return new Promise((resolve, reject) => {
        const existing = document.querySelector(`script[src="${src}"]`);
        if (existing) {
            resolve();
            return;
        }
        const script = document.createElement("script");
        script.src = src;
        script.onload = () => resolve();
        script.onerror = () => reject(new Error("Failed to load " + src));
        document.head.appendChild(script);
    });
}

async function ensureQuillLoaded() {
    if (!document.querySelector(`link[href="${QUILL_CSS}"]`)) {
        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = QUILL_CSS;
        document.head.appendChild(link);
    }
    if (!window.Quill) {
        await loadScript(QUILL_JS);
    }
}

export async function initEditor(container, initialHtml, dotNetRef) {
    await ensureQuillLoaded();

    const quill = new window.Quill(container, {
        theme: "snow",
        modules: {
            toolbar: [
                [{ header: [1, 2, 3, false] }],
                ["bold", "italic", "underline", "strike"],
                [{ list: "ordered" }, { list: "bullet" }],
                [{ direction: "rtl" }],
                ["link", "image"],
                ["clean"]
            ]
        }
    });

    if (initialHtml) {
        quill.clipboard.dangerouslyPasteHTML(initialHtml);
    }

    quill.on("text-change", () => {
        dotNetRef.invokeMethodAsync("OnContentChanged", quill.root.innerHTML);
    });

    let savedRange = quill.getSelection();

    quill.getModule("toolbar").addHandler("image", () => {
        savedRange = quill.getSelection(true);
        dotNetRef.invokeMethodAsync("RequestImageInsert");
    });

    return {
        setContent: (html) => {
            quill.root.innerHTML = html || "";
        },
        insertImage: (url) => {
            const range = savedRange || quill.getSelection(true) || { index: quill.getLength(), length: 0 };
            quill.insertEmbed(range.index, "image", url, "user");
            quill.setSelection(range.index + 1, 0, "user");
        },
        dispose: () => {
            quill.off("text-change");
        }
    };
}
