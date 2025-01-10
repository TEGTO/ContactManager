using ContactManager.Api;
using ContactManager.Api.Data;
using ContactManager.Api.Data.Repositories;
using ContactManager.Api.Services;
using DatabaseControl;
using ExceptionHandling;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

#region DB

builder.Services.AddDbContextFactory<ContactDbContext>(builder.Configuration.GetConnectionString(ConfigurationKeys.DATABASE_CONNECTION_STRING)!, "ContactManager.Api");
builder.Services.AddRepository<ContactDbContext>();

#endregion

#region Cors

var allowedOriginsString = builder.Configuration[ConfigurationKeys.ALLOWED_CORS_ORIGINS] ?? string.Empty;
var allowedOrigins = allowedOriginsString.Split(",", StringSplitOptions.RemoveEmptyEntries);

var corsPolicy = "AllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod();
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
        }
    });
});

#endregion

#region Fluent Validator

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

ValidatorOptions.Global.LanguageManager.Enabled = false;

#endregion

#region Project Services

builder.Services.AddSingleton<IContactRepository, ContactRepository>();
builder.Services.AddSingleton<IReadFromFileService, ReadFromFileService>();

#endregion

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.ConfigureCustomInvalidModelStateResponseControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

var app = builder.Build();

app.UseCors(corsPolicy);

if (app.Configuration[ConfigurationKeys.EF_CREATE_DATABASE] == "true")
{
    await app.ConfigureDatabaseAsync<ContactDbContext>(CancellationToken.None);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
