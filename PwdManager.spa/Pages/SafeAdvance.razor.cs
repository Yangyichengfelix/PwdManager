using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PwdManager.spa.Pages
{
    public partial class SafeAdvance
    {
        [Inject, NotNull]
        public NavigationManager? _navManager { get; set; }
        /// <summary>
        /// injection dépendance mud snackbar
        /// </summary>
        [Inject, NotNull]
        public ISnackbar? Snackbar { get; set; }


        #region Properties
        /// <summary>
        /// modèle d'inscription
        /// </summary>
        /// 
        public class EntreeCreate
        {
            public string? Url { get; set; }
            [Required]

            public string Login { get; set; } = "login";
            [Required]
            public string Password { get; set; } = "P@ssword1";
            [Required]

            public string PasswordConfirm { get; set; } = "P@ssword1";
        }
        public EntreeCreate entreeCreate { get; set; } = new EntreeCreate();
        private bool success;
    
   
        /// <summary>
        /// formulaire POST
        /// </summary>
        private MudForm form =new();

        public string PasswordSample { get; set; } = "Minimum 8 caractères, dont 1 chiffre, 1 majuscule et 1 caractère spécial";


        private bool isPwdShow;
        private bool isConfirmShow;
        private InputType PasswordInput = InputType.Password;
        private InputType ConfirmInput = InputType.Password;
        private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        private string PasswordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;


        private IJSObjectReference? jsModule { get; set; }
        private DotNetObjectReference<SafeAdvance>? objRef;

        #endregion

        #region Methods

        /// <summary>
        /// Initialisation de conposant
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
        }



        protected override async Task OnParametersSetAsync()
        {
            jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./Pages/SafeAdvance.razor.js");
            objRef = DotNetObjectReference.Create(this);

            await base.OnParametersSetAsync();
        }
        private async Task GeneratePrivateKey()
        {
            // Appeler la fonction JavaScript pour générer la clé
            if (jsModule !=null)
            {

            ko = await jsModule.InvokeAsync<KeyObj>("generatePrivateKeyAndSalt", entreeCreate.Password);
            }
            StateHasChanged();
            System.Console.WriteLine(ko.privateKey);
            System.Console.WriteLine(ko.salt);

        }

        private async Task EncryptLoginPwd()
        {
            // Appeler la fonction JavaScript pour chiffrer les données
            if (jsModule!=null)
            {

            em = await jsModule.InvokeAsync<EntreeModel>("encryptInfo",
            ko.privateKey, ko.salt, 
            LoginToEncrypt, PwdToEncrypt);
            }
            StateHasChanged();

        }

        private async Task DecryptInfo()
        {
            // Appeler la fonction JavaScript pour chiffrer les données
            await JSRuntime.InvokeVoidAsync("decryptData", ko.privateKey, ko.salt, em.ivLogin,em.Cipherlogin, em.loginTag);
            StateHasChanged();

        }


        /// <summary>
        /// Change visibility of password input
        /// </summary>
        private void ChangePwdVisibility()
        {
            if (isPwdShow)
            {
                isPwdShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isPwdShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }
        /// <summary>
        /// Change visibility of password confirm input
        /// </summary>
        private void ChangeConfirmVisibility()
        {
            if (isConfirmShow)
            {
                isConfirmShow = false;
                PasswordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;
                ConfirmInput = InputType.Password;
            }
            else
            {
                isConfirmShow = true;
                PasswordConfirmInputIcon = Icons.Material.Filled.Visibility;
                ConfirmInput = InputType.Text;
            }
        }


        /// <summary>
        /// verify if password matches with password confirm
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string? PasswordMatch(string arg)
        {
            if (entreeCreate.Password != arg)
                return "les deux saisies de mdp ne sont pas identiques";
            return null;
        }
        #endregion

    }
}
