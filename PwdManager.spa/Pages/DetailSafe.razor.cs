using BootstrapBlazor.Components;
using DocumentFormat.OpenXml.Drawing.Diagrams;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using MudBlazor;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.Shared.Dtos.Entrees;
using PwdManager.spa.Models;
using PwdManager.spa.Services;
using PwdManager.spa.Shared.Dialogs;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using static PwdManager.spa.Pages.SafeCreate;
using Task = System.Threading.Tasks.Task;
namespace PwdManager.spa.Pages
{
    public partial class DetailSafe
    {
        #region Parameters

        [Parameter]
        public int Id { get; set; }
        #endregion
        #region DI
        [Inject, NotNull] private IDialogService? DialogService { get; set; }
        [Inject, NotNull] private ICoffreService? _coffreService { get; set; }

        [Inject, NotNull]  private IEntreeService? _entreeService { get; set; }

        [Inject, NotNull]  private ISnackbar? Snackbar { get; set; }
        [Inject, NotNull] private IJSRuntime? _js { get; set; }
        [Inject, NotNull] private KeyService? keyService { get; set; }
 
        [Inject, NotNull] private NavigationManager? Navigation { get; set; }
        #endregion
        public CoffreEntreeReadOnlyDto response { get; set; } = new CoffreEntreeReadOnlyDto();
        protected EntreeCreateModel entreeCreateModel { get; set; } = new EntreeCreateModel();
        public EntreeCreateDto entreeCreateDto { get; set; } = new EntreeCreateDto();
        protected CoffreUpdateModel coffreUpdateModel { get; set; } = new CoffreUpdateModel();
        public CoffreUpdateDto coffreUpdateDto { get; set; } = new CoffreUpdateDto();

        /// <summary>
        /// collapse pour la création entrée
        /// </summary>
        bool _expanded = false;
        string VaultIcon = "";
        bool loading = false;
        private bool isPwdShow;
        private bool isConfirmShow;
        private bool isUpdatePwdShow;
        private bool isUpdateConfirmShow;
        private MudForm UpdateForm = new();
        private bool UpdateValid;
        private InputType PasswordInput = InputType.Password;
        private InputType ConfirmInput = InputType.Password;
        private InputType UpdatePasswordInput = InputType.Password;
        private InputType UpdateConfirmInput = InputType.Password;
        private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        private string PasswordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;
        private string UpdatePasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        private string UpdatePasswordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;
        protected EncryptResult urlResult { get; set; } = new();
        protected EncryptResult loginResult { get; set; } = new();
        protected EncryptResult pwdResult { get; set; } = new();
        protected List<EntreeReadModel> readmodeldata { get; set; } = new();
        protected EntreeReadModel originalData { get; set; } = new();

        #region Local class

        protected class EncryptResult
        {
            public string? EncryptData { get; set; }
            public string? Tag { get; set; }
            public string? Iv { get; set; }
       
        }

        #endregion

        private Dictionary<string, ElementReference> selectComponentsText = new Dictionary<string, ElementReference>();
        private async Task SelectText(string id)
        {
            await _js.InvokeVoidAsync("copyText", id);
            Snackbar.Add("Password copied to clipborad!", Severity.Normal);

        }
        private MudTextField<string> keyGenReference = new();
        #region reload entries
        protected async Task ReloadEntry()
        {
            response = await _coffreService.GetCoffreById(Id)??throw new Exception("response Coffre return null");
            readmodeldata = new();
            coffreUpdateDto.Id = Id;
            coffreUpdateDto.Created = response.Created;
            coffreUpdateModel.Title = response.Title;
            coffreUpdateModel.Description = response.Description;
            if (response.Entrees.Count > 0)
            {
                var tasks = response.Entrees.Select(async f =>
                {
                    EntreeReadModel rm = new EntreeReadModel();
                    rm = await _js.InvokeAsync<EntreeReadModel>("decryptInfo", keyService.PrivateKey, response.Salt, f.EncryptedLogin, f.EncryptedPwd, f.EncryptedURL, f.IVLogin, f.IVPwd, f.IVUrl);
                    rm.visible = false;
                    rm.Id = f.Id;
                    rm.EncryptedLogin = f.EncryptedLogin;
                    rm.EncryptedPwd = f.EncryptedPwd;
                    if (!string.IsNullOrEmpty(f.EncryptedURL))
                    {
                        rm.EncryptedUrl = f.EncryptedURL;
                    }
                    if (!string.IsNullOrEmpty(f.Icon))
                    {
                        rm.Icon = f.Icon;
                    }
                    readmodeldata.Add(rm);
                }
                );
                await Task.WhenAll(tasks);
            }
        }
        #endregion
        #region Init

        protected override async Task OnInitializedAsync()
        {
            loading = true;
            try
            {
                System.Console.WriteLine(string.IsNullOrWhiteSpace(keyService.PrivateKey));
                response = await _coffreService.GetCoffreById(Id) ?? throw new Exception("response Coffre return null");
                if (string.IsNullOrWhiteSpace(keyService.PrivateKey))
                {
                    do
                    {
                        DialogParameters<DialogPwd> parameters = new DialogParameters<DialogPwd>();
                        parameters.Add(x => x.Hash, response.PasswordHash);
                        parameters.Add(x => x.Salt, response.Salt);
                        var dialog = await DialogService.ShowAsync<DialogPwd>("Type the password of this vault", parameters);
                        var result = await dialog.Result;           
                    } 
                    while (string.IsNullOrWhiteSpace(keyService.PrivateKey));
                }
                await ReloadEntry();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Une erreur s'est produite lors de la récupération des coffres : {ex.Message}", Severity.Error);
                Snackbar.Add($"Assurez vous que vous avez bien le droit de lecture du coffre : {ex.Message}", Severity.Warning);
            }
            loading = false ;

        }
        #endregion

        private void Add()
        {
            _expanded = !_expanded;
        }

        #region DELETE vault
        private async Task Delete()
        {
            DialogParameters<DialogRemoveVault> parameters = new DialogParameters<DialogRemoveVault>();
            parameters.Add(x => x.Hash, response.PasswordHash);
            parameters.Add(x => x.Salt, response.Salt);
            parameters.Add(x => x.Id, response.Id);
            var dialog = await DialogService.ShowAsync<DialogRemoveVault>($"Remove {response.Title}", parameters);
            var result = await dialog.Result;
            Navigation.NavigateTo("safe/list");
        }
        #endregion

        protected  HashAndSalt hashAndSalt { get; set; } = new HashAndSalt();

        private string keyString = "";
        private bool pwdReadOk = false;
        private IJSObjectReference? jsModule { get; set; } 
        private DotNetObjectReference<DetailSafe>? objRef;

        #region Reload vault

        #endregion
        #region Update vault
        private async Task UpdateVault()
        {
            loading = true;
            StateHasChanged();
            jsModule = await _js.InvokeAsync<IJSObjectReference>("import", "./Pages/SafeCreate.razor.js");
            objRef = DotNetObjectReference.Create(this);
            hashAndSalt = await _js.InvokeAsync<HashAndSalt>("hashPassword", coffreUpdateModel.Pwd);
            coffreUpdateDto.Salt = hashAndSalt.Salt;
            coffreUpdateDto.PasswordHash = hashAndSalt.HashedPassword;
            coffreUpdateDto.Description = coffreUpdateModel.Description;
            coffreUpdateDto.Title = coffreUpdateModel.Title;
            
            keyString = await _js.InvokeAsync<string>("generateKey", coffreUpdateModel.Pwd, coffreUpdateDto.Salt);
            keyService.PrivateKey = keyString;
            pwdReadOk = await _js.InvokeAsync<bool>("compareHashedPassword", coffreUpdateModel.Pwd, coffreUpdateDto.PasswordHash, coffreUpdateDto.Salt);

            HttpResponseMessage updated = await _coffreService.UpdateCoffre(coffreUpdateDto);
            await UpdateEntry();
            if (updated.IsSuccessStatusCode)
            {
                Snackbar.Add("Vault updated", Severity.Warning);
            }
            else
            {
                Snackbar.Add("Vault update falied", Severity.Error);
                Snackbar.Add($"{updated.StatusCode}, {updated.Content}", Severity.Error);
            }
            await OnInitializedAsync();

            loading = false;
            StateHasChanged();
        }
        #endregion
        #region UPDATE entry
        private async Task UpdateEntry()
        {
        
            if (readmodeldata.Count > 0)
            {
                
                var tasks = readmodeldata.Select(async f =>
                {
                    EntreeDto updateDto = new EntreeDto();
                    updateDto = await _js.InvokeAsync<EntreeDto>("encryptInfo", keyService.PrivateKey, coffreUpdateDto.Salt, f.Login, f.Pwd, f.Url);
                    updateDto.Id = f.Id;
                    updateDto.CoffreId = response.Id;
                    updateDto.Icon = f.Icon;
                    HttpResponseMessage updateEntryOk=await _entreeService.UpdateEntree(updateDto);
                    if (updateEntryOk.IsSuccessStatusCode)
                    {
                        Snackbar.Add($"{f.Url} of {response.Title} updated", Severity.Success);
                    }
                    else
                    {
                        Snackbar.Add($"{f.Url} of {response.Title} update failed", Severity.Warning);
                        Snackbar.Add($"{updateEntryOk.StatusCode}, {updateEntryOk.Content} update failed", Severity.Error);
                    }
                }
                );
                await Task.WhenAll(tasks);
            }
        }
        #endregion
        #region CREATE entry

        private async Task Create()
        {
            loading = true;
            StateHasChanged();
            entreeCreateDto =  await _js.InvokeAsync<EntreeCreateDto>("encryptInfo", keyService.PrivateKey, response.Salt, entreeCreateModel.Login, entreeCreateModel.Pwd, entreeCreateModel.Url);

            entreeCreateDto.CoffreId=response.Id;
            if (!string.IsNullOrEmpty(VaultIcon))
            {
                entreeCreateDto.Icon = VaultIcon;  /// affect icon string
            }

            StateHasChanged();

            HttpResponseMessage createOK= await _entreeService.AddEntree(entreeCreateDto);
            if (createOK.IsSuccessStatusCode)
            {
                Snackbar.Add("secret added", Severity.Info);
                //Navigation.NavigateTo($"/safe/list");
                await ReloadEntry();
                entreeCreateModel = new();
                StateHasChanged();
            }
            else
            {
                Snackbar.Add($"{createOK.StatusCode}, {createOK.Content}", Severity.Error);
                Snackbar.Add("you can't modify this vault", Severity.Error);

            }
            _expanded = !_expanded;
            entreeCreateDto = new();
            loading = false;
            StateHasChanged();

        }
        #endregion

        protected void IconSelected(string iconString)
        {
            VaultIcon = iconString;
            StateHasChanged();
        }
        private async Task Show()
        {
            try
            {
                System.Console.WriteLine("Avant l'appel à decryptInfo");
                originalData = await _js.InvokeAsync<EntreeReadModel>("decryptInfo", keyService.PrivateKey, response.Salt, entreeCreateDto.EncryptedLogin, entreeCreateDto.EncryptedPwd, entreeCreateDto.EncryptedURL, entreeCreateDto.IVLogin, entreeCreateDto.IVPwd, entreeCreateDto.IVUrl);
                System.Console.WriteLine("Après l'appel à decryptInfo");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Erreur dans la méthode Show : {ex.Message}");
            }
        }
        #region Update Dialog
        private async Task OpenUpdateDialog(EntreeReadModel entree)
        {
            DialogParameters<DialogEntry> parameters = new DialogParameters<DialogEntry>();
            parameters.Add(x => x.Entree, entree);
            parameters.Add(x => x.Salt, response.Salt);
            parameters.Add(x => x.CoffreId, response.Id);
            var dialog = await DialogService.ShowAsync<DialogEntry>($"{entree.Icon}", parameters);
            var result = await dialog.Result;
            if (!result.Canceled) {
                await ReloadEntry();
            }
        }

        #endregion
        #region Delete Entry

        private async Task DeleteEntry(int id)
        {
            var result = await _entreeService.DeleteEntree(id);
            if (result.IsSuccessStatusCode)
            {
                Snackbar.Add("Deleted secret", Severity.Normal);
                await ReloadEntry();
                StateHasChanged();
            }
            else
            {
                Snackbar.Add($"{result.StatusCode}, {result.Content}", Severity.Error);
            }
        }
        #endregion
        #region Check
        /// <summary>
        /// Change visibility of password input
        /// </summary>
        private void ChangeUpdatePwdVisibility()
        {
            if (isUpdatePwdShow)
            {
                isUpdatePwdShow = false;
                UpdatePasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                UpdatePasswordInput = InputType.Password;
            }
            else
            {
                isUpdatePwdShow = true;
                UpdatePasswordInputIcon = Icons.Material.Filled.Visibility;
                UpdatePasswordInput = InputType.Text;
            }
        }
        /// <summary>
        /// Change visibility of password confirm input
        /// </summary>
        private void ChangeUpdateConfirmVisibility()
        {
            if (isUpdateConfirmShow)
            {
                isUpdateConfirmShow = false;
                UpdatePasswordConfirmInputIcon = Icons.Material.Filled.VisibilityOff;
                UpdateConfirmInput = InputType.Password;
            }
            else
            {
                isUpdateConfirmShow = true;
                UpdatePasswordConfirmInputIcon = Icons.Material.Filled.Visibility;
                UpdateConfirmInput = InputType.Text;
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
        /// verify password strength
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        private IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Il faut un mot de passe!";
                yield break;
            }
            if (pw.Length < 8)
                yield return "Le mot de passe doit avoir minimum 8";
            if (pw.Length > 16)
                yield return "Le mot de passe doit avoir maximum 16";
            if (!Regex.IsMatch(pw, @"[A-Z]"))
                yield return "Le mot de passe doit avoir au moins 1 lettre majuscule";
            if (!Regex.IsMatch(pw, @"[a-z]"))
                yield return "Le mot de passe doit avoir au moins 1 lettre miniscule";
            if (!Regex.IsMatch(pw, @"[0-9]"))
                yield return "Le mot de passe doit avoir au moins 1 numéro";
            if (!Regex.IsMatch(pw, @"[\*@!#%&\(\)\^~{}\\.]"))
                yield return "le mdp doit contenir un caractère spécial,ex: *@!#%&\\()^~{}./";
        }
        /// <summary>
        /// verify if password matches with password confirm
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string? PasswordMatch(string arg)
        {
            if (entreeCreateModel.Pwd != arg)
                return "les deux saisies de mdp ne sont pas identiques";
            return null;
        }

        private string? UpdatePasswordMatch(string arg)
        {
            if (coffreUpdateModel.Pwd != arg)
                return "les deux saisies de mdp ne sont pas identiques";
            return null;
        }
        #endregion
    }
}
