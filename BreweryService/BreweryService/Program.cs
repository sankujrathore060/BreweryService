using System;
using System.Runtime;
using Asp.Versioning;
using BreweryService.Common;
using BreweryService.Data.Repository;
using BreweryService.Middleware;
using BreweryService.Service;
using NLog.Extensions.Logging;
using NLog.Web;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    // Configure NLog
    builder.Services.AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
    });

    // Add NLog as the logger provider
    builder.Services.AddSingleton<ILoggerProvider, NLogLoggerProvider>();
    // Add services to the container.
    builder.Services.AddMemoryCache();
    builder.Services.AddHttpClient<IBreweryRepository, BreweryRepository>();
    builder.Services.AddScoped<IBreweryService, BreweryService.Service.BreweryService>();
    builder.Services.Configure<BreweryServiceAppSettings>(builder.Configuration.GetSection("BreweryServiceAppSettings"));


    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    });
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseMiddleware<BreweryServiceCorsMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
