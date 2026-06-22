using CompanyKnowledgeApi.Common.Extensions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Features.Documents.UploadDocument;
using CompanyKnowledgeApi.Features.Search.SemanticSearch;
using CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;
using CompanyKnowledgeApi.Infrastructure.Documents.Chunking;
using CompanyKnowledgeApi.Infrastructure.Documents.Cleaning;
using CompanyKnowledgeApi.Infrastructure.Documents.Extraction;
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
builder.Services.Configure<DocumentChunkingOptions>(builder.Configuration.GetSection("DocumentChunking"));
builder.Services.Configure<EmbeddingOptions>(builder.Configuration.GetSection("AI"));
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<ITextExtractor, PdfPigTextExtractor>();
builder.Services.AddScoped<ITextExtractor, OpenXmlDocxTextExtractor>();
builder.Services.AddScoped<ITextCleaner, TextCleaner>();
builder.Services.AddScoped<ITextChunker, TextChunker>();
builder.Services.AddScoped<IValidator<UploadDocumentModel>, UploadDocumentValidator>();
builder.Services.AddScoped<IValidator<SemanticSearchModel>, SemanticSearchValidator>();
builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>((serviceProvider, httpClient) =>
{
    var options = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<EmbeddingOptions>>()
        .Value;

    httpClient.BaseAddress = new Uri(options.OllamaBaseUrl);
});
builder.Services.AddScopedServicesFrom(typeof(Program).Assembly);
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
