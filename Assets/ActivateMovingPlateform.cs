using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateMovingPlateform : MonoBehaviour,IlaserAble
{
    public GameObject movingPlateform;
    public bool canMove;
    public Transform targetA;
    public Transform targetB;
    public float speed = 2f;

    private Vector3 _nextTarget;
    public void LaserReaction()
    {
        canMove=true;
    }
    public void LaserStop()
    {
        canMove = false;
    }
    void Start()
    {
        _nextTarget = targetA.position;
    }

    void FixedUpdate()
    {
        if (canMove)
            transform.position = Vector3.MoveTowards(transform.position, _nextTarget, speed * Time.fixedDeltaTime);

        if (transform.position == targetA.position)
        {
            _nextTarget = targetB.position;
        }
        else if (transform.position == targetB.position)
        {
            _nextTarget = targetA.position;
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
