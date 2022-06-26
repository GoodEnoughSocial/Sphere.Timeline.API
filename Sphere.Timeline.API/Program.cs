using Serilog;
using Sphere.Shared;

// Setting this allows us to get some benefits all over the place.
Services.Current = Services.Timeline;

Log.Logger = SphericalLogger.StartupLogger(Services.Current);

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(SphericalLogger.SetupLogger);
    builder.Services.AddInjectableOrleansClient();
    builder.Services.AddHealthChecks();

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    app.MapHealthChecks(Constants.HealthCheckEndpoint);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    if (ex.GetType().Name != "StopTheHostException")
    {
        Log.Fatal(ex, "Unhandled exception");
    }
}
finally
{
    Log.Information("Shutting down");
    Log.CloseAndFlush();
}
