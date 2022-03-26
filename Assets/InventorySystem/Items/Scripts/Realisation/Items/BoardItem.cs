public class BoardItem : PickupableItem
{
	public override IInventoryItem ToIInventoryItem()
	{
		return new Board(ItemInfo, Amount);
	}

	public class Board : BasicInventoryItem
	{
		public Board(IInventoryItemInfo itemInfo) : base(itemInfo) { }

		public Board(IInventoryItemInfo itemInfo, int amount) : base(itemInfo, amount) { }

		public override IInventoryItem Clone()
		{
			var clonedItem = new Board(metadata);
			clonedItem.state.amount = state.amount;
			return clonedItem;
		}
	}
}
