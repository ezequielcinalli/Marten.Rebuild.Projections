using Marten.Events.Projections;

namespace Marten.Rebuild.Projections.Api;

public class TodoItemsCollectionViewProjection : EventProjection
{
    public TodoItemsCollectionViewProjection()
    {
        ProjectAsync<TodoItemAdded>(async (@event, ops) =>
        {
            var document = await ops.LoadAsync<TodoItemsCollectionView>(@event.CollectionId);
            if (document is null)
            {
                document = new TodoItemsCollectionView { Id = @event.CollectionId };
            }

            if (document.TodoItemsDescriptions.TryAdd(@event.Id, @event.Description))
            {
                ops.Store(document);
            }
            else
            {
                Console.WriteLine("Here expected result is to not exists key in dictionary due to the truncate of the view table");
            }
        });

        ProjectAsync<TodoItemDescriptionChanged>(async (@event, ops) =>
        {
            var document = await ops.LoadAsync<TodoItemsCollectionView>(@event.CollectionId);
            if (document is null)
            {
                throw new InvalidOperationException($"TodoItemsCollectionView with id {@event.CollectionId} not found");
            }

            document.TodoItemsDescriptions[@event.Id] = @event.Description;
        });
    }
}
