namespace Marten.Rebuild.Projections.Api;

public class TodoItem
{
    public Guid Id { get; set; }
    public Guid CollectionId { get; set; }
    public string Description { get; set; } = "";
}

public record TodoItemAdded(Guid Id, Guid CollectionId, string Description);

public record TodoItemDescriptionChanged(Guid Id, Guid CollectionId, string Description);
