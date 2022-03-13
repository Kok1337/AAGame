using System;

public class NailsItem : BasicInventoryItem
{
	public NailsItem() : base() { }

	public NailsItem(IInventoryItemInfo itemInfo) : base(itemInfo) {}


	public override IInventoryItem Clone()
	{
		var clonedItem = new NailsItem(metadata);
		clonedItem.state.amount = state.amount;
		return clonedItem;
	}
}
