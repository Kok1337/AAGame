using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour
{
	[SerializeField] private Image _imageIcon;
	[SerializeField] private Text _textAmount;

	public UIInventorySlot uiSlot { get; private set; }
	public IInventoryItem item { get; private set; }
	public Canvas canvas { get; private set; }
	public RectTransform rectTransform { get; private set; }
	public Vector2 slotPosition { get => uiSlot?.rectTransform?.localPosition ?? Vector3.zero; }

	private void Start()
	{
		canvas = GetComponentInParent<Canvas>();
		uiSlot = GetComponentInParent<UIInventorySlot>();
		rectTransform = GetComponent<RectTransform>();
	}

	public void SetAnchoredPositionInScreen(Vector3 newPosition)
	{
		Vector2 screenPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out screenPosition);
		rectTransform.anchoredPosition = screenPosition - slotPosition;
	}

	public void ReturnToSlot()
	{
		rectTransform.anchoredPosition = Vector3.zero;
	}

	public void Refresh(IInventorySlot slot)
	{
		if (slot.isEmpty)
		{
			Cleanup();
			return;
		}

		item = slot.item;

		_imageIcon.gameObject.SetActive(true);
		_imageIcon.sprite = item.metadata.spriteIcon;

		_textAmount.gameObject.SetActive(slot.amount > 1);
		_textAmount.text = $"x{slot.amount}";
	}

	public void Cleanup()
	{
		_textAmount.gameObject.SetActive(false);
		_imageIcon.gameObject.SetActive(false);
		ReturnToSlot();
		item = null;
	}
}
