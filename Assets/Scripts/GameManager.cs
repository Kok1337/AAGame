using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	public static readonly UnityEvent<object, GameObject> OnDropItem = new UnityEvent<object, GameObject>();

	public static void SendDropItem(object sender, GameObject item)
	{
		OnDropItem?.Invoke(sender, item);
	}
}
