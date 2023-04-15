using CoolDebugs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlatformGravity : Platform
{
    [Header("Gravity settings")]
    [SerializeField, Tooltip("SpriteShapeController of the gravity tube.")]
    private SpriteShapeController _tubeShapeController;
    [SerializeField, Tooltip("How fast is the rigidBody traveling through the tube.")]
    private float _gravitySpeed;
    [SerializeField, Tooltip("Max distance of the grqvity tube."), Range(5, 10)]
    private float _tubeLengthLimit;
    [SerializeField, Tooltip("The time it take for the object to reach the center of the tube."), Range(0.01f, 1f)]
    private float _slurpDuration;
    [SerializeField, Tooltip("Slurp tweening")]
    private AnimationCurve _slurpCurve;
    [SerializeField]
    private BoxCollider2D _mouseDetectionCollider;

    private float _aboveDistance, _belowDistance;
    private BoxCollider2D _tubeCollider;
    private List<URigidbody2D> _gravityMoveBodies;
    private List<URigidbody2D> _slurpInBodies;

    //=========================================================

    private void Awake()
    {
        _gravityMoveBodies = new List<URigidbody2D>();
        _slurpInBodies = new List<URigidbody2D>();
        _tubeCollider = GetComponent<BoxCollider2D>();

        _tubeShapeController.spline.Clear();

        if (_tubeShapeController.spline.GetPointCount() == 0)
        {
            _tubeShapeController.spline.InsertPointAt(0, new Vector2(0, 0));
            _tubeShapeController.spline.InsertPointAt(1, new Vector2(0, 1));
            _tubeShapeController.spline.InsertPointAt(2, new Vector2(1, 1));
            _tubeShapeController.spline.InsertPointAt(3, new Vector2(1, 0));
        }
    }

    private void Start()
    {
        CalculatePoints();
    }

    /// <summary> Calculates the points above and below the platform. </summary>
    private void CalculatePoints()
    {
        LayerMask mask = LayerMask.GetMask("Map");
        Vector2 origin = transform.position;
        Vector2 up = transform.up.normalized;
        Vector2 size = new Vector2((_coll.size.x - 0.625f) *.8f, .3f);
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

        // Calculer la distance point <-> platforme
        _aboveDistance = Vector2.Dot(abovePoint - origin, up);
        _belowDistance = Vector2.Dot(belowPoint - origin, up);

        // Changer la taille et l'offset du collider pour le recentrer
        _tubeCollider.size = new Vector2(_coll.size.x, _aboveDistance - _belowDistance);

        // Recentrer l'offset du collider
        abovePoint = (Vector2)transform.position + _aboveDistance * up; 
        belowPoint = (Vector2)transform.position + _belowDistance * up;
        Vector2 middle = Vector2.Lerp(belowPoint, abovePoint, .5f);
        _tubeCollider.offset = new Vector2(_coll.offset.x, transform.InverseTransformPoint(middle).y);

        // Change mouse detection box collider
        _mouseDetectionCollider.size = _tubeCollider.size;
        _mouseDetectionCollider.offset = _tubeCollider.offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isGhost)
            return;

        if (collision.TryGetComponent<URigidbody2D>(out URigidbody2D urb))
        {
            if (_slurpInBodies.Contains(urb))
                return;

            urb.DisableControls(); // Au cas ou le urb est le joueur.
            StartCoroutine(LerpIn(urb));
            _slurpInBodies.Add(urb);
        }
    }

    /// <summary> Sluuurrrrpppp </summary>
    private IEnumerator LerpIn(URigidbody2D urb)
    {
        Vector2 start = urb.transform.position;
        Vector2 thisToObject = start - (Vector2)transform.position;

        float dotProduct = Vector2.Dot(thisToObject, transform.up.normalized); // Ramene le joueur dans le tube si il entre par le dessus ou le dessous
        if (dotProduct > _aboveDistance)
        {
            dotProduct = _aboveDistance * .80f;
        }
        else if (dotProduct < _belowDistance)
        {
            dotProduct = _belowDistance * .80f;
        }

        Vector2 end = transform.position + dotProduct * transform.up.normalized;

#if UNITY_EDITOR
        CoolDraws.DrawWireSphere(start, 0.5f, Color.red, 10);
        CoolDraws.DrawWireSphere(end, 0.5f, Color.blue, 10);
#endif

        urb.RigidBody2D.gravityScale = 0;
        urb.RigidBody2D.velocity = Vector2.zero;

        float t = 0;
        while (t < 1)
        {
            urb.transform.position = Vector2.LerpUnclamped(start, end, _slurpCurve.Evaluate(t));
            t += Time.fixedDeltaTime / _slurpDuration;
            yield return new WaitForFixedUpdate();
        }

        urb.EnableControls();
        urb.RigidBody2D.velocity = Vector2.zero;
        _slurpInBodies.Remove(urb);
        _gravityMoveBodies.Add(urb);

        yield break;
    }

    private void FixedUpdate()
    {
        if (_isGhost)
            return;
        else if(!_mouseDetectionCollider.enabled)
            _mouseDetectionCollider.enabled = true;

        MoveRigidBodies();
    }

    /// <summary> Moves the given rigidbody in the direction the platform is facing. </summary>
    public void MoveRigidBodies()
    {
        if (_gravityMoveBodies.Count <= 0)
            return;

        for (int i = 0; i < _gravityMoveBodies.Count; i++)
        {
            Rigidbody2D rb = _gravityMoveBodies[i].RigidBody2D;
            rb.velocity = _gravitySpeed * transform.up + transform.right * Vector2.Dot(rb.velocity, transform.right);
        }
    }

    private void LateUpdate()
    {
        _tubeShapeController.gameObject.SetActive(_canBePlaced);
        CalculatePoints();
        DrawShape();
    }

    private void DrawShape()
    {
        float width = _coll.size.x - 0.625f;
        float offset = _tubeCollider.offset.x;
        Vector2 pointPosition = Vector2.zero;

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    pointPosition = new Vector2(-width * .5f + offset, _belowDistance);
                    break;

                case 1:
                    pointPosition = new Vector2(-width * .5f + offset, _aboveDistance);
                    break;

                case 2:
                    pointPosition = new Vector2(width * .5f + offset, _aboveDistance);
                    break;

                case 3:
                    pointPosition = new Vector2(width * .5f + offset, _belowDistance);
                    break;

                default: break;
            }

            _tubeShapeController.spline.SetPosition(i, pointPosition);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_isGhost)
            return;

        if (collision.TryGetComponent<URigidbody2D>(out URigidbody2D urb))
        {
            if (_slurpInBodies.Contains(urb))
                return;

            _gravityMoveBodies.Remove(urb);
            urb.ResetGravityScale();
            urb.EnableControls();
        }
    }

    private void OnDestroy() // L'erreur quand on sort du play mode c'est NORMAL
    {
        foreach(var b in _gravityMoveBodies)
        {
            b.ResetGravityScale();
        }
        _gravityMoveBodies.Clear();

        foreach (var b in _slurpInBodies)
        {
            b.EnableControls();
            b.ResetGravityScale();
        }
        _slurpInBodies.Clear();
    }
}
