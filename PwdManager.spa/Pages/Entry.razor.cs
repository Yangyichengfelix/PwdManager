
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.Shared.Dtos.Entrees;
using PwdManager.spa.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PwdManager.spa.Pages
{
    public partial class Entry
    {
        [Parameter]
        public int Id { get; set; }
        [Inject, NotNull] public ICoffreService? _coffre { get; set; }
        [Inject, NotNull] public IEntreeService? _entree { get; set; }
        [Inject, NotNull] public NavigationManager? _navManager { get; set; }
        /// <summary>
        /// injection dépendance mud snackbar
        /// </summary>
        [Inject, NotNull] public ISnackbar? Snackbar { get; set; }


        #region Properties
        /// <summary>
        /// modèle d'inscription
        /// </summary>
        /// 
        public class EntreeCreate
        {

            public string LoginToEncrypt { get; set; } = "";
            public string PwdToEncrypt { get; set; } = "";
            public string UrlToEncrypt { get; set; } = "";
        }

        public EntreeCreateDto entreeCreate { get; set; } = new EntreeCreateDto();

        /// <summary>
        /// formulaire POST
        /// </summary>
        private MudForm form { get; set; }=new();

        private IJSObjectReference? jsModule { get; set; }
        private DotNetObjectReference<Entry>? objRef;

        #endregion

        public CoffreEntreeReadOnlyDto response { get; set; } = new();
        public string? IconString { get; set; }
        #region Methods

        /// <summary>
        /// Initialisation de conposant
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            try
            {
                response = await _coffre.GetCoffreById(Id);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Une erreur s'est produite lors de la récupération des coffres : {ex.Message}", Severity.Error);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./Pages/Entry.razor.js");
            objRef = DotNetObjectReference.Create(this);

            await base.OnParametersSetAsync();
        }

        private async Task EncryptLoginPwd()
        {
            // Appeler la fonction JavaScript pour chiffrer les données
            if (jsModule is not null)
            {

            entreeCreate = await jsModule.InvokeAsync<EntreeCreateDto>("encryptInfo",
            response.PasswordHash,response.Salt,
            LoginToEncrypt, PwdToEncrypt, UrlToEncrypt);
            }
            StateHasChanged();

        }
        private async Task ValidateEntree()
        {
            entreeCreate.CoffreId = Id;
            if (!string.IsNullOrEmpty(IconString))
            {
                entreeCreate.Icon = IconString;
            }
            var ok = await _entree.AddEntree(entreeCreate);

            if (ok.IsSuccessStatusCode)
            {
                System.Console.WriteLine("ok");
            }
        }
        //private async Task DecryptInfo()
        //{
        //    // Appeler la fonction JavaScript pour déchiffrer les données
        //    string decryptedLogin = await jsModule.InvokeAsync<string>("decryptData", ko.privateKey, ko.salt, em.ivLogin, em.Cipherlogin, em.loginTag);
        //    string decryptedPassword = await jsModule.InvokeAsync<string>("decryptData", ko.privateKey, ko.salt, em.ivPassword, em.Cipherpassword, em.passwordTag);
        //    string decryptedUrl = await jsModule.InvokeAsync<string>("decryptData", ko.privateKey, ko.salt, em.ivUrl, em.CipherUrl, em.urlTag);

        //    // Assigner les valeurs déchiffrées aux propriétés correspondantes
        //    em.DecryptedLogin = decryptedLogin;
        //    em.DecryptedPassword = decryptedPassword;
        //    em.DecryptedUrl = decryptedUrl;

        //    StateHasChanged();
        //}
        protected async Task IconSelect(string iconString)
        {
            IconString = iconString;
            StateHasChanged();
            await InvokeAsync(StateHasChanged);
        }
        #endregion
    }
}
