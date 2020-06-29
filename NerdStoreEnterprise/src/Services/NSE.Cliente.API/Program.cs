using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NSE.WebAPI.Core.Log;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace NSE.Clientes.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = LoggerFactory.Factory();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
    }
}
