using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField, Range(0, 2f)]
    private float _groundCheckDistance;

    [Header("Movement")]
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _accelerationStrength;
    [SerializeField]
    private float _airAccelerationFactor;
    [SerializeField]
    private float _deccelerationStrength;

    private Rigidbody2D _rb2D;

    //==========================================================================

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
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
            if (_rb2D.velocity.sqrMagnitude > 0 && GroundCheck())
                Decceleration();
        }

        if(GroundCheck())
            CapSpeed();
    }

    private bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, .5f, Vector2.down, _groundCheckDistance, ~LayerMask.GetMask("Bouncing Platform"));

        if (hit)
            CoolDebugs.CoolDebugs.DrawWireSphere((Vector2)transform.position + _groundCheckDistance * Vector2.down, .5f, Color.green, Time.fixedDeltaTime);
        else
            CoolDebugs.CoolDebugs.DrawWireSphere((Vector2)transform.position + _groundCheckDistance * Vector2.down, .5f, Color.red, Time.fixedDeltaTime);

        return hit;
    }
    private void CapSpeed()
    {
        if(_rb2D.velocity.sqrMagnitude > _maxSpeed * _maxSpeed)
            _rb2D.velocity = _maxSpeed * _rb2D.velocity.normalized;
    }

    private void Decceleration()
    {
        _rb2D.velocity -= _deccelerationStrength * _rb2D.velocity.normalized;

        if (_rb2D.velocity.sqrMagnitude < 0.5f)
        {
            _rb2D.velocity = Vector2.zero;
        }
    }

    private void Acceleration(Vector2 direction)
    {
        if (GroundCheck())
            _rb2D.velocity += _accelerationStrength * direction;
        else
            _rb2D.velocity += _accelerationStrength * _airAccelerationFactor * direction;
    }
}
