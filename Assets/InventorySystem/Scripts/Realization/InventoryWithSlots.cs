using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryWithSlots : IInventory
{
	private int _inventoryCapacity;
	private List<IInventorySlot> _slots;


	public int inventoryCapacity
	{
		get => _inventoryCapacity;

		set => _inventoryCapacity = value;
	}

	public bool isFull => _slots.All(slot => slot.isFull);


	public InventoryWithSlots(int capacity)
	{
		this._inventoryCapacity = capacity;
		this._slots = new List<IInventorySlot>(capacity);
		for (int i = 0; i < capacity; i++)
			_slots.Add(new InventorySlot());
	}


	public IInventoryItem[] GetAllItems()
	{
		return (from slot in _slots where !slot.isEmpty select slot.item).ToArray();
	}

	public IInventoryItem[] GetAllItems(Type itemType)
	{
		return (from slot in _slots where !slot.isEmpty && slot.itemType == itemType select slot.item).ToArray();
	}

	public IInventoryItem GetItem(Type itemType)
	{
		// Находим IInventoryItem для получения метаднных
		return _slots.Find(slot => !slot.isEmpty && slot.itemType == itemType).item;
	}

	public int GetItemAmount(Type itemType)
	{
		return (from slot in _slots where !slot.isEmpty && slot.itemType == itemType select slot.amount).Sum();
	}

	public bool HasItem(Type itemType, out IInventoryItem item)
	{
		item = GetItem(itemType);
		return item != null;
	}

	public void Remove(object sender, Type itemType, int amount = 1)
	{
		var slotsWithItem = GetAllSlots(itemType);
		if (slotsWithItem.Length == 0)
			return;

		var amountToRemove = amount;
		var count = slotsWithItem.Length;

		for (int i = count - 1; i >= 0; i--)
		{
			var slot = slotsWithItem[i];
			// Если можем из данного слота все отнять - отнимаем
			if (slot.amount >= amountToRemove)
			{
				slot.item.state.amount -= amountToRemove;

				if (slot.amount <= 0)
					slot.Clear();

				// Сколько отняли (все)
				InventoryManager.SendInventoryItemRemoved(sender, itemType, amountToRemove);

				break;
			}

			// Сколько всего мы можем удалить из текущего слота (все)
			var amountRemoved = slot.amount;
			amountToRemove -= slot.amount;
			slot.Clear();

			InventoryManager.SendInventoryItemRemoved(sender, itemType, amountRemoved);
		}
	}

	public bool TryToAddItem(object sender, IInventoryItem item)
	{
		var slotsWithSameItemButNotEmpty = _slots.Find(slot => !slot.isEmpty && slot.itemType == item.type && !slot.isFull);

		if (slotsWithSameItemButNotEmpty != null)
			return TryToAddToSlot(sender, slotsWithSameItemButNotEmpty, item);

		var emptySlot = _slots.Find(slot => slot.isEmpty);

		if (emptySlot != null)
			return TryToAddToSlot(sender, emptySlot, item);

		return false;
	}


	public bool TryToAddToSlot(object sender, IInventorySlot slot, IInventoryItem item)
	{
		// Проверка, влезет ли в данный слот если слот не пустой
		var fits = slot.isEmpty ? item.state.amount <= item.metadata.maxAmountInStack : (slot.amount + item.state.amount) <= slot.capacity;
		// Сколько нужно добавить в слот
		// Если влазиет - добавить все иначе сколько влезет
		var amountToAdd = fits ? item.state.amount : slot.capacity - slot.amount;
		// Сколько осталось
		var amountLeft = item.state.amount - amountToAdd;

		if (slot.isEmpty)
		{
			// Клонируем объект и запихиваем в него то кол-во, которое добавляем в текущий слот
			var clonedItem = item.Clone();
			clonedItem.state.amount = amountToAdd;
			slot.SetItem(clonedItem);
		}
		else
			slot.item.state.amount += amountToAdd;
	
		InventoryManager.SendInventoryItemAdded(this, item, amountToAdd);

		if (amountLeft <= 0)
			return true;

		item.state.amount = amountLeft;
		return TryToAddItem(sender, item);
	}

	public IInventorySlot[] GetAllSlots(Type itemType)
	{
		return (from slot in _slots where !slot.isEmpty && slot.itemType == itemType select slot).ToArray();
	}

	public IInventorySlot[] GetAllSlots()
	{
		return _slots.ToArray();
	}

	private void SwapSlots(object sender, IInventorySlot slot1, IInventorySlot slot2)
	{
		Debug.Log($"SwapSlots. slot1={slot1.itemType}, slot2={slot2.itemType}");
		IInventoryItem tmp = slot1.item;
		slot1.SetItem(slot2.item);
		slot2.SetItem(tmp);
		Debug.Log($"SwapSlots. slot1={slot1.itemType}, slot2={slot2.itemType}");
		InventoryManager.SendInventoryStateChanged(sender);
	}

	public void TransitFromSlotToSlot(object sender, IInventorySlot fromSlot, IInventorySlot toSlot)
	{
		if (fromSlot.isEmpty)
			throw new Exception("fromSlot is empty");

		if (toSlot.isFull)
			return;

		if (!toSlot.isEmpty && fromSlot.itemType != toSlot.itemType)
		{
			SwapSlots(sender, fromSlot, toSlot);
			return;
		}
			
		var slotCapacity = fromSlot.capacity;
		var fits = fromSlot.amount + toSlot.amount <= slotCapacity;
		var amountToAdd = fits ? fromSlot.amount : slotCapacity - toSlot.amount;
		var amountLeft = fromSlot.amount - amountToAdd;

		if (toSlot.isEmpty)
		{
			toSlot.SetItem(fromSlot.item);
			fromSlot.Clear();
			InventoryManager.SendInventoryStateChanged(sender);
		}

		toSlot.item.state.amount += amountToAdd;

		if (fits)
			fromSlot.Clear();
		else
			fromSlot.item.state.amount = amountLeft;

		InventoryManager.SendInventoryStateChanged(sender);
	}
}