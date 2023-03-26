using System.Collections;
using UnityEditor;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _minSpeed;
    [SerializeField]
    private float _accelerationStrength;
    [SerializeField]
    private float _airAccelerationFactor;
    [SerializeField]
    private float _deccelerationStrength;

    [Header("Jump")]
    [SerializeField, Range(0, 2f)]
    private float _groundCheckDistance;
    [SerializeField, Range(0, 2f)]
    private float _headCheckDistance;
    [SerializeField]
    private AnimationCurve _jumpVelocityCurve;
    [SerializeField]
    private float _jumpStrength;
    [SerializeField]
    private float _minJumpDuration;
    [SerializeField]
    private float _maxJumpDuration;
    [SerializeField]
    private float _jumpDurationMultiplier;
    private float _defaultGravityScale;
    private float _currentJumpDuration;
    private Coroutine _jumpCoroutine;

    private Rigidbody2D _rb2D;

    //==========================================================================

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rb2D.gravityScale;
    }

    private void Update()
    {
        if(Input.anyKey) // A modifier pour le passage vers le new input system
        {
            if (Input.GetKey(KeyCode.D))
            {
                Acceleration(Vector2.right);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                Acceleration(Vector2.left);
            }
        }
        else
        {
            if (_rb2D.velocity.sqrMagnitude > 0 && GroundCheck(~LayerMask.GetMask("Bouncing Platform", "Speed Platform")))
                Decceleration();
        }

        if(GroundCheck(~LayerMask.GetMask("Bouncing Platform", "Speed Platform")))
            ClampSpeed(_maxSpeed);

        if(Input.GetKey(KeyCode.Space))
        {
            if (GroundCheck(~LayerMask.GetMask("Bouncing Platform")) && _jumpCoroutine == null)
                _jumpCoroutine = StartCoroutine(JumpCoroutine());
            else if (_jumpCoroutine != null)
                IncreaseJumpDuration(Time.deltaTime * _jumpDurationMultiplier);
        }
    }

    private bool GroundCheck(LayerMask layerMask)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, .5f, Vector2.down, _groundCheckDistance, layerMask);

#if UNITY_EDITOR
        if (hit)
            CoolDebugs.CoolDebugs.DrawWireSphere((Vector2)transform.position + _groundCheckDistance * Vector2.down, .5f, Color.green, Time.fixedDeltaTime);
        else
            CoolDebugs.CoolDebugs.DrawWireSphere((Vector2)transform.position + _groundCheckDistance * Vector2.down, .5f, Color.red, Time.fixedDeltaTime);
#endif

        return hit;
    }

    private bool HeadCheck()
    {
        return Physics2D.CircleCast(transform.position, .5f, Vector2.up, _headCheckDistance);
    }

    public void ClampSpeed(float maxSpeed)
    {
        if(_rb2D.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            _rb2D.velocity = maxSpeed * _rb2D.velocity.normalized;
    }

    private void Decceleration()
    {
        _rb2D.velocity -= _deccelerationStrength * Time.deltaTime * _rb2D.velocity.normalized;

        if (_rb2D.velocity.sqrMagnitude < _minSpeed * _minSpeed)
            _rb2D.velocity = Vector2.zero;
    }

    private void Acceleration(Vector2 direction)
    {
        if (GroundCheck(LayerMask.GetMask("Speed Platform")))
            return;
        
        if (GroundCheck(~LayerMask.GetMask("Bouncing Platform")))
            _rb2D.velocity += _accelerationStrength * Time.deltaTime * direction;
        else
            _rb2D.velocity += _accelerationStrength * _airAccelerationFactor * Time.deltaTime * direction;
    }
    
    private IEnumerator JumpCoroutine()
    {
        _defaultGravityScale = _rb2D.gravityScale;
        _rb2D.gravityScale = 0;
        _currentJumpDuration = _minJumpDuration;

        float t = 0;
        while(t < _currentJumpDuration)
        {
            if (HeadCheck())
                break;

            _rb2D.velocity = new Vector2(_rb2D.velocity.x, _jumpStrength * _jumpVelocityCurve.Evaluate(t / _currentJumpDuration));

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        StopJumpCoroutine();
        yield break;
    }

    private void IncreaseJumpDuration(float amount)
    {
        _currentJumpDuration = Mathf.Clamp(_currentJumpDuration + amount, _minJumpDuration, _maxJumpDuration);
    }

    public void StopJumpCoroutine() // Utile pour la platforme de gravity
    {
        if( _jumpCoroutine != null )
            StopCoroutine(_jumpCoroutine);

        _rb2D.gravityScale = _defaultGravityScale;
        _jumpCoroutine = null;
    }

#if UNITY_EDITOR
    // =====================================================================

    private void OnDrawGizmosSelected()
    {
        if(_maxJumpDuration < _minJumpDuration)
            _maxJumpDuration = _minJumpDuration;

        GUIStyle labels = new GUIStyle();
        labels.alignment = TextAnchor.MiddleCenter;
        labels.normal.textColor = Color.black;
        labels.fontStyle = FontStyle.Bold;
        labels.fontSize = 20;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + _groundCheckDistance * Vector2.down, .5f);
        Handles.Label((Vector2)transform.position + _groundCheckDistance * Vector2.down, "Ground", labels);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere((Vector2)transform.position + _headCheckDistance * Vector2.up, .5f);
        Handles.Label((Vector2)transform.position + _headCheckDistance * Vector2.up, "Head", labels);
    }
#endif
}
