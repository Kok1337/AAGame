using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{

	private static UIItem selectedUiItem = null;

	public static readonly UnityEvent<UIItem> OnUIItemSelected = new UnityEvent<UIItem>();

	private void Start() {}

	public static void SelectUIItem(UIItem uiItem)
	{
		selectedUiItem = uiItem;
		Debug.Log("SelectUIItem");
		OnUIItemSelected.Invoke(uiItem);
	}

	private void Update()
	{
		if (selectedUiItem != null)
		{
			selectedUiItem.SetAnchoredPositionInScreen(Input.mousePosition);

			if (Input.GetMouseButtonDown(1))
			{
				selectedUiItem.ReturnToSlot();
				selectedUiItem = null;		
			}
		}
	}
}
