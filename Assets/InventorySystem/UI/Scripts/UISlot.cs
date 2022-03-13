using UnityEngine;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IPointerDownHandler
{
	private RectTransform _rectTransform;
	private UIItem _uiItem;

	public RectTransform rectTransform { get => _rectTransform; }
	public UIItem uiItem { get => _uiItem; }

	private void Start()
	{
		_rectTransform = GetComponent<RectTransform>();
		_uiItem = GetComponentInChildren<UIItem>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		transform.SetAsLastSibling();
		InventoryManager.SelectUIItem(uiItem);
	}
}
