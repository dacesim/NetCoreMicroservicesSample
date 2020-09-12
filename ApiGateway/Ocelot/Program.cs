using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var ocelotJson = new JObject();
                    foreach (var jsonFilename in Directory.EnumerateFiles("Configuration", "ocelot.*.json", SearchOption.AllDirectories))
                    {
                        using (StreamReader fi = File.OpenText(jsonFilename))
                        {
                            var json = JObject.Parse(fi.ReadToEnd());
                            ocelotJson.Merge(json, new JsonMergeSettings
                            {
                                MergeArrayHandling = MergeArrayHandling.Merge
                            });
                        }
                    }

                    File.WriteAllText("ocelot.json", ocelotJson.ToString());

                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
