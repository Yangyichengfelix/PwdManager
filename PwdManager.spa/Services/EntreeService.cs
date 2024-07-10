using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using PwdManager.Shared.Dtos.Entrees;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ZXing.Aztec.Internal;

namespace PwdManager.spa.Services
{
    public class EntreeService : BaseService, IEntreeService
    {
        protected new readonly HttpClient _httpClient;
        protected new readonly IJSRuntime _jSRuntime;
        protected new readonly IAccessTokenProvider _AuthorizationService;
        protected new readonly IConfiguration _configuration;
        protected new readonly string? apiUrl;
        public EntreeService(IConfiguration configuration, HttpClient httpClient, IJSRuntime jSRuntime, IAccessTokenProvider AuthorizationService) : base(configuration,httpClient, jSRuntime, AuthorizationService)
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
            _AuthorizationService = AuthorizationService;
            _configuration = configuration;
            apiUrl = _configuration.GetValue<string>("PwdManager.srv") ?? throw new ArgumentNullException(nameof(apiUrl));
        }

        public async Task<HttpResponseMessage> AddEntree(EntreeCreateDto entreeCreateDto)
        {
            var accessTokenResult = await _AuthorizationService.RequestAccessToken();

            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException("Failed to provision the access token.");
            }

            var AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);

            var response = await _httpClient.PostAsJsonAsync($"{apiUrl}/api/Entree", entreeCreateDto);

            response.EnsureSuccessStatusCode();

            return response;
          
        }

        public async Task<HttpResponseMessage> DeleteEntree(int id)
        {
            var accessTokenResult = await _AuthorizationService.RequestAccessToken();

            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException("Failed to provision the access token.");
            }

            var AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);

            var response = await _httpClient.DeleteAsync($"{apiUrl}/api/Entree/{id}");

            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> UpdateEntree(EntreeDto entreeUpdateDto)
        {
            var accessTokenResult = await _AuthorizationService.RequestAccessToken();

            if (!accessTokenResult.TryGetToken(out var token))
            {
                throw new InvalidOperationException("Failed to provision the access token.");
            }

            var AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", AccessToken.Value);

            var response = await _httpClient.PutAsJsonAsync($"{apiUrl}/api/Entree/{entreeUpdateDto.Id}", entreeUpdateDto);

            response.EnsureSuccessStatusCode();

            return response;

        }
    }
}
