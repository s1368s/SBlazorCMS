const TINYMCE_JS = "https://cdn.jsdelivr.net/npm/tinymce@7/tinymce.min.js";

function loadScript(src) {
    return new Promise((resolve, reject) => {
        const existing = document.querySelector(`script[src="${src}"]`);
        if (existing) {
            resolve();
            return;
        }
        const script = document.createElement("script");
        script.src = src;
        script.referrerPolicy = "origin";
        script.onload = () => resolve();
        script.onerror = () => reject(new Error("Failed to load " + src));
        document.head.appendChild(script);
    });
}

async function ensureTinyMceLoaded() {
    if (!window.tinymce) {
        await loadScript(TINYMCE_JS);
    }
}

export async function initEditor(container, initialHtml, dotNetRef) {
    await ensureTinyMceLoaded();

    container.innerHTML = initialHtml || "";

    let pendingFilePickerCallback = null;
    let editor = null;

    const editors = await window.tinymce.init({
        target: container,
        license_key: "gpl",
        branding: false,
        promotion: false,
        height: 420,
        directionality: "rtl",
        menubar: false,
        plugins: "advlist autolink lists link image table code fullscreen media searchreplace",
        toolbar: "undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image media table | ltr rtl | removeformat code fullscreen",
        image_advtab: true,
        object_resizing: true,
        file_picker_types: "image",
        file_picker_callback: (callback) => {
            pendingFilePickerCallback = callback;
            dotNetRef.invokeMethodAsync("RequestImageInsert");
        },
        setup: (ed) => {
            editor = ed;
            ed.on("init", () => {
                ed.on("change input undo redo", () => {
                    dotNetRef.invokeMethodAsync("OnContentChanged", ed.getContent());
                });
            });
        }
    });

    editor = editor || editors[0];

    return {
        setContent: (html) => {
            editor.setContent(html || "");
        },
        insertImage: (url) => {
            if (pendingFilePickerCallback) {
                pendingFilePickerCallback(url, { alt: "" });
                pendingFilePickerCallback = null;
            } else {
                editor.insertContent(`<img src="${url}" />`);
            }
        },
        dispose: () => {
            window.tinymce.remove(editor);
        }
    };
}
