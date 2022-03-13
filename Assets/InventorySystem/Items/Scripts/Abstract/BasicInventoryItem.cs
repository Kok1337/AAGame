using System;
using UnityEngine;

public abstract class BasicInventoryItem : IInventoryItem
{
	private IInventoryItemInfo _itemInfo;
	private IInventoryItemState _itemState;

	public BasicInventoryItem(IInventoryItemInfo itemInfo)
	{
		this._itemState = new InventoryItemState();
		this._itemInfo = itemInfo;
	}


	public IInventoryItemInfo metadata => _itemInfo;

	public IInventoryItemState state => _itemState;

	public Type type => GetType();

	public abstract IInventoryItem Clone();
}
