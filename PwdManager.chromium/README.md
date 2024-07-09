
![Logo](https://dev.azure.com/yyang0113/2d0c4bb2-7a38-4246-8eca-94e06463547d/_apis/git/repositories/9a03a645-ffa4-46fc-90af-42590df12db7/items?path=/PwdManager.spa/wwwroot/vault.png&versionDescriptor%5BversionOptions%5D=0&versionDescriptor%5BversionType%5D=0&versionDescriptor%5Bversion%5D=new-romain&resolveLfs=true&%24format=octetStream&api-version=5.0)


# PwdManager chromium extension [![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

:question: Ce projet exemplaire vise à créer un gestionnaire de mots de passe en tant qu'extension pour le navigateur Google Chrome. Il est développé en utilisant Blazor WebAssembly,  Le projet repose sur [Blazor.BrowserExtension](https://github.com/mingyaulee/Blazor.BrowserExtension) , une bibliothèque créée par [mingyaulee](https://github.com/mingyaulee), et intègre l'authentification Azure AD via Microsoft Authentication Library (MSAL).

Fonctionnalités principales :

- Gestion des Mots de Passe : Stocker, récupérer et gérer en toute sécurité les mots de passe des utilisateurs.
- Authentification Azure AD : Utiliser MSAL pour intégrer l'authentification Azure AD, garantissant une connexion sécurisée et une gestion appropriée des autorisations.
- Interface Utilisateur Intuitive : Créer une interface utilisateur conviviale pour une expérience transparente.
## 🧠Difficultés Rencontrées :
La fusion de Blazor.BrowserExtension et MSAL a présenté plusieurs défis qui ont été surmontés au cours du développement. La documentation détaille ces difficultés, offrant ainsi une ressource précieuse pour d'autres développeurs confrontés à des problèmes similaires. Quelques-unes des difficultés abordées dans la documentation incluent :

- Interopérabilité entre Blazor.BrowserExtension et MSAL : Explorer les défis spécifiques liés à l'intégration de ces deux technologies et les solutions apportées pour assurer un fonctionnement harmonieux.

- URL de redirection post connexion MSAL: 
    
    - 1 . Créez une nouvelle inscription d'application dans le centre d'administration Microsoft Entra.
    - 2 . Fournissez votre ID client dans la configuration PublicClientApplication dans auth.js.
    - 3 . Sous l'onglet Authentification, ajoutez une nouvelle URI de redirection sous Application à page unique.
    - 4 .  L'URL de cette URI de redirection doit être du format https://<extension-id>.chromiumapp.org , par exemple https://epfnbngoodhmbeepjlcohfacgnbhbhah.chromiumapp.org/.
    - 5 . L'ID de votre extension peut être trouvé sur la page Paramètres des extensions après le chargement de l'extension, ou en invoquant chrome.identity.getRedirectURL() dans l'extension.


## Référence

 - :lock: [Authentification OpenId Azure Entra ID](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-microsoft-entra-id?view=aspnetcore-7.0&viewFallbackFrom=aspnetcore-8.0) - Documentation SSO Compte Microsoft Blazor WASM
- 🔗[MSAL](https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/) - Documentation msal-browser en JavaScript 
 - 🛠[Google Chrome extension APIs](https://developer.chrome.com/docs/extensions/reference/api) - Documentation chrome APIs
 - 🚀[Que c'est, Manifest V3](https://developer.chrome.com/docs/extensions/develop/migrate/what-is-mv3) - Ficher Manifest V3 chrome extension
 - ⚡️[Build browser extensions easily with Blazor](https://mingyaulee.github.io/Blazor.BrowserExtension/) - Documentation Blazor.BrowserExtension

## Modification wwwroot/Index.html
````html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>PwdManager.chromium</title>
    <base href="/" />
    <link href="css/app.css" rel="stylesheet" />
    <link rel="icon" type="image/png" href="favicon.png" />
    <!--<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />-->

    <link href="content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
    <link href="PwdManager.chromium.styles.css" rel="stylesheet" />
</head>
<body>
    <div id="app">
        <svg class="loading-progress">
            <circle r="40%" cx="50%" cy="50%" />
            <circle r="40%" cx="50%" cy="50%" />
        </svg>
        <div class="loading-progress-text"></div>
    </div>
    <div id="blazor-error-ui">
        An unhandled error has occurred.
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>
    <script src="framework/blazor.webassembly.js"></script>
    <script src="js/msal-browser.min.js"></script>
    <!--<script src="_content/Microsoft.Authentication.WebAssembly.Msal/AuthenticationService.js"></script>-->
    <!--<script src="_content/MudBlazor/MudBlazor.min.js"></script>--> 
    <!-- Changer "_content" à "content" sinon les ficher des paquets ne sont pas reconnu dans la structure  -->

    <script src="content/Microsoft.Authentication.WebAssembly.Msal/AuthenticationService.js"></script>
    <script src="content/MudBlazor/MudBlazor.min.js"></script>
    <script src="js/login.js"></script>
    <script src="js/common.js"></script>
    <script src="js/cryptoProcess.js"></script>
    <script src="js/localStorage.js"></script>
</body>
</html>

````
## Examples Manifest V3 

```json
{
  "manifest_version": 3,
  "name": "PwdManager.chromium Extension",
  "description": "My browser extension built with Blazor WebAssembly",
  "version": "0.1",
  "background": {
    "service_worker": "BackgroundWorker.js",
    "type": "module"
  },
  "action": {
    "default_popup": "popup.html"
  },
  "options_ui": {
    "page": "options.html",
    "open_in_tab": true
  },
  "permissions": [
    "activeTab",
    "storage",
    "tabs",
    "identity", ///! important pour APIs Google Chrome Extensions
    "identity.email"
  ],
  "content_security_policy": {
    "extension_pages": "script-src 'self' 'wasm-unsafe-eval'; object-src 'self'"
  },
  "web_accessible_resources": [
    {
      "resources": [
        "framework/*",
        "content/*"
      ],
      "matches": [ "<all_urls>" ]
    }
  ]
}
```
## Debug dans navigateur Google Chrome

![Extension debug](https://dev.azure.com/yyang0113/2d0c4bb2-7a38-4246-8eca-94e06463547d/_apis/git/repositories/9a03a645-ffa4-46fc-90af-42590df12db7/items?path=/PwdManager.chromium/Extensions.png&versionDescriptor%5BversionOptions%5D=0&versionDescriptor%5BversionType%5D=0&versionDescriptor%5Bversion%5D=Shared&resolveLfs=true&%24format=octetStream&api-version=5.0)











