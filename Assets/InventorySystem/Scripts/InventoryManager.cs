using System;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
	public static readonly UnityEvent<object, UIInventoryItem> OnUIItemSelected = new UnityEvent<object, UIInventoryItem>();
	public static readonly UnityEvent<object, Type, int> OnInventoryItemRemoved = new UnityEvent<object, Type, int>();
	public static readonly UnityEvent<object, IInventoryItem, int> OnInventoryItemAdded = new UnityEvent<object, IInventoryItem, int>();
	public static readonly UnityEvent<object> OnInventoryStateChanged = new UnityEvent<object>();

	public static UIInventoryItem selectedUiItem { get; private set; }

	private void Awake()
	{
		OnInventoryStateChanged.AddListener((sender) => { UpdateSelectedItem(); });
		OnInventoryItemAdded.AddListener((sender, item, added) => { SendInventoryStateChanged(sender); });
		OnInventoryItemRemoved.AddListener((sender, type, removed) => { SendInventoryStateChanged(sender); });
	}

	public static void SendUIItemSelected(object sender, UIInventoryItem uiItem)
	{
		selectedUiItem = uiItem;
		OnUIItemSelected.Invoke(sender, uiItem);
	}

	public static void SendInventoryItemRemoved(object sender, Type itemType, int removed)
	{
		OnInventoryItemRemoved.Invoke(sender, itemType, removed);
	}

	public static void SendInventoryItemAdded(object sender, IInventoryItem item, int addad)
	{
		OnInventoryItemAdded.Invoke(sender, item, addad);
	}

	public static void SendInventoryStateChanged(object sender)
	{
		OnInventoryStateChanged.Invoke(sender);
	}

	private void Update()
	{
		if (selectedUiItem != null)
		{
			selectedUiItem.SetAnchoredPositionInScreen(Input.mousePosition);
		}
	}

	private void UpdateSelectedItem()
	{
		if (selectedUiItem == null)
			return;

		if (selectedUiItem.item.state.isEmpty) {
			selectedUiItem.ReturnToSlot();
			SendUIItemSelected(this, null);
		}
	}
}
