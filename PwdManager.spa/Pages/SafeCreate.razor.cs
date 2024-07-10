using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.spa.Models;
using PwdManager.spa.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;


namespace PwdManager.spa.Pages
{
    public partial class SafeCreate
    {
        [Inject, NotNull] ICoffreService? _coffre { get; set; }
        [Inject, NotNull] NavigationManager? _navManager { get; set; }
        /// <summary>
        /// injection dépendance mud snackbar
        /// </summary>
        [Inject, NotNull] ISnackbar? _Snackbar { get; set; }
        [Inject, NotNull] IJSRuntime? _js { get; set; }

        #region Properties
        /// <summary>
        /// modèle d'inscription
        /// </summary>
        /// 
        public class EntreeCreate
        {

            public string Password { get; set; } = "P@ssword1";
            [Required]

            public string PasswordConfirm { get; set; } = "P@ssword1";
        }

        public EntreeCreate entreeCreate { get; set; } = new EntreeCreate();
        private bool success;

        private string[] errors = { };
        /// <summary>
        /// formulaire POST
        /// </summary>
        private MudForm form = new();

        public string PasswordSample { get; set; } = "Minimum 8 caractères, dont 1 chiffre, 1 majuscule et 1 caractère spécial";


        private bool isPwdShow;
        private bool isConfirmShow;
        private InputType PasswordInput = InputType.Password;
        private InputType ConfirmInput = InputType.Password;
        private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        private string PasswordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;



        private string keyString = "";
        private bool pwdReadOk=false;
        private IJSObjectReference? jsModule { get; set; }
        private DotNetObjectReference<SafeCreate>? objRef;

        #endregion

        public CoffreCreateDto coffreCreate { get; set; } = new CoffreCreateDto();
        public HashAndSalt hashAndSalt { get; set; } = new HashAndSalt();



        #region Methods

        /// <summary>
        /// Initialisation de conposant
        /// </summary>
        /// <returns></returns>





        protected override async Task OnParametersSetAsync()
        {
            jsModule = await _js.InvokeAsync<IJSObjectReference>(
                "import", "./Pages/SafeCreate.razor.js");
            objRef = DotNetObjectReference.Create(this);

            await base.OnParametersSetAsync();
        }

        private async   Task GenerateHashedPwd()
        {
            hashAndSalt= await _js.InvokeAsync<HashAndSalt>("hashPassword", entreeCreate.Password);
            coffreCreate.Salt = hashAndSalt.Salt;
            coffreCreate.PasswordHash = hashAndSalt.HashedPassword;


            keyString = await _js.InvokeAsync<string>("generateKey", entreeCreate.Password, coffreCreate.Salt);

            pwdReadOk = await _js.InvokeAsync<bool>("compareHashedPassword", entreeCreate.Password, coffreCreate.PasswordHash, coffreCreate.Salt);

   

        }

        private async Task Send()
        {

            var result = await _coffre.AddCoffre(coffreCreate);
            if (result.IsSuccessStatusCode)
            {
                _Snackbar.Add("vault created", Severity.Success);
                _navManager.NavigateTo("/Safe/list");
            }
            else
            {
                _Snackbar.Add($"{result.StatusCode},{result.Content}", Severity.Error);

            }
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
