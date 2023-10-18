using Marten.Events.Aggregation;

namespace Marten.Rebuild.Projections.Api;

public class TodoItemProjection : SingleStreamProjection<TodoItem>
{
    public void Apply(TodoItemAdded @event, TodoItem document)
    {
        document.Id = @event.Id;
        document.CollectionId = @event.CollectionId;
        document.Description = @event.Description;
    }

    public void Apply(TodoItemDescriptionChanged @event, TodoItem document)
    {
        document.Description = @event.Description;
    }
}
