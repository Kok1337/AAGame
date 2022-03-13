using System;

public interface IInventoryItem
{
	IInventoryItemInfo metadata { get; }
	IInventoryItemState state { get; }

	Type type { get; }

	IInventoryItem Clone();
}
