using System;
using UnityEngine;

public abstract class BasicInventoryItem : IInventoryItem
{
	[SerializeField]
	private IInventoryItemInfo _itemInfo;
	private IInventoryItemState _itemState;


	public BasicInventoryItem()
	{
		this._itemState = new InventoryItemState();
	}

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
