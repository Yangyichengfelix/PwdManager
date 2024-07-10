using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using PwdManager.Shared.Data;
using PwdManager.Shared.Dtos.Coffres;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace PwdManager.spa.Services
{
    public class UserService : BaseService, IUserService
    {
      [NotNull]protected readonly HttpClient _httpClient;
      [NotNull]protected readonly IJSRuntime _jSRuntime;
      [NotNull]protected readonly IAccessTokenProvider _AuthorizationService;
      [NotNull]protected readonly IConfiguration? _configuration;
      [NotNull]protected readonly string? apiUrl;
        public UserService(IConfiguration configuration, HttpClient httpClient, IJSRuntime jSRuntime, IAccessTokenProvider AuthorizationService) : base(configuration,httpClient, jSRuntime, AuthorizationService)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
            _AuthorizationService = AuthorizationService;
            apiUrl = _configuration.GetValue<string>("PwdManager.srv") ?? throw new ArgumentNullException(nameof(apiUrl));
        }

        public async Task<HttpResponseMessage> Search(string keyword)
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


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{apiUrl}/api/user/search/{keyword}");

            request.SetBrowserResponseStreamingEnabled(true);

            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            return response;

        }
    }
}
