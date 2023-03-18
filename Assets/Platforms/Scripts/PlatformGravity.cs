using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Collections.Generic;

public class PlatformGravity : Platform
{
    [Header("Gravity settings")]
    [SerializeField, Tooltip("How much force this platform exerts on the object in relation to its gravity.")]
    private float _gravityScale;
    [SerializeField, Tooltip("Left and right Raycast points")]
    private Transform _leftPoint, _rightPoint;
    [SerializeField, Tooltip("Number of raycasts cast above and below the platform."), Range(2, 100)]
    private int _raycastPrecision;
    [SerializeField, Tooltip("Max distance of the raycast."), Range(10, 500)]
    private float _raycastLengthLimit;

    private Vector2[] _abovePoints, _belowPoints;
    private SpriteShapeRenderer _tubeShapeRenderer;
    private SpriteShapeController _tubeShapeController;
    private PolygonCollider2D _tubeCollider;
    private List<URigidbody2D> _bodies;

    //=========================================================

    private void Awake()
    {
        _bodies = new List<URigidbody2D>();
        _abovePoints = new Vector2[_raycastPrecision];
        _belowPoints = new Vector2[_raycastPrecision];
        _tubeShapeController = GetComponent<SpriteShapeController>();
        _tubeShapeRenderer = GetComponent<SpriteShapeRenderer>();
        _tubeCollider = GetComponent<PolygonCollider2D>();

        CalculatePoints();
        FirstDrawShape();
    }

    /// <summary> Moves the given rigidbody in the direction the platform is facing. </summary>
    public void MoveRigidBodies()
    {
        if (_bodies.Count <= 0)
            return;

        for(int i =  0; i < _bodies.Count; i++)
        {
            _bodies[i].Rb.AddForce(9.81f * _bodies[i].Rb.gravityScale * _gravityScale * transform.up, ForceMode2D.Force);
        }
    }

    /// <summary> Calculates the points above and below the platform. </summary>
    private void CalculatePoints()
    {
        LayerMask mask = LayerMask.GetMask("Map", "Platform");
        RaycastHit2D hit;
        Vector2 origin;

        for (int i = 0; i < _abovePoints.Length; i++)
        {
            origin = Vector2.Lerp(_leftPoint.position, _rightPoint.position, (float)i / (float)(_raycastPrecision - 1));
            hit = Physics2D.Raycast(origin, transform.up, _raycastLengthLimit, mask);
            _abovePoints[i] = hit ? hit.point : origin + (Vector2)transform.up * _raycastLengthLimit;
        }

        for (int i = 0; i < _belowPoints.Length; i++)
        {
            origin = Vector2.Lerp(_rightPoint.position, _leftPoint.position, (float)i / (float)(_raycastPrecision - 1));
            hit = Physics2D.Raycast(origin, -transform.up, _raycastLengthLimit, mask);
            _belowPoints[i] = hit ? hit.point : origin - (Vector2)transform.up * _raycastLengthLimit;
        }
    }

    private void FirstDrawShape()
    {
        _tubeShapeController.spline.Clear();
        _tubeCollider.points = new Vector2[_abovePoints.Length + _belowPoints.Length];
        for (int i = 0; i < _abovePoints.Length; i++)
        {
            _tubeShapeController.spline.InsertPointAt(i, transform.InverseTransformPoint(_abovePoints[i]));
            _tubeCollider.points[i] = transform.InverseTransformPoint(_abovePoints[i]);
        }

        for (int i = 0; i < _belowPoints.Length; i++)
        {
            _tubeShapeController.spline.InsertPointAt(i + _abovePoints.Length, transform.InverseTransformPoint(_belowPoints[i]));
            _tubeCollider.points[i + _abovePoints.Length] = transform.InverseTransformPoint(_belowPoints[i]);
        }
    }

    private void DrawShape()
    {
        for (int i = 0; i < _abovePoints.Length; i++)
        {
            _tubeShapeController.spline.SetPosition(i, transform.InverseTransformPoint(_abovePoints[i]));
        }

        for (int i = 0; i < _belowPoints.Length; i++)
        {
            _tubeShapeController.spline.SetPosition(i + _abovePoints.Length, transform.InverseTransformPoint(_belowPoints[i]));
        }
    }

    private void LateUpdate()
    {
        CalculatePoints();
        DrawShape();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<URigidbody2D>(out URigidbody2D urp))
        {
            _bodies.Add(urp);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<URigidbody2D>(out URigidbody2D urp))
        {
            _bodies.Remove(urp);
        }
    }

    private void FixedUpdate()
    {
        MoveRigidBodies();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!UnityEditor.EditorApplication.isPlaying)
            return;

        Vector2 origin;

        Gizmos.color = Color.red;
        for (int i = 0; i < _abovePoints.Length; i++)
        {
            origin = Vector2.Lerp(_leftPoint.position, _rightPoint.position, (float)i / (float)(_raycastPrecision - 1));
            Gizmos.DrawLine(origin, _abovePoints[i]);
            Gizmos.DrawWireSphere(_abovePoints[i], 0.25f);
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < _belowPoints.Length; i++)
        {
            origin = Vector2.Lerp(_rightPoint.position, _leftPoint.position, (float)i / (float)(_raycastPrecision - 1));
            Gizmos.DrawLine(origin, _belowPoints[i]);
            Gizmos.DrawWireSphere(_belowPoints[i], 0.25f);
        }
    }
#endif
}
