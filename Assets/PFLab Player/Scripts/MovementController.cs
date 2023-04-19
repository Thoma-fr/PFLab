using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class MovementController : MonoBehaviour
{
    private URigidbody2D _urb;
    private InputController _inputs;
    private CapsuleCollider2D _collider;
    public static MovementController instance;
    [Header("Movement")]
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _minSpeed;
    [SerializeField, Tooltip("Taux d'acceleration.")]
    private float _accelerationStrength;
    [SerializeField, Tooltip("Multiplicateur d'acceleration lorsque le joueur est dans les airs.")]
    private float _airAccelerationFactor;
    [SerializeField]
    private float _maxAirSpeed;
    [SerializeField, Tooltip("Force de freinage.")]
    private float _deccelerationStrength;
    public Vector2 MovementDirection { get; set; }

    [Header("Jump")]
    [SerializeField, Range(0, 2f)]
    private float _groundCheckDistance;
    [SerializeField, Range(0, 2f)]
    private float _headCheckDistance;
    [SerializeField, Tooltip("Courbe de la vitesse verticale du joueur pendant toute la duree du saut.")]
    private AnimationCurve _jumpVelocityCurve;
    [SerializeField, Tooltip("Vitesse du saut lorsque f(t) = 1 sur la courbe.")]
    private float _jumpStrength;
    [SerializeField, Tooltip("Duree minimale du saut.")]
    private float _minJumpDuration;
    [SerializeField, Tooltip("Duree maximale du saut")]
    private float _maxJumpDuration;
    [SerializeField, Tooltip("Vitesse a laquelle la duree du saut passe du min au max pendant qu'on reste appuye sur la touche de saut.")]
    private float _jumpDurationMultiplier;

    private float _currentJumpDuration;
    private Coroutine _jumpCoroutine;
    public bool HoldingJump { get; set; }

    private Animator _Animator;
    private SpriteRenderer _spriteRenderer;
    private VisualEffect _dustParticle;
    //==========================================================================

    private void Awake()
    {
        _urb = GetComponent<URigidbody2D>();
        _inputs = GetComponent<InputController>();
        _collider = GetComponent<CapsuleCollider2D>();
        _Animator= GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _dustParticle = GetComponent<VisualEffect>();

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if(_urb.RigidBody2D.gravityScale != 0)
        {
            if(MovementDirection.x != 0)
                Acceleration(MovementDirection);
            else
            {
                // Si le joueur se deplace et qu'il touche le sol, on le ralenti
                if (_urb.RigidBody2D.velocity.sqrMagnitude > 0 && GroundCheck(~LayerMask.GetMask("Bouncing Platform", "Speed Platform")))
                    Decceleration();
            }
        }
        else // Joueur dans gravity tube
        {
            if (MovementDirection != Vector2.zero)
                Acceleration(MovementDirection);
        }

        // Si le joueur touche le sol excepte les plateformes de bounce et de speed, on clamp sa vitesse.
        if(GroundCheck(~LayerMask.GetMask("Bouncing Platform", "Speed Platform")))
            ClampSpeed(_maxSpeed);

        if (HoldingJump)
        {
            if (GroundCheck(~LayerMask.GetMask("Bouncing Platform")) && _jumpCoroutine == null)
                _jumpCoroutine = StartCoroutine(JumpCoroutine());
            else if (_jumpCoroutine != null)
                IncreaseJumpDuration(Time.deltaTime * _jumpDurationMultiplier);
        }
        VisualThings();
    }
    private void VisualThings()
    {
        flipX();
        _Animator.SetBool("IsGrounded", GroundCheck(~LayerMask.GetMask("Bouncing Platform", "Speed Platform")));
        _Animator.SetFloat("SpeedX", MathF.Abs(_urb.RigidBody2D.velocity.x));
        if (GroundCheck(~LayerMask.GetMask("Bouncing Platform", "Speed Platform")) && _urb.RigidBody2D.velocity.x != 0)
        {
            _dustParticle.SetBool("IsWalking", true);
            _dustParticle.SetFloat("DirVel", _urb.RigidBody2D.velocity.x * -1);
        }
        else
        {
            _dustParticle.SetBool("IsWalking", false);
        }
    }
    private bool GroundCheck(LayerMask layerMask)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _collider.size.x * .5f, Vector2.down, _groundCheckDistance, layerMask);

#if UNITY_EDITOR
        if (hit)
            CoolDebugs.CoolDraws.DrawWireSphere((Vector2)transform.position + _groundCheckDistance * Vector2.down, _collider.size.x * .5f, Color.green, Time.fixedDeltaTime);
        else
            CoolDebugs.CoolDraws.DrawWireSphere((Vector2)transform.position + _groundCheckDistance * Vector2.down, _collider.size.x * .5f, Color.red, Time.fixedDeltaTime);
#endif

        return hit;
    }

    private bool HeadCheck()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _collider.size.x * .5f, Vector2.up, _headCheckDistance);
        return (hit && !hit.collider.usedByEffector);
    }

    /// <summary> Limite la vitesse a maxSpeed </summary>
    public void ClampSpeed(float maxSpeed)
    {
        if(_urb.RigidBody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            _urb.RigidBody2D.velocity = maxSpeed * _urb.RigidBody2D.velocity.normalized;
    }

    /// <summary> Ralenti le joueur au cours du temps. </summary>
    private void Decceleration()
    {
        _urb.RigidBody2D.velocity -= _deccelerationStrength * Time.deltaTime * _urb.RigidBody2D.velocity.normalized;

        if (_urb.RigidBody2D.velocity.sqrMagnitude < _minSpeed * _minSpeed)
            _urb.RigidBody2D.velocity = Vector2.zero;
    }

    /// <summary> Accelere le joueur au cours du temps dans la direction donnee. </summary>
    private void Acceleration(Vector2 direction)
    {
        if (GroundCheck(LayerMask.GetMask("Speed Platform")))
            return;

        if (GroundCheck(~LayerMask.GetMask("Bouncing Platform"))) // On ground
            _urb.RigidBody2D.velocity += _accelerationStrength * Time.deltaTime * new Vector2(direction.x, 0);
        else if(!_urb.InGravityTube) // In air but not in gravity tube
        {
            _urb.RigidBody2D.velocity += _accelerationStrength * _airAccelerationFactor * Time.deltaTime * new Vector2(direction.x, 0);
            if(Mathf.Abs(_urb.RigidBody2D.velocity.x) > _maxAirSpeed)
                _urb.RigidBody2D.velocity = new Vector2(_maxAirSpeed * direction.normalized.x, _urb.RigidBody2D.velocity.y);
        }

        if (_urb.InGravityTube)// In air and in gravityTube
        {
            _urb.RigidBody2D.velocity += _accelerationStrength * _airAccelerationFactor * Time.deltaTime * direction;
        }

    }

    private IEnumerator JumpCoroutine()
    {
        // Ajustement des valeurs avant d'initier le saut
        _urb.RigidBody2D.gravityScale = 0;
        _currentJumpDuration = _minJumpDuration;
        _Animator.SetTrigger("Jump");
        // Boucle de saut, change la vitesse verticale du joueur au cours du temps
        float t = 0;
        while(t < _currentJumpDuration)
        {
            if (!_inputs.ControlsEnabled)
            {
                _jumpCoroutine = null;
                yield break;
            }

            // Petit check pour voir si on se cogne pas
            if (HeadCheck())
                break;

            _urb.RigidBody2D.velocity = new Vector2(_urb.RigidBody2D.velocity.x, _jumpStrength * _jumpVelocityCurve.Evaluate(t / _currentJumpDuration));

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        StopJumpCoroutine();
        yield break;
    }

    /// <summary> Augmente le temps du saut par amount </summary>
    private void IncreaseJumpDuration(float amount)
    {
        _currentJumpDuration = Mathf.Clamp(_currentJumpDuration + amount, _minJumpDuration, _maxJumpDuration);
    }

    /// <summary> Post saut, reajuste les valeurs pour les mettre par defaut. </summary>
    public void StopJumpCoroutine() // Utile pour la platforme de gravity
    {
        if( _jumpCoroutine != null )
            StopCoroutine(_jumpCoroutine);

        _urb.ResetGravityScale();
        _jumpCoroutine = null;
    }
    private void flipX()
    {
        if (_urb.RigidBody2D.velocity.x > 0.3)
            _spriteRenderer.flipX = false;
        else
            _spriteRenderer.flipX = true;
    }
#if UNITY_EDITOR
    // =====================================================================

    private void OnDrawGizmosSelected()
    {

        if(_collider == null)
            _collider = GetComponent<CapsuleCollider2D>();

        if (_maxJumpDuration < _minJumpDuration)
            _maxJumpDuration = _minJumpDuration;

        GUIStyle labels = new();
        labels.alignment = TextAnchor.MiddleCenter;
        labels.normal.textColor = Color.black;
        labels.fontStyle = FontStyle.Bold;
        labels.fontSize = 20;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + _groundCheckDistance * Vector2.down, _collider.size.x * .5f);
        Handles.Label((Vector2)transform.position + _groundCheckDistance * Vector2.down, "Ground", labels);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere((Vector2)transform.position + _headCheckDistance * Vector2.up, _collider.size.x * .5f);
        Handles.Label((Vector2)transform.position + _headCheckDistance * Vector2.up, "Head", labels);
    }
#endif
}
