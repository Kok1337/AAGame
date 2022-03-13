using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IPointerDownHandler
{
	[SerializeField] UIInventoryItem _uiItem;

	private UIInventory _uiInventory;

	public RectTransform rectTransform { get; private set; }
	public UIInventoryItem uiItem { get => _uiItem; private set => _uiItem = value; }
	public IInventorySlot slot { get; private set; }

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		_uiInventory = GetComponentInParent<UIInventory>();
		uiItem = GetComponentInChildren<UIInventoryItem>();
	}

	public void SetSlot(IInventorySlot newSlot)
	{
		slot = newSlot;
		Refresh();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			var otherItemUI = InventoryManager.selectedUiItem;

			if (otherItemUI == null)
			{
				if (uiItem.item != null)
				{
					Debug.Log("InventoryManager.SendUIItemSelected(this, uiItem);");
					InventoryManager.SendUIItemSelected(this, uiItem);
					transform.SetAsLastSibling();
				}
			}
			else
			{
				var otherSlotUI = otherItemUI.uiSlot;
				if (otherSlotUI == this)
				{
					uiItem.ReturnToSlot();
					InventoryManager.SendUIItemSelected(this, null);
				}
				else
				{
					var otherSlot = otherSlotUI.slot;
					var inventory = _uiInventory.inventory;

					inventory.TransitFromSlotToSlot(this, otherSlot, slot);
				}	
			}
		}
	}

	public void Refresh()
	{
		if (slot != null)
		{
			uiItem.Refresh(slot);
		}
	}
}