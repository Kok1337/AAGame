using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class PickupableItem : MonoBehaviour
{
	[SerializeField]
	private int _amount;
	[SerializeField]
	private InventoryItemInfo _itemInfo;

	private SpriteRenderer _spriteRenderer;

	public float selectedOutlineThickness = 0.0107f;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
    {
		_spriteRenderer.sprite = _itemInfo.spriteIcon;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
			_spriteRenderer.material.SetFloat("_OutlineThickness", selectedOutlineThickness);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Player")
			_spriteRenderer.material.SetFloat("_OutlineThickness", 0);
	}
}
