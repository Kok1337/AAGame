using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboardController : MonoBehaviour
{
    public Player Player;

    private void Start()
    {
        Player = Player ?? GetComponent<Player>();
        if (Player == null)
        {
            Debug.LogError("Игрок не задан");
        }
    }

    private void Update()
    {
        if (Player != null)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            Vector2 direction = new Vector2(x, y);
            float magnitude = direction.magnitude;

            if (magnitude != 0)
            {
                Player.MovementByDirectionFromKeyBoard(magnitude > 1 ? direction.normalized : direction);
            }

			if (Input.GetKeyUp(KeyCode.F))
			{
				Player.PickupNearestItem();
			}
        }
    }
}
