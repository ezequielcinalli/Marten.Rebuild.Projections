using Marten;
using Marten.Events.Projections;
using Marten.Rebuild.Projections.Api;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMarten(options =>
{
    var connectionString =
        "Host=localhost;Port=5432;Database=MartenRebuildProjections;Username=postgres;password=postgres;Command Timeout=3";
    options.Connection(connectionString);

    options.Projections.Add<TodoItemProjection>(ProjectionLifecycle.Inline);
    options.Projections.Add<TodoItemsCollectionViewProjection>(
        ProjectionLifecycle.Inline,
        asyncOptions => asyncOptions.EnableDocumentTrackingByIdentity = true);

    options.AutoCreateSchemaObjects = AutoCreate.All;
    //Maybe incorrect use of tenants but it works for auto create the postgres database
    options.CreateDatabasesForTenants(c =>
    {
        c.ForTenant("MartenRebuildProjections");
    });
}).ApplyAllDatabaseChangesOnStartup();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
