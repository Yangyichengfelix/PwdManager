using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PwdManager.Shared.Dtos.Coffres;
using System.Net.Http.Headers;
using System.Text.Json;
using PwdManager.Shared.Data;
using Microsoft.AspNetCore.Components.Web;
using PwdManager.Shared.Dtos.UserCoffres;
using PwdManager.Shared;
using AME;
using PwdManager.Shared.Dtos.Entrees;
using System.Net.Http.Json;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using CsvHelper.Configuration;


namespace PwdManager.spa.Pages
{
    public partial class ManageAccess
    {
        #region DI
        [Inject,NotNull]
        private HttpClient? _httpClient { get; set; }
        [Inject, NotNull]
        private IAccessTokenProvider? _AuthorizationService { get; set; }
        [Inject, NotNull]
        private Microsoft.Extensions.Configuration.IConfiguration? _configuration { get; set; }
        [Inject, NotNull]
        private ISnackbar? Snackbar { get; set; }
        [Inject, NotNull]
        private NavigationManager? Navigation { get; set; }
        #endregion

        public List<ApiUser> users { get; set; } = new List<ApiUser>();
        private string apiurl = "";
        private bool _expended=false;
        private CancellationTokenSource? cts { get; set; } = new();
        protected ApiUser SelectedUser { get; set; } = new  ApiUser { AzureId="",UserId="", ApiUserCoffres=new List<ApiUserCoffre>()};
        protected CoffreEntreeReadOnlyDto SelectedCoffre { get; set; } = new ();
        protected AzureCoffreAccessDto azureCoffreAccessDto { get; set; } = new AzureCoffreAccessDto { AzureId="", Access=Access.R};
        public List<CoffreEntreeReadOnlyDto> coffres { get; set; } = new List<CoffreEntreeReadOnlyDto>();
        private string apiUrl = "";
        #region component init
        protected override async Task OnParametersSetAsync()
        {

            #region Init accessToken
            apiUrl = _configuration.GetValue<string>("PwdManager.srv")??throw new Exception("api setting is null");
            var accessTokenResult = await _AuthorizationService.RequestAccessToken();
            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException(
                    "Failed to provision the access token.");
            }
            var AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);

            coffres = new List<CoffreEntreeReadOnlyDto>();
            users = new List<ApiUser>();
            StateHasChanged();

            #endregion

            #region coffre
            try
            {
                CancellationTokenSource ctsv = new CancellationTokenSource();
                apiUrl = _configuration.GetValue<string>("PwdManager.srv") ?? throw new Exception("api setting is null");
                HttpRequestMessage requestVault = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/api/coffre/user");
                requestVault.SetBrowserResponseStreamingEnabled(true);
                using HttpResponseMessage responseVault = await _httpClient.SendAsync(requestVault, HttpCompletionOption.ResponseHeadersRead);
                responseVault.EnsureSuccessStatusCode();
                using Stream responseStreamVault = await responseVault.Content.ReadAsStreamAsync();
                await foreach (CoffreEntreeReadOnlyDto? c in JsonSerializer.DeserializeAsyncEnumerable<CoffreEntreeReadOnlyDto>(
                    responseStreamVault,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultBufferSize = 128
                    }))
                {
                    if (c != null)
                    {

                        coffres.Add(c);
                    }
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Une erreur s'est produite lors de la récupération des coffres : {ex.Message}", Severity.Error);
            }
            #endregion

        }
        #endregion
        #region grant access
        protected  async Task Grant()
        {
            if (
                !string.IsNullOrEmpty(azureCoffreAccessDto.AzureId)||
                azureCoffreAccessDto.CoffreId>0
                )
            {
                try
                {
                    CancellationTokenSource cts = new CancellationTokenSource();
                    var accessTokenResult = await _AuthorizationService.RequestAccessToken();
                    if (!accessTokenResult.TryGetToken(out var token))
                    {
                        throw new InvalidOperationException(
                            "Failed to provision the access token.");
                    }
                    var AccessToken = token;

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("bearer", AccessToken.Value);
                    apiUrl = _configuration.GetValue<string>("PwdManager.srv") ?? throw new Exception("api setting is null");

                    var response = await _httpClient.PostAsJsonAsync($"{apiUrl}/api/accessmanage", azureCoffreAccessDto);

                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        Snackbar.Add($"Grant {azureCoffreAccessDto.Access} access to {azureCoffreAccessDto.AzureId} on {SelectedCoffre.Title}", Severity.Info);
                        azureCoffreAccessDto = new AzureCoffreAccessDto { AzureId = "", Access = Access.R };
                        SelectedCoffre = new();
                        await FetchUser();
                        SelectedUser = new ApiUser { AzureId = "", UserId = "" };
                    }
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Une erreur s'est produite lors de affectation de droit d'accès pour {azureCoffreAccessDto.AzureId} sur {SelectedCoffre.Title}: {ex.Message}", Severity.Error);
                }
            }

        }

        #endregion
        #region Refresh
        protected IDictionary<string, object> GetAccessTokenClaims(AccessToken AccessToken)
        {
            if (AccessToken == null)
            {
                return new Dictionary<string, object>();
            }

            // header.payload.signature
            string payload = AccessToken.Value.Split(".")[1];
            string base64Payload = payload.Replace('-', '+').Replace('_', '/')
                .PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            byte[] buffers = Convert.FromBase64String(base64Payload);
            var result = JsonSerializer.Deserialize<IDictionary<string, object>>(buffers);
            if (result is null)
            {
                return new Dictionary<string, object>();
            }
            return result;
        }
        private async Task FetchUser()
        {
            StopCount();
            cts = new CancellationTokenSource();

            try
            {
                var accessTokenResult = await _AuthorizationService.RequestAccessToken();
                if (!accessTokenResult.TryGetToken(out var token))
                {
                    throw new InvalidOperationException(
                        "Failed to provision the access token.");
                }
                var AccessToken = token;

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", AccessToken.Value);
                apiurl = _configuration.GetValue<string>("PwdManager.srv") ?? throw new Exception("api setting is null");
                users = new List<ApiUser>();
                StateHasChanged();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{apiurl}/api/user/search?keyword={SelectedUser.AzureId}");
                request.SetBrowserResponseStreamingEnabled(true);
                using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                response.EnsureSuccessStatusCode();
                using Stream responseStream = await response.Content.ReadAsStreamAsync();
                await foreach (ApiUser? u in JsonSerializer.DeserializeAsyncEnumerable<ApiUser>(
                    responseStream,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultBufferSize = 128
                    }))
                {
                    if (u != null )
                    {
                        users.Add(u);
                    }
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Une erreur s'est produite lors de la récupération des utilisateur : {ex.Message}", Severity.Error);
            }
        }
        #endregion
        #region Search

        private void StopCount()
        {
            cts?.Cancel();
            cts = null;
        }
        protected async Task InputChanged(string searchWord)
        {
            if (!string.IsNullOrWhiteSpace(searchWord))
            {
                _expended= true;
                StopCount();
                cts = new CancellationTokenSource();
                try
                {
                    var accessTokenResult = await _AuthorizationService.RequestAccessToken();
                    if (!accessTokenResult.TryGetToken(out var token))
                    {
                        throw new InvalidOperationException(
                            "Failed to provision the access token.");
                    }
                    AccessToken AccessToken = token;

                    var azureName= GetAccessTokenClaims(AccessToken).FirstOrDefault(w=>w.Key.Contains("unique_name"));

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("bearer", AccessToken.Value);
                    apiurl = _configuration.GetValue<string>("PwdManager.srv") ?? throw new Exception("api setting is null");
                    users = new List<ApiUser>();
                    StateHasChanged();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{apiurl}/api/user/search?keyword={searchWord}");
                    request.SetBrowserResponseStreamingEnabled(true);
                    using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                    response.EnsureSuccessStatusCode();
                    using Stream responseStream = await response.Content.ReadAsStreamAsync();
                    await foreach (ApiUser? u in JsonSerializer.DeserializeAsyncEnumerable<ApiUser>(
                        responseStream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            DefaultBufferSize = 128
                        }))
                    {
                        if (u !=null&& azureName.Value.ToString()!= u.AzureId)
                        {
                            users.Add(u);
                        }
                        StateHasChanged();
                    }
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Une erreur s'est produite lors de la récupération des utilisateur : {ex.Message}", Severity.Error);
                }
            }
            else
            {
                _expended = false;
            }
     
        }

        #endregion

        #region ApiUser table
   
        private MudTable<ApiUser> userTable = new();
        private void RowClickEvent(TableRowClickEventArgs<ApiUser> tableRowClickEventArgs)
        {
            if (SelectedUser == tableRowClickEventArgs.Item)
            {   
                azureCoffreAccessDto.AzureId = "";
                SelectedUser = new ApiUser { AzureId = "", UserId="" };
            }
            else
            {
                SelectedUser = tableRowClickEventArgs.Item;
                azureCoffreAccessDto.AzureId = tableRowClickEventArgs.Item.AzureId;
            }
            StateHasChanged();

        }
        #endregion
        #region CoffreEntreeReadOnlyDto table
        private int selectedVaultRowNumber = -2;
        private MudTable<CoffreEntreeReadOnlyDto> coffreTable= new();

        private void VaultRowClickEvent(TableRowClickEventArgs<CoffreEntreeReadOnlyDto> tableRowClickEventArgs)
        {
            if (SelectedCoffre == tableRowClickEventArgs.Item)
            {
                SelectedCoffre = new CoffreEntreeReadOnlyDto();
                azureCoffreAccessDto.CoffreId = 0;



            }
            else
            {
                SelectedCoffre = tableRowClickEventArgs.Item;
                azureCoffreAccessDto.CoffreId = tableRowClickEventArgs.Item.Id;
   
            }
            StateHasChanged();

        }
        #endregion

        private string SelectedVaultRowClassFunc(CoffreEntreeReadOnlyDto element, int rowNumber)
        {
            
            if (selectedVaultRowNumber == rowNumber)
            {
                selectedVaultRowNumber = -2;    
                return string.Empty;
            }
            else if (coffreTable.SelectedItem != null && coffreTable.SelectedItem.Equals(element))
            {
                selectedVaultRowNumber = rowNumber;         
                return "selectedVault";
            }
            else
            {
                return "default";
            }
        }

    }
}
