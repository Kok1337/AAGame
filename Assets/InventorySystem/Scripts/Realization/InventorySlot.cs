using System;

public class InventorySlot : IInventorySlot
{
	private IInventoryItem _item;


	public IInventoryItem item => _item;

	public bool isFull => isEmpty ? false : amount == capacity;

	public bool isEmpty => item == null;

	public Type itemType => item.type;

	public int amount => isEmpty ? 0 : item.state.amount;

	public int capacity => isEmpty ? 0 : item.metadata.maxAmountInStack;


	public void Clear()
	{
		if (isEmpty)
			return;

		_item.state.amount = 0;
		_item = null;
	}

	public void SetItem(IInventoryItem item)
	{
		if (!isEmpty)
			return;

		this._item = item;
	}
}
