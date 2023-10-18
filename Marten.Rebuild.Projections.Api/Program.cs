using Marten;
using Marten.Events.Projections;
using Marten.Rebuild.Projections.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMarten(options =>
{
    var connectionString = "Host=localhost;Port=5432;Database=MartenRebuildProjections;Username=postgres;password=postgres;Command Timeout=3";
    options.Connection(connectionString);

    options.Projections.Add<TodoItemProjection>(ProjectionLifecycle.Inline);
    options.Projections.Add<TodoItemsCollectionViewProjection>(ProjectionLifecycle.Inline);
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
