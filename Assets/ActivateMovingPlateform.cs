using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
public class ActivateMovingPlateform : MonoBehaviour
{
    public bool canMove;
    public Transform targetA;
    public Transform targetB;
    public float speed = 2f;
    public float backSpeed = 2f;
    void FixedUpdate()
    {
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetB.position, speed * Time.fixedDeltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetA.position, speed * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
