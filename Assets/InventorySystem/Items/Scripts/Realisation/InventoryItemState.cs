using System;

[Serializable]
public class InventoryItemState : IInventoryItemState
{
	public int amount { get; set; }

	public bool isEmpty => amount == 0;

	public InventoryItemState(int itemAmount = 0)
	{
		amount = itemAmount;
	}
}
