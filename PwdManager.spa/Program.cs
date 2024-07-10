using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using PwdManager.spa.Utilities;
using PwdManager.spa.HashCheckService;
using MudBlazor.Services;
using MudBlazor;
using PwdManager.spa.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PwdManager.spa.Provider;
using System.IdentityModel.Tokens.Jwt;

namespace PwdManager.spa
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddBootstrapBlazor();
            builder.Services.AddDensenExtensions();
            builder.Services.AddStorages();
            builder.Services.AddSingleton<HashServiceFactory>();
            builder.Services.AddSingleton<KeyService>();

            builder.Services.AddScoped<ApiAuthenticationStateProvider>();

            builder.Services.AddScoped<JwtSecurityTokenHandler>();

            builder.Services.AddScoped<ICoffreService, CoffreService>();
            builder.Services.AddScoped<IEntreeService, EntreeService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build());
            builder.Services.AddHttpClient("PwdManager.srv", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("PwdManager.srv") ?? throw new ArgumentException());
            });
            builder.Services.AddSingleton(services =>
            {
                // Get the service address from appsettings.json
                var config = services.GetRequiredService<IConfiguration>();
#if DEBUG

                var notificationUrl = config["notificationUrl80"];
#else
                var notificationUrl = config["notificationUrl"];
#endif      
                // If no address is set then fallback to the current webpage URL
                if (string.IsNullOrEmpty(notificationUrl))
                {
                    var navigationManager = services.GetRequiredService<NavigationManager>();
                    notificationUrl = navigationManager.BaseUri;
                }
                // Create a channel with a GrpcWebHandler that is addressed to the backend server.
                //
                // GrpcWebText is used because server streaming requires it. If server streaming is not used in your app
                // then GrpcWeb is recommended because it produces smaller messages.
                var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
                return GrpcChannel.ForAddress(notificationUrl, new GrpcChannelOptions { HttpHandler = httpHandler });
            });

            builder.Services.AddMudServices();
            builder.Services.AddMudMarkdownServices();
           
            builder.Services.AddMsalAuthentication(options =>  // Integrates authentication with the MSAL library
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                //options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("api://b456bb9c-0156-44b9-8760-61f5bc326637/coffre");
            });
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            await builder.Build().RunAsync();
        }
    }
}