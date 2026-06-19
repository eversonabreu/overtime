using Evertech.Overtime.API;
using Evertech.Overtime.Domain.DI;
using Evertech.Overtime.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

ConfigurationSettings.ResolveEnvironmentName(builder.Environment, args);
ConfigurationSettings.ResolveSecrets(builder.Configuration, builder.Environment.EnvironmentName);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDomain();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();