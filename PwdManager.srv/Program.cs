using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using PwdManager.srv.Configs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using PwdManager.srv.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PwdManager.srv.Models;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Hosting;
using PwdManager.srv.Hubs;
using PwdManager.srv.Contracts;
using PwdManager.srv.Services;
using System;
using System.Text.Json.Serialization;
namespace PwdManager.srv
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
           

            //var folder = Environment.SpecialFolder.LocalApplicationData;
            //var path = Environment.GetFolderPath(folder);
            //var DbPath = System.IO.Path.Join(path, "pwdman.db");

            var connString = builder.Configuration.GetConnectionString("postgres");
            //var connString = builder.Configuration.GetConnectionString("msqql"); // sql server
            // Add services to the container.
            builder.Services.AddSignalR();
            builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.AddAutoMapper(typeof(MapperConfig));
            builder.Services.AddDbContextPool<PwdDbContext>(options =>
            {
                //options.UseSqlite($"Data Source={DbPath}");
       
                options.UseNpgsql(connString);
                //options.UseSqlServer(connString); // sql server
            });


            //  builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            //.AddMicrosoftIdentityWebApp(builder.Configuration, "AzureAd")
            //.EnableTokenAcquisitionToCallDownstreamApi(new string[] { "user.read" })
            //.AddMicrosoftGraph(builder.Configuration.GetSection("GraphBeta"))
            //.AddInMemoryTokenCaches();
            //builder.Services.AddAuthentication(AzureADDefaults.JwtBearerAuthenticationScheme)
            //    .AddAzureADBearer(options => builder.Configuration.Bind("AzureAd", options));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
            builder.Services.AddSingleton(new Appsettings(builder.Environment.ContentRootPath));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                    new string[] {}
                }
                 });
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            //builder.Services.AddSwaggerGen(
            //    options => {
            //    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "pwdman", Version = "v1" });
            //    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            //    {
            //        Description = "JWT Bearer {token}\"",
            //        Type = SecuritySchemeType.OAuth2,
            //        In = ParameterLocation.Header,
            //        Flows = new OpenApiOAuthFlows()
            //        {
            //            Implicit = new OpenApiOAuthFlow
            //            {
            //                Scopes = new Dictionary<string, string>
            //                {
            //                    { "user_impersonation", "Access API" }
            //                },
            //                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{Appsettings.app(new string[] { "AzureAD", "TenantId" })}/oauth2/authorize")
            //            }
            //        }
            //    });
            //    // ��header������token�����ݵ���̨
            //    options.OperationFilter<SecurityRequirementsOperationFilter>();
            //}
            //);
            builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddTransient<IAuthorizationRepo, AuthorizationRepo>();
            builder.Services.AddTransient<IUserRepo, UserRepo>();
            builder.Services.AddTransient<ICoffreRepo, CoffreRepo>();
            builder.Services.AddTransient<ICoffreLogRepo, CoffreLogRepo>();
            builder.Services.AddScoped<IUserCoffreRepo, UserCoffreRepo>();
            builder.Services.AddTransient<IEntreeRepo, EntreeRepo>();
            builder.Services.AddTransient<IEntreeLogRepo, EntreeLogRepo>();

            builder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build());
        
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyMethod()
                    .AllowAnyHeader()
                     //.SetIsOriginAllowed(origin => true) // allow any origin
                     .WithOrigins(
                        builder.Configuration["allowOrigins:githublink"]??throw new NullReferenceException(), 
                        builder.Configuration["allowOrigins:ducklink"] ?? throw new NullReferenceException()
                     )
                    .WithExposedHeaders("X-Pagination")
                       );
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                #region Swagger
                app.UseSwagger();
                app.UseSwaggerUI(
                //    c =>
                //{
                    
                //    var ApiName = Appsettings.app(new string[] { "Startup", "ApiName" });
                //    c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"{ApiName} v1");

                //    c.OAuthClientId(Appsettings.app(new string[] { "Swagger", "ClientId" }));
                //    //c.OAuthClientSecret(Appsettings.app(new string[] { "Swagger", "ClientSecret" }));
                //    c.OAuthRealm(Appsettings.app(new string[] { "AzureAD", "ClientId" }));
                //    c.OAuthAppName("pwdman API V1");
                //    c.OAuthScopeSeparator(" ");
                //    c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>() { { "resource", Appsettings.app(new string[] { "AzureAD", "ClientId" }) } });
                //}
                );
                #endregion
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization(); 

            app.MapHub<FileTransferHub>("/file-transfer-hub");
            app.MapControllers();

            app.Run();
        }

    }
}
