using System;

public interface IInventory
{
	int inventoryCapacity { get; set; }
	bool isFull { get; }

	IInventoryItem GetItem(Type itemType);
	IInventoryItem[] GetAllItems();
	IInventoryItem[] GetAllItems(Type itemType);
	int GetItemAmount(Type itemType);
	bool TryToAddItem(object sender, IInventoryItem item);
	void Remove(object sender, Type itemType, int amount = 1);
	bool HasItem(Type itemType, out IInventoryItem item);
}
