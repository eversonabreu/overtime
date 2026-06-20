using Evertech.Overtime.Application.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Environment.EnvironmentName = ConfigurationSettings.GetEnvironmentName(args);
ConfigurationSettings.ResolveSecrets(builder.Configuration, builder.Environment.EnvironmentName);

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();