using Evertech.Overtime.API.Endpoints;
using Evertech.Overtime.API.Extensions;
using Evertech.Overtime.API.Middlewares;
using Evertech.Overtime.Application.Configurations;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Environment.EnvironmentName = ConfigurationSettings.GetEnvironmentName(args);
ConfigurationSettings.ResolveSecrets(builder.Configuration, builder.Environment.EnvironmentName);

builder.Services.AddServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseExceptionMiddleware();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Evertech Overtime API";
        options.Theme = ScalarTheme.DeepSpace;
    });
}

app.MapGroup("/api")
    .MapGet("/health-check", () => Results.Ok($"On-air → {app.Environment.EnvironmentName}"));

app.MapGroup("/api")
    .AddAuthEndpoints()
    .AddPersonEndpoints()
    .AddGroupEndpoints()
    .AddJourneyEndpoints()
    .AddCompensatoryConversionEndpoints()
    .AddHolidayEndpoints()
    .AddLocationEndpoints();

app.UseHttpsRedirection();

app.Run();