using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItemInfo
{
	int maxAmountInStack { get; }
	Sprite spriteIcon { get; }
}
