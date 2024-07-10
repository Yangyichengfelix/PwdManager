using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using MudBlazor;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.spa.Services;
using PwdManager.spa.Shared.Dialogs;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PwdManager.spa.Pages
{
    public partial class Safe
    {
        #region DI

        [Inject ,NotNull]
         IDialogService? DialogService { get; set; }
        [Inject ,NotNull]
        private ICoffreService? _coffreService { get; set; }

        [Inject, NotNull]
        private ISnackbar? Snackbar { get; set; }
        [Inject ,NotNull]
        private HttpClient? _httpClient { get; set; }
        [Inject, NotNull]
        private IAccessTokenProvider? _AuthorizationService { get; set; }
        [Inject, NotNull]

        private Microsoft.Extensions.Configuration.IConfiguration? _configuration { get; set; }

        [Inject, NotNull]
        private NavigationManager? Navigation {  get; set; }
        #endregion
        public List<CoffreEntreeReadOnlyDto> coffres { get; set; } = new List<CoffreEntreeReadOnlyDto>();
        private string apiUrl = "";
        private bool loading = false;
        #region Init

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                loading = true;

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
                coffres = new List<CoffreEntreeReadOnlyDto>();
                StateHasChanged();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/api/coffre/user");
                request.SetBrowserResponseStreamingEnabled(true);
                using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using Stream responseStream = await response.Content.ReadAsStreamAsync();
                await foreach (CoffreEntreeReadOnlyDto? c in JsonSerializer.DeserializeAsyncEnumerable<CoffreEntreeReadOnlyDto>(
                    responseStream,
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
                loading = false;
                StateHasChanged();

            }
            catch (Exception ex)
            {
                Snackbar.Add($"Une erreur s'est produite lors de la récupération des coffres : {ex.Message}", Severity.Error);
            }
        }
        #endregion

        #region Navigation

        async Task Open(CoffreEntreeReadOnlyDto item)
        {
            var parameters = new DialogParameters<DialogPwd> ();
            parameters.Add(x => x.Hash, item.PasswordHash);
            parameters.Add(x => x.Salt, item.Salt);         
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth=true,Position= DialogPosition.BottomCenter };
            var dialog = await DialogService.ShowAsync<DialogPwd>("Entrez le mot de passe de coffre", parameters,options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
      
                Navigation.NavigateTo($"/safe/{item.Id}");
            }
        }
        private void ManageAccess()
        {
            try
            {
                Navigation.NavigateTo($"/safe/ManageAccess");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Une erreur s'est produite lors de la navigation vers la gestion d'accès : {ex.Message}", Severity.Error);
            }
        }

        private void Add()
        {
            try
            {
                Navigation.NavigateTo($"/safe/create");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Une erreur s'est produite lors de la navigation vers la création du coffre : {ex.Message}", Severity.Error);
            }
        }
        #endregion

    }
}