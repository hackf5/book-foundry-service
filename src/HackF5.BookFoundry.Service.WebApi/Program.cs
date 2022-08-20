using Articulum.Core.WebApi;

using dotenv.net;

using HackF5.BookFoundry.Service.WebApi.Data;
using HackF5.BookFoundry.Service.WebApi.GraphQL;

DotEnv
    .Fluent()
    .WithProbeForEnv(15)
    .Load();

var connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        $"Connection string environment variable not defined 'POSTGRESQL_CONNECTION_STRING'.");

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddCors()
    .AddNpgsqlDbContext<ApplicationDbContext>(
        connectionString,
        new() { NpgsqlOptionsAction = o => o.UseNetTopologySuite() })
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

app.SetBaseUri();

await app.WaitForShutdownAsync();