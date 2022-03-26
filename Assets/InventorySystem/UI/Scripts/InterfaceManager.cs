using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _uiInvenotory;

	private void Start()
	{
		_uiInvenotory?.SetActive(false);
	}

	private void Update()
    {
		if (Input.GetKeyUp(KeyCode.I))
		{
			if (inventoryOpen)
			{
				closeInventory();
			}
			else
			{
				openInventory();
			}
		}
	}

	public bool inventoryOpen => _uiInvenotory?.activeSelf ?? false;

	private void openInventory()
	{
		if (!inventoryOpen)
		{
			_uiInvenotory?.SetActive(true);
		}
	}

	private void closeInventory()
	{
		if (inventoryOpen)
		{
			_uiInvenotory?.SetActive(false);
		}
	}
}
