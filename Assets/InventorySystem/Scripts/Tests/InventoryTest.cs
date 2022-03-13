using System.Linq;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
	private InventoryWithSlots _inventory;

    private void Awake()
    {
		var inventoryCapacity = 10;
		_inventory = new InventoryWithSlots(inventoryCapacity);
		var item = new NailsItem(new InventoryItemInfo(5, null));
		Debug.Log(item.type);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
			AddRandomApple();

		if (Input.GetKeyDown(KeyCode.R))
			RemoveRandomApple();

		if (Input.GetKeyDown(KeyCode.P))
		{
			var slots = _inventory.GetAllSlots();
			string res = "";
			foreach (var s in slots)
				res += s.amount + " ";

			Debug.Log(res);
		}

		if (Input.GetKeyDown(KeyCode.C))
			Debug.Log(_inventory.GetItemAmount(typeof(NailsItem)));
	}

	private void RemoveRandomApple()
	{
		var rCount = Random.Range(1, 10);
		Debug.Log($"Remove. ItemType={typeof(NailsItem)}, amount={rCount}");
		_inventory.Remove(this, typeof(NailsItem), rCount);
	}

	private void AddRandomApple()
	{
		var rCount = Random.Range(1, 5);
		var item = new NailsItem(new InventoryItemInfo(5, null));
		item.state.amount = rCount;
		Debug.Log($"Try add. ItemType={item.type}, amount={rCount}");
		_inventory.TryToAddItem(this, item);
	}
}
