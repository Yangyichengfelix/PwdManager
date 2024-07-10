

// Helper function to convert byte arrays to hex strings
function bytesToHex(bytes) {
    return Array.from(bytes).map(byte => byte.toString(16).padStart(2, "0")).join("");
}

// Helper function to export a CryptoKey as a hex string
async function exportKey(key) {
    const keyData = await crypto.subtle.exportKey("raw", key);
    return bytesToHex(new Uint8Array(keyData));
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

    console.log('Cipherlogin:', bytesToHex(new Uint8Array(cipherlogin)));
    console.log('Cipherpassword:', bytesToHex(new Uint8Array(cipherpassword)));
    console.log('Cipherurl:', bytesToHex(new Uint8Array(cipherurl)));

    console.log('loginTag:', bytesToHex(new Uint8Array(cipherlogin.slice(-16))));
    console.log('passwordTag:', bytesToHex(new Uint8Array(cipherpassword.slice(-16))));
    console.log('urlTag:', bytesToHex(new Uint8Array(cipherurl.slice(-16))));

    console.log('ivLogin:', bytesToHex(ivLogin));
    console.log('ivPassword:', bytesToHex(ivPassword));
    console.log('ivUrl:', bytesToHex(ivUrl));

    // Return JSON object with encrypted data
    return {
        encryptedLogin: bytesToHex(new Uint8Array(cipherlogin)),
        encryptedPwd: bytesToHex(new Uint8Array(cipherpassword)),
        encryptedURL: bytesToHex(new Uint8Array(cipherurl)),
        tagLogin: bytesToHex(new Uint8Array(cipherlogin.slice(-16))),
        tagPwd: bytesToHex(new Uint8Array(cipherpassword.slice(-16))),
        tagUrl: bytesToHex(new Uint8Array(cipherurl.slice(-16))),
        ivLogin: bytesToHex(ivLogin),
       ivPwd: bytesToHex(ivPassword),
        ivUrl: bytesToHex(ivUrl),
    };

}

export async function decryptData(privateKey, salt, iv, cipherText, tag) {
    // Convert privateKey, salt, iv, cipherText, and tag to Uint8Array
    let privateKeyBytes = hexToBytes(privateKey);
    let saltBytes = hexToBytes(salt);
    let ivBytes = hexToBytes(iv);
    let cipherTextBytes = hexToBytes(cipherText);
    let tagBytes = hexToBytes(tag);

    // Derive a symmetric key from the private key and salt
    let keyMaterial = await crypto.subtle.importKey("raw", privateKeyBytes, { name: "PBKDF2" }, false, ["deriveKey"]);
    let derivedKey = await crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: saltBytes,
            iterations: 5000,
            hash: "SHA-256"
        },
        keyMaterial,
        { name: "AES-GCM", length: 256 },
        true,
        ["encrypt", "decrypt"]
    );

    // Decrypt data using AES-GCM
    let decryptedText = await crypto.subtle.decrypt({ name: "AES-GCM", iv: ivBytes, tag: tagBytes }, derivedKey, cipherTextBytes);

    // Convert Uint8Array to string
    let decryptedString = new TextDecoder().decode(decryptedText);

    // Return the decrypted string
    return decryptedString;
}



// Helper functions


function hexToBytes(hex) {
    let bytes = [];
    for (let i = 0; i < hex.length; i += 2) {
        bytes.push(parseInt(hex.substr(i, 2), 16));
    }
    return new Uint8Array(bytes);
}
