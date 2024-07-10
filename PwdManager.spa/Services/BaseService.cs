using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

namespace PwdManager.spa.Services
{
    public class BaseService:IBaseService
    {
        protected readonly IConfiguration _configuration;
        protected readonly HttpClient _httpClient;
        protected readonly IJSRuntime _jSRuntime;
        protected readonly IAccessTokenProvider _AuthorizationService;
        protected readonly string? apiUrl;
        public BaseService(
            IConfiguration configuration,
            HttpClient httpClient,
            IJSRuntime jSRuntime,
            IAccessTokenProvider AuthorizationService
            )
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
            _AuthorizationService = AuthorizationService;
            _configuration= configuration;
            apiUrl = _configuration.GetValue<string>("PwdManager.srv") ?? throw new ArgumentNullException(nameof(apiUrl));
        }
    }
}
