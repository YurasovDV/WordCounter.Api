﻿using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .ConfigureServices(services =>
                {
                    services.AddMvc();
                    services.AddRouting();
                    services.AddSingleton<IMessageSender, MessageSender>();
                    services.AddSingleton<IEnvironmentFacade, EnvironmentFacade>();
                    services.AddSingleton<Connector, Connector>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            }).Build().Run();
        }
    }
}
