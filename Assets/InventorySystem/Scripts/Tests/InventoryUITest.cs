using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUITest
{
	private InventoryItemInfo _nailsInfo;
	private InventoryItemInfo _boardInfo;

	public InventoryWithSlots inventory { get; private set; }

	public InventoryUITest(int inventoryCapacity, InventoryItemInfo nailsInfo, InventoryItemInfo boardInfo)
	{
		_nailsInfo = nailsInfo;
		_boardInfo = boardInfo;

		Debug.Log("inventoryCapacity=" + inventoryCapacity);
		inventory = new InventoryWithSlots(inventoryCapacity);

		FillSlots();
	}

	private void FillSlots()
	{
		var allSlots = inventory.GetAllSlots();
		var availableSlots = new List<IInventorySlot>(allSlots);

		var filledSlots = 5;

		for (int i = 0; i < filledSlots; i++)
		{
			var addedSlot = AddRandomItemsIntoRandomSlot(availableSlots, new NailsItem(_nailsInfo));
			availableSlots.Remove(addedSlot);

			addedSlot = AddRandomItemsIntoRandomSlot(availableSlots, new BoardItem(_boardInfo));
			availableSlots.Remove(addedSlot);
		}
	}

	private IInventorySlot AddRandomItemsIntoRandomSlot(List<IInventorySlot> slots, IInventoryItem newItem)
	{
		var rSlotIndex = Random.Range(0, slots.Count);
		var rSlot = slots[rSlotIndex];
		var rCount = Random.Range(1, 4);
		newItem.state.amount = rCount;
		inventory.TryToAddToSlot(this, rSlot, newItem);
		return rSlot;
	}
}
