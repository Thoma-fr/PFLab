using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using CoolDebugs;
using TreeEditor;

public class PlatformGravity : Platform
{
    [Header("Gravity settings")]
    [SerializeField, Tooltip("SpriteShapeController of the gravity tube.")]
    private SpriteShapeController _tubeShapeController;
    [SerializeField, Tooltip("How fast is the rigidBody traveling through the tube.")]
    private float _gravitySpeed;
    [SerializeField, Tooltip("Left and right Raycast points")]
    private Transform _leftPoint, _rightPoint;
    [SerializeField, Tooltip("Max distance of the grqvity tube."), Range(10, 500)]
    private float _tubeLengthLimit;

    private float _aboveDistance, _belowDistance;
    private BoxCollider2D _tubeCollider;
    private List<URigidbody2D> _bodies;

    //=========================================================

    private void Awake()
    {
        _bodies = new List<URigidbody2D>();
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
        Vector2 up = transform.up.normalized;
        Vector2 size = _tubeCollider.size * new Vector2(.8f, 1); // Remplacer quand on a aura la logique d'expension des platformes
        float angle = transform.rotation.eulerAngles.z;

        // Above
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, angle, up, _tubeLengthLimit, mask);
        Vector2 abovePoint, belowPoint;
        if (hit)
            abovePoint = hit.point;
        else
            abovePoint = origin + _tubeLengthLimit * up;

        // Below
        hit = Physics2D.BoxCast(origin, size, angle, -up, _tubeLengthLimit, mask);
        if (hit)
            belowPoint = hit.point;
        else
            belowPoint = origin - _tubeLengthLimit * up;

        // Calculer la distance point <> platforme
        _aboveDistance = Vector2.Dot(abovePoint - origin, up);
        _belowDistance = Vector2.Dot(belowPoint - origin, up);
    }

    private void DrawShape()
    {
        float width = _tubeCollider.size.x;
        Vector2 pointPosition = Vector2.zero;

        if(_tubeShapeController.spline.GetPointCount() == 0)
        {
            _tubeShapeController.spline.InsertPointAt(0, new Vector2(0, 0));
            _tubeShapeController.spline.InsertPointAt(1, new Vector2(1, 0));
            _tubeShapeController.spline.InsertPointAt(2, new Vector2(1, 1));
            _tubeShapeController.spline.InsertPointAt(3, new Vector2(0, 1));
        }


        for(int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    pointPosition = new Vector2(_aboveDistance, -width * .5f);
                    break;

                case 1:
                    pointPosition = new Vector2(_aboveDistance, width * .5f);
                    break;

                case 2:
                    pointPosition = new Vector2(_belowDistance, width * .5f);
                    break;

                case 3:
                    pointPosition = new Vector2(_belowDistance, -width * .5f);
                    break;

                default: break;
            }

            _tubeShapeController.spline.SetPosition(i, pointPosition);
        }

        _tubeCollider.size = new Vector2(width, _aboveDistance - _belowDistance);
        _tubeCollider.offset = transform.position - _tubeCollider.bounds.center;
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
