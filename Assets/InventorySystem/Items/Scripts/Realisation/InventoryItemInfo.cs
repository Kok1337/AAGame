using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemInfo", menuName = "Gameplay/Items/Create New ItemInfo")]
public class InventoryItemInfo : ScriptableObject, IInventoryItemInfo
{
	[SerializeField]
	private int _maxAmountInStack;

	[SerializeField]
	private Sprite _spriteIcon;


	public int maxAmountInStack => _maxAmountInStack;

	public Sprite spriteIcon => _spriteIcon;


	public InventoryItemInfo(int maxAmountInStack, Sprite spriteIcon)
	{
		this._maxAmountInStack = maxAmountInStack;
		this._spriteIcon = spriteIcon;
	}
}
