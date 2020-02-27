using Microsoft.Extensions.Hosting;
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
                    services.AddDbContext<CountResultsContext>();
                    services.AddTransient<IWordCountersRepository, WordCountersRepository>();
                    services.AddTransient<ICounterRequestRepository, CounterRequestRepository>();
                    services.AddTransient<ICountersService, CountersService>();
                    services.AddCors(opts => opts.AddPolicy("my-cors-policy", builder => { builder.AllowAnyOrigin().AllowAnyHeader(); }));
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                }).Configure(app =>
                {
                    app.UseRouting();
                    app.UseCors("my-cors-policy");
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            }).Build().Run();
        }
    }
}
