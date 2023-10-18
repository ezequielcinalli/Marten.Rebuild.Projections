namespace Marten.Rebuild.Projections.Api;

public class TodoItemsCollectionView
{
    public Guid Id { get; set; }
    public Dictionary<Guid, string> TodoItemsDescriptions { get; set; } = new();
}
