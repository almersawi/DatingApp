using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var Services = scope.ServiceProvider;

            try 
            {
                var context = Services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync(); // to update database every time the program srarts
                await seed.SeedUsers(context);
            }
            catch (Exception ex)
            {
                var Logger = Services.GetRequiredService<ILogger<Program>>();
                Logger.LogError(ex, "Error occured while migrate database");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
