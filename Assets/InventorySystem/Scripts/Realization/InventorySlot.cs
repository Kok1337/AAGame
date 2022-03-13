using System;

public class InventorySlot : IInventorySlot
{
	public IInventoryItem item { get; private set; }

	public bool isFull => isEmpty ? false : amount == capacity;

	public bool isEmpty => item == null;

	public Type itemType => item.type;

	public int amount => isEmpty ? 0 : item.state.amount;

	public int capacity => isEmpty ? 0 : item.metadata.maxAmountInStack;


	public void Clear()
	{
		if (isEmpty)
			return;

		item.state.amount = 0;
		item = null;
	}

	public void SetItem(IInventoryItem item)
	{
		this.item = item;
	}
}
