


const redirectUri = typeof chrome !== "undefined" && chrome.identity ?
    chrome.identity.getRedirectURL() : 
     `${window.location.origin}/index.html`;
console.log("Chrome extension redirect URI set to ", redirectUri);
console.log("This url must be registered in the Azure portal as a single-page application redirect uri, and as the post logout url");

const msalInstance = new msal.PublicClientApplication({
    auth: {
        authority: "https://login.microsoftonline.com/ae1f23b7-23b1-4759-80cd-dfb63fe25fc5",
        clientId: "55dc59ec-1c82-40b0-a351-5815d726b4a6",
        redirectUri:redirectUri,
        postLogoutRedirectUri: redirectUri
    },
    cache: {
        cacheLocation: "localStorage"
    }
});

// Set currently logged in account
const accounts = msalInstance.getAllAccounts();
if (accounts.length) {
    //document.getElementById("username").innerHTML = accounts[0].username;
}



async function SimulateAccessTokenResult() {
    const tokenResult = await getaccessTk();
    const accessToken = tokenResult.accessToken;
   // console.log(accessToken);
    return accessToken;
}


async function SimulateGraphResult() {
    console.log(await callGraphMeEndpoint());
}
async function SimulateLoginResult() {
    const loginurl = await getLoginUrl();
    const loginresult = await launchWebAuthFlow(loginurl);
    //console.log(loginresult);
}


/**
 * Generates a login url
 */
async function getLoginUrl(request, reject) {
    return new Promise((resolve) => {
        msalInstance.loginRedirect({
            ...request,
            onRedirectNavigate: (url) => {
                resolve(url);
                return false;
            }
        }).catch(reject);
    });
}

/**
 * Generates a logout url
 */
async function getLogoutUrl(request) {
    return new Promise((resolve, reject) => {
        msalInstance.logout({
            ...request,
            onRedirectNavigate: (url) => {
                resolve(url);
                return false;
            }
        }).catch(reject);
    });
}
const params= {
    //authority: `https://login.microsoftonline.com/ae1f23b7-23b1-4759-80cd-dfb63fe25fc5`,
    scopes: [`api://b456bb9c-0156-44b9-8760-61f5bc326637/coffre`],  
    account: msalInstance.getAllAccounts()[0]
  };

async function getaccessTk(){

    try {
        return login = await await acquireToken({
            scopes:["api://b456bb9c-0156-44b9-8760-61f5bc326637/coffre"],
            account: msalInstance.getAllAccounts()[0]
        });
      } catch (error) {
        //await msalInstance.loginRedirect(params);
        //const login = await msalInstance.acquireTokenSilent(params);
        //return login.accessToken;
        console.log(error);
      }
}
/**
 * Makes an http request to the MS graph Me endpoint
 */
async function callGraphMeEndpoint() {
    const {
        accessToken
    } = await acquireToken({
        scopes: [ "user.read" ],
        extraScopesToConsent: ["api://b456bb9c-0156-44b9-8760-61f5bc326637/coffre"],
        account: msalInstance.getAllAccounts()[0]
    });

    return callMSGraph("https://graph.microsoft.com/v1.0/me", accessToken);
}

/**
 * Makes an http request to the given MS graph endpoint
 */
async function callMSGraph(endpoint, accessToken) {
    const headers = new Headers();
    const bearer = `Bearer ${accessToken}`;

    headers.append("Authorization", bearer);

    const options = {
        method: "GET",
        headers
    };

    return fetch(endpoint, options)
        .then(response => response.json())
        .catch(error => console.log(error));
}

/**
 * Attempts to silent acquire an access token, falling back to interactive.
 */
async function acquireToken(request) {
    return msalInstance.acquireTokenSilent(request)
        .catch(async (error) => {
            console.error(error);
            const acquireTokenUrl = await getAcquireTokenUrl(request);

            return launchWebAuthFlow(acquireTokenUrl);
        })
}

/**
 * Generates an acquire token url
 */
async function getAcquireTokenUrl(request) {
    return new Promise((resolve, reject) => {
        msalInstance.acquireTokenRedirect({
            ...request,
            onRedirectNavigate: (url) => {
                resolve(url);
                return false;
            }
        }).catch(reject);
    });
}

/**
 * Launch the Chromium web auth UI.
 * @param {*} url AAD url to navigate to.
 * @param {*} interactive Whether or not the flow is interactive
 */
async function launchWebAuthFlow(url) {
    return new Promise((resolve, reject) => {
        chrome.identity.launchWebAuthFlow({
            interactive: true,
            url
        }, (responseUrl) => {
            // Response urls includes a hash (login, acquire token calls)
            if (responseUrl.includes("#")) {
                msalInstance.handleRedirectPromise(`#${responseUrl.split("#")[1]}`)
                    .then(resolve)
                    .catch(reject)
            } else {
                // Logout calls
                resolve();
            }
        })
    })
}

/**
 * Returns the user sign into the browser.
 */
async function getSignedInUser() {
    return new Promise((resolve, reject) => {
        if (chrome && chrome.identity) {
            // Running in extension popup
            chrome.identity.getProfileUserInfo((user) => {
                if (user) {
                    resolve(user);
                } else {
                    resolve(null);
                }
            });
        } else {
            // Running on localhost
            resolve(null);
        }
    })
}