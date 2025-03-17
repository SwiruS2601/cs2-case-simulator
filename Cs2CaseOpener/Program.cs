using Cs2CaseOpener.Extensions;

var builder = WebApplication.CreateBuilder();

builder.ConfigureSerilog();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCorsPolicy();

var app = builder.Build();
app.ConfigureMiddleware();

await app.InitializeDatabaseAsync();
await app.InitializeCountersAsync();

app.Run();
