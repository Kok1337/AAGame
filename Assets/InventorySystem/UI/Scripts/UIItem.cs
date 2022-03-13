using UnityEngine;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour
{
	private Canvas _canvas;
	private RectTransform _itemRectTransform;
	private UISlot _uiSlot;

	public Canvas canvas { get => _canvas; }
	public RectTransform rectTransform { get => _itemRectTransform; }
	public Vector2 slotPosition { get => _uiSlot?.rectTransform?.localPosition ?? Vector3.zero; }

	private void Start()
	{
		_itemRectTransform = GetComponent<RectTransform>();
		_canvas = GetComponentInParent<Canvas>();
		_uiSlot = GetComponentInParent<UISlot>();
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
}
