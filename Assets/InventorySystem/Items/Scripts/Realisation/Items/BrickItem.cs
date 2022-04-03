public class BrickItem : PickupableItem
{
	public override IInventoryItem ToIInventoryItem()
	{
		return new Brick(ItemInfo, Amount);
	}

	public class Brick : BasicInventoryItem
	{
		public Brick(IInventoryItemInfo itemInfo) : base(itemInfo) { }

		public Brick(IInventoryItemInfo itemInfo, int amount) : base(itemInfo, amount) { }

		public override IInventoryItem Clone()
		{
			var clonedItem = new Brick(metadata);
			clonedItem.state.amount = state.amount;
			return clonedItem;
		}
	}
}