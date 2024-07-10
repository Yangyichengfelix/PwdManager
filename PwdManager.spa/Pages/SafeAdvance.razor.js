

// Helper function to convert byte arrays to hex strings
function bytesToHex(bytes) {
    return Array.from(bytes).map(byte => byte.toString(16).padStart(2, "0")).join("");
}

// Helper function to export a CryptoKey as a hex string
async function exportKey(key) {
    const keyData = await crypto.subtle.exportKey("raw", key);
    return bytesToHex(new Uint8Array(keyData));
}

export async function generatePrivateKeyAndSalt(initPassword) {
    // Convert initialization password to Uint8Array
    const initPasswordBytes = new TextEncoder().encode(initPassword);

    // Generate a random salt
    const salt = new Uint8Array(16);
    crypto.getRandomValues(salt);

    // Derive a symmetric key from the initialization password and salt
    const keyMaterial = await crypto.subtle.importKey("raw", initPasswordBytes, { name: "PBKDF2" }, false, ["deriveKey"]);
    const derivedKey = await crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: salt,
            iterations: 5000,
            hash: "SHA-256"
        },
        keyMaterial,
        { name: "AES-GCM", length: 256 },
        true,
        ["encrypt", "decrypt"]
    );

    // Export the derived key as hex string
    const privateKey = await exportKey(derivedKey);

    // Return JSON object with private key and salt
    return { privateKey, salt: bytesToHex(salt) };
}





export async function encryptInfo(privateKey, salt, login, password, url) {
    // Convert login and password to Uint8Array
    let loginBytes = new TextEncoder().encode(login);
    let passwordBytes = new TextEncoder().encode(password);
    let urlBytes = new TextEncoder().encode(url);

    // Derive a symmetric key from the private key and salt
    let keyMaterial = await crypto.subtle.importKey("raw", hexToBytes(privateKey), { name: "PBKDF2" }, false, ["deriveKey"]);
    let derivedKey = await crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: hexToBytes(salt),
            iterations: 5000,
            hash: "SHA-256"
        },
        keyMaterial,
        { name: "AES-GCM", length: 256 },
        true,
        ["encrypt", "decrypt"]
    );

    // Encrypt data using AES-GCM
    let ivLogin = crypto.getRandomValues(new Uint8Array(12));
    let ivPassword = crypto.getRandomValues(new Uint8Array(12));
    let ivUrl = crypto.getRandomValues(new Uint8Array(12));

    let cipherlogin = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivLogin }, derivedKey, loginBytes);
    let cipherpassword = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivPassword }, derivedKey, passwordBytes);
    let cipherurl = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivUrl }, derivedKey, urlBytes);

    // Return JSON object with encrypted data
    return {
        Cipherlogin: bytesToHex(new Uint8Array(cipherlogin)),
        Cipherpassword: bytesToHex(new Uint8Array(cipherpassword)),
        Cipherurl: bytesToHex(new Uint8Array(cipherurl)),
        loginTag: bytesToHex(new Uint8Array(cipherlogin.slice(-16))),
        passwordTag: bytesToHex(new Uint8Array(cipherpassword.slice(-16))),
        urlTag: bytesToHex(new Uint8Array(cipherurl.slice(-16))),
        ivLogin: bytesToHex(ivLogin),
        ivPassword: bytesToHex(ivPassword),
        ivUrl: bytesToHex(ivUrl),
    };

}



// Helper functions


function hexToBytes(hex) {
    let bytes = [];
    for (let i = 0; i < hex.length; i += 2) {
        bytes.push(parseInt(hex.substr(i, 2), 16));
    }
    return new Uint8Array(bytes);
}
