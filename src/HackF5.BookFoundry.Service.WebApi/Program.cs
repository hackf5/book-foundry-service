using Articulum.Core.WebApi;

using dotenv.net;

using HackF5.BookFoundry.Service.WebApi.Data;
using HackF5.BookFoundry.Service.WebApi.GraphQL;

DotEnv
    .Fluent()
    .WithProbeForEnv(15)
    .Load();

SerilogUtilities.InitializeLogger();
SerilogUtilities.ExcludeRequestPathPredicate = path => path.StartsWith("/graphql", StringComparison.Ordinal);

var connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        $"Connection string environment variable not defined 'POSTGRESQL_CONNECTION_STRING'.");

var builder = WebApplication.CreateBuilder(args);

builder.UseSerilog();

builder
    .Services
    .AddCors()
    .UseMinimalHttpClientLogging()
    .AddNpgsqlDbContext<ApplicationDbContext>(
        connectionString,
        new()
        {
            NpgsqlOptionsAction = o => { },
            EnableSensitiveDataLogging = true,
        })
    .AddGraphQLServer()
    .BuildDefaultSchema();

var app = builder.Build();

app.UseCors(
    x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app
    .UseHttpsRedirection()
    .UseRouting()
    .UseSerilog()
    .UseEndpoints(eb => eb.MapGraphQL());

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    if (Environment.GetEnvironmentVariable("ENSURE_DATABASE_CREATED") is not null)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}

await app.StartAsync();

await app.WaitForShutdownAsync();