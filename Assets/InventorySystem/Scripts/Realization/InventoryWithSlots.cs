using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryWithSlots : IInventory
{
	private int _inventoryCapacity;
	private List<IInventorySlot> _slots;


	public event Action<object, IInventoryItem, int> OnInventoryItemAddedEvent;
	public event Action<object, Type, int> OnInventoryItemRemovedEvent;
	public event Action<object> OnInventoryUpdatedEvent;


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
				Debug.Log($"Item removed. ItemType={itemType}, amount={amountToRemove}");
				OnInventoryItemRemovedEvent?.Invoke(sender, itemType, amountToRemove);

				break;
			}

			// Сколько всего мы можем удалить из текущего слота (все)
			var amountRemoved = slot.amount;
			amountToRemove -= slot.amount;
			slot.Clear();

			Debug.Log($"Item removed. ItemType={itemType}, amount={amountRemoved}");
			OnInventoryItemRemovedEvent?.Invoke(sender, itemType, amountRemoved);
		}
	}

	public bool TryToAddItem(object sender, IInventoryItem item)
	{
		var slotsWithSameItemButNotEmpty = _slots.Find(slot => !slot.isEmpty && slot.itemType == item.type && !slot.isFull);

		if (slotsWithSameItemButNotEmpty != null)
			return TryToAddToSlot(sender, slotsWithSameItemButNotEmpty, item);

		var emprySlot = _slots.Find(slot => slot.isEmpty);

		if (emprySlot != null)
			return TryToAddToSlot(sender, emprySlot, item);

		return false;
	}


	public bool TryToAddToSlot(object sender, IInventorySlot slot, IInventoryItem item)
	{
		// Проверка, влезет ли в данный слот если слот не пустой
		var fits = slot.isEmpty ? item.state.amount < item.metadata.maxAmountInStack : slot.amount + item.state.amount <= slot.capacity;
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

		Debug.Log($"Item added. ItemType={item.type}, amount={amountToAdd}, fits={fits}, amountLeft={amountLeft}");
		OnInventoryItemAddedEvent?.Invoke(this, item, amountToAdd);

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
}