using Common.Logging;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseSerilog(SeriLogger.Configure);

builder.Configuration
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);

builder.Services
    .AddOcelot()
        .AddCacheManager(
            settings => settings.WithDictionaryHandle());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

await app.UseOcelot();

app.Run();