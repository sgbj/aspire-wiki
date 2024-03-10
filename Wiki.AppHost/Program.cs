var builder = DistributedApplication.CreateBuilder(args);

builder.AddContainer("search", "elasticsearch", "8.12.2");

var sql = builder.AddSqlServer("sql")
    .AddDatabase("db");

var cache = builder.AddRedis("cache");

var messaging = builder.AddKafka("messaging");

var api = builder.AddProject<Projects.Wiki_Api>("api")
    .WithReference(sql)
    .WithReference(cache)
    .WithReference(messaging);

builder.AddNpmApp("web", "../Wiki.Web")
    .WithReference(api)
    .WithEndpoint(containerPort: 4200, scheme: "http", env: "PORT")
    .AsDockerfileInManifest();

builder.Build().Run();
