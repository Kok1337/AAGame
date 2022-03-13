using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
	[SerializeField] private InventoryItemInfo _nailsInfo;
	[SerializeField] private InventoryItemInfo _boardInfo;

	private UIInventorySlot[] _uiSlots;

	public InventoryWithSlots inventory { get; private set; }

	private void Start()
	{
		InventoryManager.OnInventoryStateChanged.AddListener((sender) => { Refresh(); });

		_uiSlots = GetComponentsInChildren<UIInventorySlot>();
		inventory = new InventoryWithSlots(_uiSlots.Length);
		FillSlots();
		SetupInventoryUI(inventory);
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
		var rSlotIndex = UnityEngine.Random.Range(0, slots.Count);
		var rSlot = slots[rSlotIndex];
		var rCount = UnityEngine.Random.Range(1, 4);
		newItem.state.amount = rCount;
		inventory.TryToAddToSlot(this, rSlot, newItem);
		return rSlot;
	}

	public void Refresh()
	{
		foreach (var uiSlot in _uiSlots)
			uiSlot.Refresh();
	}

	private void SetupInventoryUI(InventoryWithSlots inventory)
	{
		var allSlots = inventory.GetAllSlots();
		var allSlotsCount = allSlots.Length;

		for (int i = 0; i < allSlotsCount; i++)
		{
			var slot = allSlots[i];
			var uiSlot = _uiSlots[i];
			uiSlot.SetSlot(slot);
		}
	}
}
