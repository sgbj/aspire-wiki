using Wiki.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlServerDbContext<WikiDbContext>("db");

builder.AddKafkaProducer<string, string>("messaging");
builder.AddKafkaConsumer<string, string>("messaging");

builder.AddServiceDefaults();

builder.Services.AddHostedService<ConsumerWorker>();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<WikiDbContext>();
    db.Database.EnsureCreated();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPageEndpoints();

app.MapDefaultEndpoints();

app.Run();
