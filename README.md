# Marten.Rebuild.Projections.Api

This is a demo project to reproduce the problematic behavior of the Marten's `RebuildProjection` method for the projections with the marker interface `EventProjection`. For projections with the marker interface `SingleStreamProjection` works like a charm.

## Setup and steps to reproduce

- Have a postgres running in localhost port 5432 (for more details connection string in Program.cs)
- Run the application
- Hit the endpoint "add-test-data" to load 5 mock events
- In the `mt_doc_todoitemscollectionview` table change the column data, for example the descriptions of the items dictionary from "item1"/"item2"/"item3" to "itemX invalid"
- Hit the endpoint "rebuild-projections" to rebuild the projections

## Expected behavior

The projection should be rebuilt and the `mt_doc_todoitemscollectionview` table should be updated with the original data, but due to missing "truncate" command to clean the table it is not happen.

If i manually truncate the table with the utility `await store.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(TodoItemsCollectionView), cancellationToken);` then when the projection is rebuilt and query data, the view is not found. For example in this Apply method the document is always null:

```csharp
ProjectAsync<TodoItemDescriptionChanged>(async (@event, ops) =>
{
	var document = await ops.LoadAsync<TodoItemsCollectionView>(@event.CollectionId);
	if (document is null)
	{
		throw new InvalidOperationException($"TodoItemsCollectionView with id {@event.CollectionId} not found");
	}

	document.TodoItemsDescriptions[@event.Id] = @event.Description;
});
```
