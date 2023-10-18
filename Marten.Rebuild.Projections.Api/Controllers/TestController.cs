using Microsoft.AspNetCore.Mvc;

namespace Marten.Rebuild.Projections.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("recreate-all-projections", Name = nameof(RecreateAllProjections))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> RecreateAllProjections([FromServices] IDocumentStore store, CancellationToken cancellationToken)
    {
        // Resolve the IProjectionDaemon from the store
        using var daemon = await store.BuildProjectionDaemonAsync();

        await daemon.StartAllShards();

        // Stop the projection daemon
        await daemon.StopAll();

        // Start the projection daemon to rebuild the projections
        await daemon.StartDaemon();

        await daemon.RebuildProjection("TodoItem", TimeSpan.FromSeconds(5), cancellationToken);

        await store.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(TodoItemsCollectionView), cancellationToken);
        await daemon.RebuildProjection("Marten.Rebuild.Projections.Api.TodoItemsCollectionViewProjection", TimeSpan.FromSeconds(5), cancellationToken);

        await daemon.StopAll();

        return Ok($"All Ok! {DateTime.Now}");
    }

    [HttpGet("add-test-data")]
    public async Task<IActionResult> Test([FromServices] IDocumentStore store, CancellationToken cancellationToken)
    {
        var collection1Id = Guid.NewGuid();
        var collection2Id = Guid.NewGuid();

        var event1 = new TodoItemAdded(Guid.NewGuid(), collection1Id, "item1");
        var event2 = new TodoItemAdded(Guid.NewGuid(), collection1Id, "item2");
        var event3 = new TodoItemAdded(Guid.NewGuid(), collection2Id, "item3");

        var event4 = new TodoItemDescriptionChanged(event1.Id, collection1Id, "item1 updated");
        var event5 = new TodoItemDescriptionChanged(event3.Id, collection2Id, "item3 updated");

        await using var session = store.LightweightSession(); ;

        session.Events.Append(event1.Id, event1);
        await session.SaveChangesAsync(cancellationToken);

        session.Events.Append(event2.Id, event2);
        await session.SaveChangesAsync(cancellationToken);

        session.Events.Append(event3.Id, event3);
        await session.SaveChangesAsync(cancellationToken);

        session.Events.Append(event4.Id, event4);
        await session.SaveChangesAsync(cancellationToken);

        session.Events.Append(event5.Id, event5);
        await session.SaveChangesAsync(cancellationToken);

        return Ok($"All Ok! {DateTime.Now}");
    }

    [HttpGet("delete-database-data")]
    public async Task<IActionResult> DeleteDatabaseData([FromServices] IDocumentStore store, CancellationToken cancellationToken)
    {
        await store.Advanced.Clean.CompletelyRemoveAllAsync(cancellationToken);

        return Ok($"All Ok! {DateTime.Now}");
    }
}
