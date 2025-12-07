const downloadFile = (fileName, contentType, content) => {
    const link = document.createElement('a');
    const blob = new Blob([content], { type: contentType || 'text/plain' });
    const url = URL.createObjectURL(blob);
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
};

window.downloadFile = downloadFile;

window.previewHtml = (htmlContent, title = 'Preview') => {
    const previewWindow = window.open('', '_blank');
    if (!previewWindow) {
        return;
    }

    previewWindow.document.write(`<html><head><title>${title}</title></head><body>${htmlContent}</body></html>`);
    previewWindow.document.close();
};
