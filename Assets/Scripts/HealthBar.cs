using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;
    public float fill;

    // Start is called before the first frame update
    void Start()
    {
        fill = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        bar.fillAmount = fill;

        if (Input.GetKey("space"))
        {
            fill -= 0.1f * Time.deltaTime;
        }
    }
}
