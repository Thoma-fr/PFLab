using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using CoolDebugs;

public class PlatformGravity : Platform
{
    [Header("Gravity settings")]
    [SerializeField, Tooltip("How fast is the rigidBody traveling through the tube.")]
    private float _gravitySpeed;
    [SerializeField, Tooltip("Left and right Raycast points")]
    private Transform _leftPoint, _rightPoint;
    [SerializeField, Tooltip("Max distance of the grqvity tube."), Range(10, 500)]
    private float _tubeLengthLimit;

    private Vector2 _abovePoint, _belowPoint;
    private SpriteShapeRenderer _tubeShapeRenderer;
    private SpriteShapeController _tubeShapeController;
    private BoxCollider2D _tubeCollider;
    private List<URigidbody2D> _bodies;

    //=========================================================

    private void Awake()
    {
        _bodies = new List<URigidbody2D>();
        _tubeShapeController = GetComponent<SpriteShapeController>();
        _tubeShapeRenderer = GetComponent<SpriteShapeRenderer>();
        _tubeCollider = GetComponent<BoxCollider2D>();

        _tubeShapeController.spline.Clear();
        CalculatePoints();
    }

    /// <summary> Moves the given rigidbody in the direction the platform is facing. </summary>
    public void MoveRigidBodies()
    {
        if (_bodies.Count <= 0)
            return;

        //for(int i =  0; i < _bodies.Count; i++)
        //{
        //    _bodies[i].RigidBody2D.AddForce(9.81f * _bodies[i].RigidBody2D.gravityScale * _gravityScale * transform.up, ForceMode2D.Force);
        //}
    }

    /// <summary> Calculates the points above and below the platform. </summary>
    private void CalculatePoints()
    {
        LayerMask mask = LayerMask.GetMask("Map", "Platform", "Bouncing Platform", "Speed Platform");
        Vector2 origin = transform.position;

        //RaycastHit2D hit = Physics2D.BoxCast(origin, , transform.rotation.eulerAngles.z, transform.up.normalized, mask);

    }

    private void DrawShape()
    {
        //for (int i = 0; i < _abovePoints.Length; i++)
        //{
        //    _tubeShapeController.spline.SetPosition(i, transform.InverseTransformPoint(_abovePoints[i]));
        //}
        //
        //for (int i = 0; i < _belowPoints.Length; i++)
        //{
        //    _tubeShapeController.spline.SetPosition(i + _abovePoints.Length, transform.InverseTransformPoint(_belowPoints[i]));
        //}
    }

    private void LateUpdate()
    {
        CalculatePoints();
        DrawShape();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isGhost)
            return;

        if(collision.TryGetComponent<URigidbody2D>(out URigidbody2D urb))
        {
            StartCoroutine(LerpIn(urb));
        }
    }

    private IEnumerator LerpIn(URigidbody2D urb)
    {
        Vector2 start = urb.transform.position;
        Vector2 thisToObject = start - (Vector2)transform.position;
        float sign = Mathf.Sign(Vector2.Dot(thisToObject, (Vector2)transform.right));
        Vector2 end = start + 0.5f * -sign * (Vector2)transform.right.normalized;

        CoolDraws.DrawWireSphere(start, 0.5f, Color.red, 10);
        CoolDraws.DrawWireSphere(end, 0.5f, Color.blue, 10);

        _bodies.Add(urb);
        yield break;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_isGhost)
            return;

        if (collision.TryGetComponent<URigidbody2D>(out URigidbody2D urb))
        {
            _bodies.Remove(urb);
        }
    }

    private void FixedUpdate()
    {
        if (_isGhost)
            return;

        MoveRigidBodies();
    }
}
