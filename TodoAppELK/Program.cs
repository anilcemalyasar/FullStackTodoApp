using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using TodoAppELK.Data;
using TodoAppELK.Models.Domain;
using TodoAppELK.Services.Abstract;
using TodoAppELK.Services.Concrete;

var MyAllowSpecificOrigins = "AllowReactApp";

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
// Add services to the container.

// ELASTIC APM ENTEGRASYONU
// builder.Services'in en üstlerine eklemek iyi bir pratiktir.
builder.Services.AddAllElasticApm();

builder.Services.AddScoped<IOpenAIService, OpenAIService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<TodoDbContext>(options =>
//    options.UseInMemoryDatabase("TodoDb"));

builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

configureLogging();
builder.Host.UseSerilog();
var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<TodoDbContext>();
        await dbContext.Database.MigrateAsync(); // Ensure database is created and migrations are applied

    }
    catch (Exception ex)
    {
        // Migration sýrasýnda bir hata olursa, logla ve uygulamayý durdur.
        Log.Fatal(ex, "An error occurred while applying database migrations.");
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

//app.Logger.LogInformation("Adding routes...");
Log.Information("Starting Todo Application...");

// API Endpoints
app.MapGet("/api/todos", async (TodoDbContext db) =>
{
    return await db.Todos.ToListAsync();
});

app.MapGet("/api/todos/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null)
    {
        Log.Error("Todo with ID {Id} not found", id);
        return Results.NotFound();
    }
    else
    {
        Log.Information("Retrieved Todo with ID {Id}", id);
        return Results.Ok(todo);
    }
});

app.MapPost("/api/todos", async (Todo todo, TodoDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

app.MapPut("/api/todos/{id}", async (int id, Todo updatedTodo, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = updatedTodo.Title;
    todo.Description = updatedTodo.Description;
    todo.IsCompleted = updatedTodo.IsCompleted;

    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.MapDelete("/api/todos/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/api/ai/analyze-todos", async (TodoDbContext db, IOpenAIService aiService) =>
{
    var todos = await db.Todos.ToListAsync();
    var analysis = await aiService.AnalyzeTodoListAsync(todos);
    return Results.Ok(new { analysis });
}); ;

app.MapGet("/api/ai/suggest-todo", async (string userInput, IOpenAIService aiService) =>
{
    var suggestion = await aiService.GenerateTodoSuggestAsync(userInput);
    return Results.Ok(new { suggestion });
});

app.MapGet("/api/ai/motivation", async (TodoDbContext db, IOpenAIService aiService) =>
{
    var todos = await db.Todos.ToListAsync();
    var completedCount = todos.Count(t => t.IsCompleted);
    var totalCount = todos.Count;

    var message = await aiService.GenerateMotivationalMessageAsync(completedCount, totalCount);
    return Results.Ok(new
    {
        message,
        completedCount,
        totalCount,
        completionRate = totalCount > 0 ? (double) completedCount / totalCount * 100 : 0
    });
});

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();

void configureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true).Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
        .Enrich.WithProperty("Environment", environment)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
        
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["Elasticsearch:Uri"] ?? "http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
        IndexFormat = $"{configuration["ApplicationName"]}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        NumberOfReplicas = 1,
        NumberOfShards = 2
    };
}
