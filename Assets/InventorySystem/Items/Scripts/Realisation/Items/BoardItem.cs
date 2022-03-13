public class BoardItem : BasicInventoryItem
{
	public BoardItem(IInventoryItemInfo itemInfo) : base(itemInfo) { }

	public override IInventoryItem Clone()
	{
		var clonedItem = new BoardItem(metadata);
		clonedItem.state.amount = state.amount;
		return clonedItem;
	}
}
