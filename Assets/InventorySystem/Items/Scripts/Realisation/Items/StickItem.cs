public class StickItem : PickupableItem
{
	public override IInventoryItem ToIInventoryItem()
	{
		return new Stick(ItemInfo, Amount);
	}

	public class Stick : BasicInventoryItem
	{
		public Stick(IInventoryItemInfo itemInfo) : base(itemInfo) { }

		public Stick(IInventoryItemInfo itemInfo, int amount) : base(itemInfo, amount) { }

		public override IInventoryItem Clone()
		{
			var clonedItem = new Stick(metadata);
			clonedItem.state.amount = state.amount;
			return clonedItem;
		}
	}
}