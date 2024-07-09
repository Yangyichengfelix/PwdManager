using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using PwdManager.notif.Services;
using PwdManager.Shared.Data;

namespace PwdManager.notif
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel((context, options) =>
            {
                options.ListenAnyIP(80, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    //listenOptions.UseHttps();
                });
            });
            // Add services to the container.
            var connString = builder.Configuration.GetConnectionString("postgres");
            builder.Services.AddDbContextPool<GrpcDbContext>(options =>
            {
                //options.UseSqlite($"Data Source={DbPath}");
                options.UseNpgsql(connString);
                //options.UseSqlServer(connString); // sql server
            });
            builder.Services.AddGrpc();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll",
                    b => b
                       .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true) // allow any origin
                        .AllowCredentials()
                    );
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseRouting();
            app.UseGrpcWeb();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGrpcService<NotificationerService>().EnableGrpcWeb().RequireCors("AllowAll");
            app.MapGrpcService<CounterService>().EnableGrpcWeb().RequireCors("AllowAll");
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}