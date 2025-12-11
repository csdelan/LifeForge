// Modal keyboard event helper for LifeForge
window.modalHelper = {
    dotNetRef: null,

    handleKeyDown: function (e) {
        if (e.key === "Escape" && window.modalHelper.dotNetRef) {
            e.preventDefault();
            window.modalHelper.dotNetRef.invokeMethodAsync('HandleEscapeKey');
        }
    }
};

window.addEscapeKeyListener = function (dotNetRef) {
    window.modalHelper.dotNetRef = dotNetRef;
    document.addEventListener('keydown', window.modalHelper.handleKeyDown);
};

window.removeEscapeKeyListener = function () {
    document.removeEventListener('keydown', window.modalHelper.handleKeyDown);
    window.modalHelper.dotNetRef = null;
};
