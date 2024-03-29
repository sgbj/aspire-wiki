var builder = DistributedApplication.CreateBuilder(args);

var cosmosdb = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureCosmosDB("db")
    : builder.AddConnectionString("db");

var cache = builder.AddRedis("cache");

var messaging = builder.AddKafka("messaging");

var search = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSearch("search")
    : builder.AddConnectionString("search");

var blobs = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureStorage("storage").AddBlobs("blobs")
    : builder.AddConnectionString("blobs");

var api = builder.AddProject<Projects.Wiki_Api>("api")
    .WithReference(cosmosdb)
    .WithReference(cache)
    .WithReference(messaging)
    .WithReference(search)
    .WithReference(blobs);

builder.AddNpmApp("web", "../Wiki.Web")
    .WithReference(api)
    .WithEndpoint(containerPort: 4200, scheme: "http", env: "PORT")
    .PublishAsDockerFile();

builder.Build().Run();
