using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventorySlot 
{
	IInventoryItem item { get; }

	bool isFull { get; }
	bool isEmpty { get; }
	Type itemType { get; }
	int amount { get; }
	int capacity { get; }

	void SetItem(IInventoryItem item);
	void Clear();
}
