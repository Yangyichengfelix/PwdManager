using Microsoft.Extensions.Configuration.Json;

namespace PwdManager.srv.Configs
{
    public class Appsettings
    {
        static IConfiguration? Configuration { get; set; }
        static string contentPath { get; set; } = "";

        public Appsettings(string contentPath)
        {
            string Path = "appsettings.json";


            //appsetting en fonction d'environnement
            {
                //string Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";
            }


            //Configuration = new ConfigurationBuilder()
            //.Add(new JsonConfigurationSource { Path = Path, ReloadOnChange = true })
            //.Build();


            //var contentPath = env.ContentRootPath;
            Configuration = new ConfigurationBuilder()
               .SetBasePath(contentPath)
               .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })
               .Build();


        }

        /// <summary>
        /// concat string
        /// </summary>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static string app(params string[] sections)
        {
            try
            {
                var val = string.Empty;
                for (int i = 0; i < sections.Length; i++)
                {
                    val += sections[i] + ":";
                }

                return Configuration?[val.TrimEnd(':')]??throw new Exception("configuration appsetting is null");
            }
            catch (Exception)
            {
                return "";
            }

        }
    }
}
