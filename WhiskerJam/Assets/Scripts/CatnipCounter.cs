using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CatnipCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI catnipMessage;

    // Update is called once per frame
    void Update()
    {
        if (Global.collectiblesAcquired <= 3)
        {
            catnipMessage.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1));
        }
        else
        {
            catnipMessage.color = Color.Lerp(Color.green, Color.magenta, Mathf.PingPong(Time.time, 1));
        }

        catnipMessage.text = "You collected " + Global.collectiblesAcquired + "/" + "4 catnips!";
    }
}
