using FluentMigrator.Runner;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using StandardAPI.API.Middleware;
using StandardAPI.Application.Extensions;
using StandardAPI.Infraestructure.Extensions;
using StandardAPI.Infraestructure.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration["Redis:ConnectionString"]!, name: "Redis", failureStatus: HealthStatus.Degraded) // Check Redis
    .AddNpgSql( // Check CRDB
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "CockroachDB",
        healthQuery: "SELECT 1;", // Simple health check query
        timeout: TimeSpan.FromSeconds(10),
        failureStatus: HealthStatus.Degraded)
    .AddCheck("Self", () => HealthCheckResult.Healthy());

builder.Services.AddMigrationRunner(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

// Enable Serilog request logging
app.UseSerilogRequestLogging();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


// Run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp(); // Run all pending migrations
}

await app.RunAsync();
