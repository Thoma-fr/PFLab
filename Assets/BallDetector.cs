using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BallDetector : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    public bool isPressed = false;

    public GameObject thingToActive;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Balls"))
        {
            thingToActive.SetActive(false);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Balls"))
        {
            thingToActive.SetActive(true);
        }
    }
}

