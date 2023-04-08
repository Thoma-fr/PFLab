using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
public class ActivateMovingPlateform : MonoBehaviour
{
    public GameObject movingPlateform;
    public float rotationSpeed;
    public bool canMove;
    public Transform targetA;
    public Transform targetB;
    public float speed = 2f;
    public float backSpeed = 2f;
    private Vector3 _nextTarget;
   
    void Start()
    {
        _nextTarget = targetA.position;
    }
    private void Update()
    {
           
    }
    void FixedUpdate()
    {
        if (canMove)
        {
            //transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetB.position, speed * Time.fixedDeltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetA.position, speed * Time.fixedDeltaTime);
        }
        //if (movingPlateform.transform.position == targetA.position)
        //{
        //    _nextTarget = targetB.position;
        //}
        //else if (movingPlateform.transform.position == targetB.position)
        //{
        //    _nextTarget = targetA.position;
        //}
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
