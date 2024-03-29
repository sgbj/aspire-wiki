using Azure.AI.OpenAI.Assistants;
using Azure.Search.Documents.Indexes;
using Azure.Storage.Blobs;
using Confluent.Kafka;
using Microsoft.Azure.Cosmos;
using Wiki.Api;
using Wiki.Api.Endpoints;
using Wiki.Api.Models;
using Wiki.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedisDistributedCache("cache");

builder.AddAzureCosmosDB("db");

builder.AddKafkaProducer<string, string>("messaging");
builder.AddKafkaConsumer<string, string>("messaging");

builder.AddAzureSearch("search");

builder.AddAzureBlobService("blobs");

builder.Services.AddSingleton(new AssistantsClient(builder.Configuration["Chat:OpenAiKey"]));

builder.AddServiceDefaults();

builder.Services.AddSingleton<PageService>();
builder.Services.AddSingleton<SearchService>();
builder.Services.AddSingleton<BlobService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddOptions<ChatOptions>().BindConfiguration("Chat");

builder.Services.AddHostedService<ConsumerWorker>();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Create database
    var cosmosClient = app.Services.GetRequiredService<CosmosClient>();
    await cosmosClient.CreateDatabaseIfNotExistsAsync("wiki");
    var database = cosmosClient.GetDatabase("wiki");
    await database.CreateContainerIfNotExistsAsync("pages", "/id");

    // Create blob container
    var blobServiceClient = app.Services.GetRequiredService<BlobServiceClient>();
    var blobContainerClient = blobServiceClient.GetBlobContainerClient("pages");
    await blobContainerClient.CreateIfNotExistsAsync();

    // Create search index
    var searchIndexClient = app.Services.GetRequiredService<SearchIndexClient>();
    await searchIndexClient.CreateOrUpdateIndexAsync(new("pages", new FieldBuilder().Build(typeof(Page))));

    // Create Kafka topics
    await Task.Delay(TimeSpan.FromSeconds(10));
    var kafkaProducer = app.Services.GetRequiredService<IProducer<string, string>>();
    await kafkaProducer.ProduceAsync(ConsumerWorker.PageUpdatedTopic, new());
    await kafkaProducer.ProduceAsync(ConsumerWorker.PageDeletedTopic, new());
}

app.UseHttpsRedirection();

app.MapPageEndpoints();
app.MapBlobEndpoints();
app.MapChatEndpoints();

app.MapDefaultEndpoints();

app.Run();
