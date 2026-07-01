using CompanyKnowledgeApi.Common.Extensions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Features.Chat.AskQuestion;
using CompanyKnowledgeApi.Features.Documents.UploadDocument;
using CompanyKnowledgeApi.Features.Search.SemanticSearch;
using CompanyKnowledgeApi.Infrastructure.Ai.Chat;
using CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;
using CompanyKnowledgeApi.Infrastructure.BackgroundJobs;
using CompanyKnowledgeApi.Infrastructure.Documents.Chunking;
using CompanyKnowledgeApi.Infrastructure.Documents.Cleaning;
using CompanyKnowledgeApi.Infrastructure.Documents.Extraction;
using CompanyKnowledgeApi.Infrastructure.Storage;
using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "Frontend";

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.Configure<DocumentStorageOptions>(builder.Configuration.GetSection("DocumentStorage"));
builder.Services.Configure<DocumentChunkingOptions>(builder.Configuration.GetSection("DocumentChunking"));
builder.Services.Configure<EmbeddingOptions>(builder.Configuration.GetSection("AI"));
builder.Services.Configure<ChatOptions>(builder.Configuration.GetSection("AI"));
builder.Services.Configure<BackgroundJobOptions>(builder.Configuration.GetSection("BackgroundJobs"));
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<ITextExtractor, PdfPigTextExtractor>();
builder.Services.AddScoped<ITextExtractor, OpenXmlDocxTextExtractor>();
builder.Services.AddScoped<ITextCleaner, TextCleaner>();
builder.Services.AddScoped<ITextChunker, TextChunker>();
builder.Services.AddScoped<IValidator<UploadDocumentModel>, UploadDocumentValidator>();
builder.Services.AddScoped<IValidator<SemanticSearchModel>, SemanticSearchValidator>();
builder.Services.AddScoped<IValidator<AskQuestionModel>, AskQuestionValidator>();
builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>((serviceProvider, httpClient) =>
{
    var options = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<EmbeddingOptions>>()
        .Value;

    httpClient.BaseAddress = new Uri(options.OllamaBaseUrl);
});
builder.Services.AddHttpClient<IChatCompletionService, OllamaChatCompletionService>((serviceProvider, httpClient) =>
{
    var options = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<ChatOptions>>()
        .Value;

    httpClient.BaseAddress = new Uri(options.OllamaBaseUrl);
    httpClient.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);
});
builder.Services.AddScopedServicesFrom(typeof(Program).Assembly);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.UseVector());
});
builder.Services.AddHangfire((serviceProvider, configuration) =>
{
    var connectionString = serviceProvider
        .GetRequiredService<IConfiguration>()
        .GetConnectionString("DefaultConnection");

    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(connectionString);
});
builder.Services.AddHangfireServer((serviceProvider, options) =>
{
    var backgroundJobOptions = serviceProvider
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<BackgroundJobOptions>>()
        .Value;

    options.WorkerCount = Math.Max(1, backgroundJobOptions.WorkerCount);
    options.Queues = ["default"];
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var backgroundJobOptions = app.Services
    .GetRequiredService<Microsoft.Extensions.Options.IOptions<BackgroundJobOptions>>()
    .Value;

if (backgroundJobOptions.EnableDashboard && !app.Environment.IsProduction())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = Array.Empty<IDashboardAuthorizationFilter>()
    });
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);

app.MapHealthChecks("/health");
app.MapFeatureEndpoints();

app.Run();
