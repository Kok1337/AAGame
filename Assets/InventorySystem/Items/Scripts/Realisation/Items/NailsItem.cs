public class NailsItem : PickupableItem
{
	public override IInventoryItem ToIInventoryItem()
	{
		return new Nails(ItemInfo, Amount);
	}

	public class Nails : BasicInventoryItem
	{
		public Nails(IInventoryItemInfo itemInfo) : base(itemInfo) { }

		public Nails(IInventoryItemInfo itemInfo, int amount) : base(itemInfo, amount) { }

		public override IInventoryItem Clone()
		{
			var clonedItem = new Nails(metadata);
			clonedItem.state.amount = state.amount;
			return clonedItem;
		}
	}
}
