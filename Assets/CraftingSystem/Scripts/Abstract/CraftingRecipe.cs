using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
	public PickupableItem Item;
	[Min(1)]
	public int Amount;
}

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Gameplay/Items/Create New Recipe")]
public class CraftingRecipe : ScriptableObject
{
	public List<ItemAmount> Materials;
	public List<ItemAmount> Results;

	public bool CanCraft(IInventory inventory)
	{
		foreach (var material in Materials)
		{
			if (inventory.GetItemAmount(material.Item.ToIInventoryItem().type) < material.Amount)
			{
				return false;
			}
		}
		return true;
	}

	public void Craft(IInventory inventory)
	{
		if (CanCraft(inventory))
		{
			foreach (var material in Materials)
			{
				inventory.Remove(this, material.Item.ToIInventoryItem().type, material.Amount);
			}

			foreach (var result in Results)
			{
				IInventoryItem createdItem = result.Item.ToIInventoryItem();
				createdItem.state.amount = result.Amount;
				inventory.TryToAddItem(this, createdItem);
			}
		}
	}
}