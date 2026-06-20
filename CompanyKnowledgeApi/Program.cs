using CompanyKnowledgeApi.Common.Extensions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Features.Documents.UploadDocument;
using CompanyKnowledgeApi.Infrastructure.Storage;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.Configure<DocumentStorageOptions>(builder.Configuration.GetSection("DocumentStorage"));
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<IValidator<Command>, Validator>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.UseVector());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.MapFeatureEndpoints();

app.Run();
