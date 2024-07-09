// Import for the side effect of defining a global 'browser' variable
import * as _ from "/content/Blazor.BrowserExtension/lib/browser-polyfill.min.js";

browser.runtime.onInstalled.addListener(() => {
  const indexPageUrl = browser.runtime.getURL("index.html");
  browser.tabs.create({
    url: indexPageUrl
  });
});

var privatekey = "";
browser.runtime.onConnect.addListener(function (port) {
    console.log("Connected .....");
    port.onMessage.addListener(function (msg) {
        console.log(msg)
        if (msg.type === "set") {
            privatekey = msg.content;

        }
        else if(msg.type==="get") {          
            port.postMessage(privatekey);
        }
        //console.log(privatekey);
    });
});


