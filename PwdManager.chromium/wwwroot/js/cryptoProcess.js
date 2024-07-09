
async function hashPassword(password) {
    let salt = generateSalt();
    let encoder = new TextEncoder();
    let passwordBuffer = encoder.encode(password);
    let saltBuffer = encoder.encode(salt);

    // Derive key using PBKDF2
    let keyMaterial = await crypto.subtle.importKey(
        'raw',
        passwordBuffer,
        { name: 'PBKDF2' },
        false,
        ['deriveBits', 'deriveKey']
    );

    let derivedKey = await crypto.subtle.deriveKey(
        { name: 'PBKDF2', salt: saltBuffer, iterations: 100000, hash: 'SHA-256' },
        keyMaterial,
        { name: 'AES-GCM', length: 256 },
        true,
        ['encrypt', 'decrypt']
    );

    // Export the derived key as bytes
    let exportedKey = await crypto.subtle.exportKey('raw', derivedKey);

    return { hashedPassword: bytesToHex(exportedKey), salt };
}

function generateSalt() {
    return Array.from(crypto.getRandomValues(new Uint8Array(16)))
        .map(byte => byte.toString(16).padStart(2, '0'))
        .join('');
}

function compareHashedPassword(inputPassword, hashedPassword, salt) {
    return new Promise(async (resolve) => {
        let encoder = new TextEncoder();
        let inputBuffer = encoder.encode(inputPassword);
        let saltBuffer = encoder.encode(salt);

        // Derive key using PBKDF2 for the input password
        let keyMaterial = await crypto.subtle.importKey(
            'raw',
            inputBuffer,
            { name: 'PBKDF2' },
            false,
            ['deriveBits', 'deriveKey']
        );

        let derivedKey = await crypto.subtle.deriveKey(
            { name: 'PBKDF2', salt: saltBuffer, iterations: 100000, hash: 'SHA-256' },
            keyMaterial,
            { name: 'AES-GCM', length: 256 },
            true,
            ['encrypt', 'decrypt']
        );

        // Export the derived key as bytes
        let exportedKey = await crypto.subtle.exportKey('raw', derivedKey);

        // Compare the exported key with the stored hashed password
        resolve(bytesToHex(exportedKey) === hashedPassword);
    });
}

function generateKey(password, salt) {
    return new Promise(async (resolve) => {
        let encoder = new TextEncoder();
        let passwordBuffer = encoder.encode(password);
        let saltBuffer = encoder.encode(salt);

        // Derive key using PBKDF2
        let keyMaterial = await crypto.subtle.importKey(
            'raw',
            passwordBuffer,
            { name: 'PBKDF2' },
            false,
            ['deriveBits', 'deriveKey']
        );

        let derivedKey = await crypto.subtle.deriveKey(
            { name: 'PBKDF2', salt: saltBuffer, iterations: 50000, hash: 'SHA-256' },
            keyMaterial,
            { name: 'AES-GCM', length: 256 },
            true,
            ['encrypt', 'decrypt']
        );

        // Export the derived key as bytes
        let exportedKey = await crypto.subtle.exportKey('raw', derivedKey);

        resolve(bytesToHex(exportedKey));
    });
}

async function encryptInfo(privateKey, salt, login, password, url) {
    let loginBytes = new TextEncoder().encode(login);
    let passwordBytes = new TextEncoder().encode(password);
    let urlBytes = new TextEncoder().encode(url);

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

    let ivLogin = crypto.getRandomValues(new Uint8Array(12));
    let ivPassword = crypto.getRandomValues(new Uint8Array(12));
    let ivUrl = crypto.getRandomValues(new Uint8Array(12));

    let encryptedLogin = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivLogin }, derivedKey, loginBytes);
    let encryptedPassword = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivPassword }, derivedKey, passwordBytes);
    let encryptedUrl = await crypto.subtle.encrypt({ name: "AES-GCM", iv: ivUrl }, derivedKey, urlBytes);

    return {
        encryptedLogin: bytesToHex(new Uint8Array(encryptedLogin)),
        EncryptedPwd: bytesToHex(new Uint8Array(encryptedPassword)),
        EncryptedURL: bytesToHex(new Uint8Array(encryptedUrl)),
        tagLogin: bytesToHex(new Uint8Array(encryptedLogin.slice(-16))),
        tagPwd: bytesToHex(new Uint8Array(encryptedPassword.slice(-16))),
        tagUrl: bytesToHex(new Uint8Array(encryptedUrl.slice(-16))),
        ivLogin: bytesToHex(ivLogin),
        iVPwd: bytesToHex(ivPassword),
        ivUrl: bytesToHex(ivUrl),
    };
}

// Fonction pour déchiffrer des informations
async function decryptInfo(privateKey, salt, encryptedLogin, encryptedPassword, encryptedUrl, ivLogin, ivPassword, ivUrl) {
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

    // Déchiffrer les données en utilisant AES-GCM
    let decryptedLogin = await crypto.subtle.decrypt({ name: "AES-GCM", iv: hexToBytes(ivLogin) }, derivedKey, hexToBytes(encryptedLogin));
    let decryptedPassword = await crypto.subtle.decrypt({ name: "AES-GCM", iv: hexToBytes(ivPassword) }, derivedKey, hexToBytes(encryptedPassword));
    let decryptedUrl = await crypto.subtle.decrypt({ name: "AES-GCM", iv: hexToBytes(ivUrl) }, derivedKey, hexToBytes(encryptedUrl));

    // Conversion des résultats en chaînes
    let loginResult = new TextDecoder().decode(decryptedLogin);
    let passwordResult = new TextDecoder().decode(decryptedPassword);
    let urlResult = new TextDecoder().decode(decryptedUrl);

    return { login: loginResult, pwd: passwordResult, url: urlResult };
}

// Helper function to convert bytes to hex string
function bytesToHex(bytes) {
    return Array.from(new Uint8Array(bytes)).map(byte => byte.toString(16).padStart(2, '0')).join('');
}

// Helper function to convert hex string to bytes
function hexToBytes(hex) {
    return new Uint8Array(hex.match(/.{1,2}/g).map(byte => parseInt(byte, 16)));
}