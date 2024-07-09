function copyToClipboard(text) {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
}

function ScrollToEnd(id) {
    document.getElementById(id).scrollIntoView();
}


function selectText(ref) {
    let input = document.getElementById(ref);
    input.focus();
    input.select();

}
function copyText(ref) {

    let input = document.getElementById(ref);
    input.focus();
    input.select();
    copyToClipboard(ref);
}

function setKey(key) {
    var port = browser.runtime.connect({
        name: "set p key"
    });
    port.postMessage({ type: "set", content: key });
    port.onMessage.addListener(function (msg) {
        console.log("message recieved from background: " + msg);
    });
}

function getKey() {
    var port = browser.runtime.connect({
        name: "get p key"
    });
    port.postMessage({ type: "get", content: null });
    port.onMessage.addListener(function (msg) {
        console.log("message recieved from background: " + msg.content);
    });

    return msg.content;
}