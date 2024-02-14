using BooksAPI;
using Serilog;
using Serilog.Events;

try
{
    Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    // Add this line:
    .WriteTo.File(
       System.IO.Path.Combine("logs", "diagnostics.txt"),
       rollingInterval: RollingInterval.Day,
       fileSizeLimitBytes: 10 * 1024 * 1024,
       retainedFileCountLimit: 2,
       rollOnFileSizeLimit: true,
       shared: true,
       flushToDiskInterval: TimeSpan.FromSeconds(1))
    .CreateLogger();
    Log.Information("\nStarting web application");

    var builder = WebApplication.CreateBuilder(args);
    builder.AddDependencyInjectionServices();

    builder.Host.UseSerilog(); // <-- Add this line

    var app = builder.Build();

    app.AddApplicationMiddlewares();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}