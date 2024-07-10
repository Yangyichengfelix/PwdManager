

window.generateKey = async function (password) {
    let passwordBytes = new TextEncoder().encode(password);
    let salt = crypto.getRandomValues(new Uint8Array(16));

    let keyMaterial = await crypto.subtle.importKey('raw', passwordBytes, { name: 'PBKDF2' }, false, ['deriveBits']);
    let derivedKey = await crypto.subtle.deriveBits({ name: 'PBKDF2', salt, iterations: 5000, hash: 'SHA-256' }, keyMaterial, 256);

    let key = new Uint8Array(derivedKey);

    console.log("Salt:", Array.from(salt, byte => byte.toString(16).padStart(2, '0')).join(''));
    console.log("Private key:", Array.from(key, byte => byte.toString(16).padStart(2, '0')).join(''));

    return key;
};

window.encryptData = async function (key, data) {
    let textEncoder = new TextEncoder();
    let encodedData = textEncoder.encode(data);

    // On s'assure que key est de type Uint8Array avant l'importation
    let keyBuffer = key instanceof Uint8Array ? key : new Uint8Array(key);

    let cryptoKey = await crypto.subtle.importKey('raw', keyBuffer, { name: 'AES-GCM' }, false, ['encrypt']);
    let iv = crypto.getRandomValues(new Uint8Array(12));
    let encrypted = await crypto.subtle.encrypt({ name: 'AES-GCM', iv }, cryptoKey, encodedData);

    let cipherText = [...new Uint8Array(encrypted)].map(b => b.toString(16).padStart(2, '0')).join('');
    console.log("Cipher Text:", cipherText);
    console.log("IV:", [...iv].map(b => b.toString(16).padStart(2, '0')).join(''));

    // Obtention du tag
    let tag = await crypto.subtle.generateKey({ name: 'HMAC', hash: { name: 'SHA-256' } }, false, ['sign', 'verify']);
    let signature = await crypto.subtle.sign('HMAC', tag, encrypted);
    console.log("Tag:", [...new Uint8Array(signature)].map(b => b.toString(16).padStart(2, '0')).join(''));

    return [...new Uint8Array(iv), ...new Uint8Array(encrypted)].map(b => b.toString(16).padStart(2, '0')).join('');
};

window.decryptData = async function (key, data) {
    let textDecoder = new TextDecoder();
    let iv = data.slice(0, 24).match(/.{2}/g).map(byte => parseInt(byte, 16));
    let encryptedData = data.slice(24).match(/.{2}/g).map(byte => parseInt(byte, 16));
    let cryptoKey = await crypto.subtle.importKey('raw', key, { name: 'AES-GCM' }, false, ['decrypt']);
    let decrypted = await crypto.subtle.decrypt({ name: 'AES-GCM', iv: new Uint8Array(iv) }, cryptoKey, new Uint8Array(encryptedData));

    let decryptedText = textDecoder.decode(decrypted);
    console.log("Decrypted Data:", decryptedText);

    return decryptedText;
};