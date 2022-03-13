using System;

[Serializable]
public class InventoryItemState : IInventoryItemState
{
	private int _itemAmount;


	public int amount { get => _itemAmount; set => _itemAmount = value; }


	public InventoryItemState(int itemAmount = 0)
	{
		this._itemAmount = itemAmount;
	}
}
