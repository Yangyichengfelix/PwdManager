using BootstrapBlazor.Components;
using JiuLing.CommonLibs.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using MudBlazor;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.CoffreLogs;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.spa.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PwdManager.spa.Pages
{
    public partial class LogView
    {
        [Inject, NotNull]
        private HttpClient? _httpClient { get; set; }
        [Inject, NotNull]
        private IAccessTokenProvider? _AuthorizationService { get; set; }
        [Inject, NotNull]
        private Microsoft.Extensions.Configuration.IConfiguration? _configuration { get; set; }
        private List<CoffreLogNotificationData>  coffreLogs = new List<CoffreLogNotificationData>();
        [ NotNull]  AccessTokenResult? accessTokenResult { get; set; }
        [ NotNull] AccessToken? AccessToken { get; set; }
        string[] vaultOperations = { };
        List<string> vaultOps = new List<string>();
        DateTime? date = DateTime.Today.AddDays(-1);
        private string apiUrl = "";
        private bool loading = false;
        private async Task GetLogs()
        {
            loading = true;
            StateHasChanged();
            vaultOperations = Array.Empty<string>();
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/api/coffrelog/{date}");
            //request.SetBrowserResponseStreamingEnabled(true);
            
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync<DateTime?>($"{apiUrl}/api/coffrelog",date);
            response.EnsureSuccessStatusCode();

            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            await foreach (CoffreLogNotificationData? c in JsonSerializer.DeserializeAsyncEnumerable<CoffreLogNotificationData>(
                responseStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultBufferSize = 128
                }))
            {
                if (c != null)
                {
                    var text = $"{c.AzureId} {c.Operation} {c.CoffreName} at {c.DateOperation}";
                    vaultOps.Add(text);
                    vaultOperations = vaultOps.ToArray();
                    await InvokeAsync(StateHasChanged);
                    //coffres.Add(c);
                }
                loading = false;
                StateHasChanged();
            }
        }
        protected override async Task OnParametersSetAsync()
        {         
            apiUrl = _configuration.GetValue<string>("PwdManager.srv") ?? throw new Exception("api setting is null");
            accessTokenResult = await _AuthorizationService.RequestAccessToken();
            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException( "Failed to provision the access token.");
            }
            AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("bearer", AccessToken.Value);

        }

    }
}
