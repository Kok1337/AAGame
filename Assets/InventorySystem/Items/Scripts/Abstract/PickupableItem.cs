using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteAlways]
public abstract class PickupableItem : MonoBehaviour
{
	[SerializeField]
	private int _amount = 1;
	[SerializeField]
	private InventoryItemInfo _itemInfo;

	private SpriteRenderer _spriteRenderer;

	private static HashSet<PickupableItem> outlinedItems = new HashSet<PickupableItem>();

	public float selectedOutlineThickness = 0.005f;

	public InventoryItemInfo ItemInfo => _itemInfo;
	public int Amount => _amount;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
    {
		if (!Application.IsPlaying(gameObject))
		{
			_spriteRenderer.sprite = _itemInfo.spriteIcon;
		}
	}

	private void Update()
	{
		if (!Application.IsPlaying(gameObject) && _spriteRenderer.sprite == null)
		{
			_spriteRenderer.sprite = _itemInfo.spriteIcon;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			_spriteRenderer.material.SetFloat("_OutlineThickness", selectedOutlineThickness);
			if (!outlinedItems.Contains(this))
			{
				outlinedItems.Add(this);
			}
		}
	}

	public void UpdateAmount(int amount)
	{
		_amount = amount;
		if (_amount == 0)
		{
			DestroyGameObject();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			_spriteRenderer.material.SetFloat("_OutlineThickness", 0);
			if (outlinedItems.Contains(this))
			{
				outlinedItems.Remove(this);
			}
		}
	}

	public abstract IInventoryItem ToIInventoryItem();

	public static PickupableItem FindNearestPickupableItem(Transform objectTransform)
	{
		Vector2 gameObjectPositiopn = new Vector2(objectTransform.position.x, objectTransform.position.y);
		PickupableItem nearestItem = null;
		float nearestDistance = Mathf.Infinity;
		foreach (PickupableItem item in outlinedItems)
		{
			Vector2 itemPositiopn = new Vector2(item.transform.position.x, item.transform.position.y);
			float distance = Vector2.Distance(itemPositiopn, gameObjectPositiopn);
			if (distance < nearestDistance)
			{
				nearestDistance = distance;
				nearestItem = item;
			}
		}
		return nearestItem;
	}

	public void DestroyGameObject()
	{
		Destroy(gameObject);
	}
}
