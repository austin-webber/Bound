using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(0f, 0f, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y <= 5f)
        {
            gameObject.transform.position = new Vector3(0f, 0f, -10f);
        }

        if (player.transform.position.y > 5f)
        {
            gameObject.transform.position = new Vector3(transform.position.x, 10.3f, transform.position.z);
        }

        if (player.transform.position.y > 15.6f)
        {
            gameObject.transform.position = new Vector3(transform.position.x, 20.6f, transform.position.z);
        }

        if (player.transform.position.y > 25.3f)
        {
            gameObject.transform.position = new Vector3(transform.position.x, 30.8f, transform.position.z);
        }

        if (player.transform.position.y > 35.8f)
        {
            gameObject.transform.position = new Vector3(transform.position.x, 41.1f, transform.position.z);
        }
    }
}
