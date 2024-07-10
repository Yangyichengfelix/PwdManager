using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos;
using PwdManager.Shared.Dtos.Coffres;
using PwdManager.Shared.Dtos.Entrees;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ZXing.Aztec.Internal;

namespace PwdManager.spa.Services
{
    public class CoffreService : BaseService, ICoffreService
    {
        protected new readonly HttpClient _httpClient;
        protected new readonly IJSRuntime _jSRuntime;
        protected new readonly IAccessTokenProvider _AuthorizationService;
        protected new readonly IConfiguration _configuration;
        protected new readonly string? apiUrl;
        public CoffreService(
            IConfiguration configuration, 
            HttpClient httpClient, 
            IJSRuntime jSRuntime, 
            IAccessTokenProvider AuthorizationService) : base(configuration,httpClient, jSRuntime, AuthorizationService)
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
            _AuthorizationService = AuthorizationService;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("PwdManager.srv")??throw new ArgumentNullException(nameof(apiUrl));
        }

        public async Task<List<CoffreEntreeReadOnlyDto>?> GetCoffres()
        {
            AccessTokenResult? accessTokenResult = await _AuthorizationService.RequestAccessToken();
            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException(
                    "Failed to provision the access token.");
            }
            AccessToken? AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);
            List<CoffreEntreeReadOnlyDto>? reponse = await _httpClient.GetFromJsonAsync<List<CoffreEntreeReadOnlyDto>>($"{apiUrl}/api/coffre");
            return reponse;
        }

        #region Get one by id
        public async Task<CoffreEntreeReadOnlyDto?> GetCoffreById(int id)
        {
            AccessTokenResult? accessTokenResult = await _AuthorizationService.RequestAccessToken();

            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException("Failed to provision the access token.");
            }
            AccessToken? AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);
            CoffreEntreeReadOnlyDto? reponse = await _httpClient.GetFromJsonAsync<CoffreEntreeReadOnlyDto>($"{apiUrl}/api/coffre/{id}");
            return reponse;
        }
        #endregion

        #region Delete

        public async Task<HttpResponseMessage> RemoveCoffre(int id)
        {
            AccessTokenResult? accessTokenResult = await _AuthorizationService.RequestAccessToken();

            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException("Failed to provision the access token.");
            }

            AccessToken? AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);

            HttpResponseMessage? response = await _httpClient.DeleteAsync($"{apiUrl}/api/coffre/{id}");

            response.EnsureSuccessStatusCode();
            return response;
        }
        #endregion
        #region Update

        public async Task<HttpResponseMessage> UpdateCoffre(CoffreUpdateDto coffreUpdateDto)
        {
            AccessTokenResult? accessTokenResult = await _AuthorizationService.RequestAccessToken();

            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException("Failed to provision the access token.");
            }

            AccessToken? AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);

            HttpResponseMessage? response = await _httpClient.PutAsJsonAsync($"{apiUrl}/api/coffre/{coffreUpdateDto.Id}", coffreUpdateDto);

            response.EnsureSuccessStatusCode();
            return response;
        }
        #endregion
        #region Create

        public async Task<HttpResponseMessage> AddCoffre(CoffreCreateDto coffreCreateDto)
        {
            AccessTokenResult? accessTokenResult = await _AuthorizationService.RequestAccessToken();

            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException("Failed to provision the access token.");
            }

            AccessToken? AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);

            HttpResponseMessage? response = await _httpClient.PostAsJsonAsync($"{apiUrl}/api/coffre", coffreCreateDto);

            response.EnsureSuccessStatusCode();
            return response;
        }
        #endregion

        public async Task AddEntree(EntreeCreateDto entreeCreateDto)
        {
            AccessTokenResult? accessTokenResult = await _AuthorizationService.RequestAccessToken();

            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException("Failed to provision the access token.");
            }
            AccessToken? AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);
            HttpResponseMessage? response = await _httpClient.PostAsJsonAsync($"{apiUrl}/api/entree", entreeCreateDto);
            response.EnsureSuccessStatusCode();
        }

        #region Get multiple relative

        public async Task<Stream> GetRelativeCoffres()
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/api/coffre/user");
            request.SetBrowserResponseStreamingEnabled(true);
            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
             return await response.Content.ReadAsStreamAsync();
        }
        #endregion
    }
}
