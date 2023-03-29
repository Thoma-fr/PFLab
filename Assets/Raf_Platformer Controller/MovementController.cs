using System.Collections;
using UnityEditor;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody2D _rb2D;

    [Header("Movement")]
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _minSpeed;
    [SerializeField, Tooltip("Taux d'acceleration.")]
    private float _accelerationStrength;
    [SerializeField, Tooltip("Multiplicateur d'acceleration lorsque le joueur est dans les airs.")]
    private float _airAccelerationFactor;
    [SerializeField, Tooltip("Force de freinage.")]
    private float _deccelerationStrength;
    public float MovementDirection { get; set; }

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

    private float _defaultGravityScale;
    private float _currentJumpDuration;
    private Coroutine _jumpCoroutine;
    public bool HoldingJump { get; set; }


    //==========================================================================

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rb2D.gravityScale;
    }

    private void Update() // A modifier pour le passage vers le new input system
    {
        if(MovementDirection != 0)
            Acceleration(MovementDirection);
        else
        {
            // Si le joueur se deplace et qu'il touche le sol, on le ralenti
            if (_rb2D.velocity.sqrMagnitude > 0 && GroundCheck(~LayerMask.GetMask("Bouncing Platform", "Speed Platform")))
                Decceleration();
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

    /// <summary> Limite la vitesse a maxSpeed </summary>
    public void ClampSpeed(float maxSpeed)
    {
        if(_rb2D.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            _rb2D.velocity = maxSpeed * _rb2D.velocity.normalized;
    }

    /// <summary> Ralenti le joueur au cours du temps. </summary>
    private void Decceleration()
    {
        _rb2D.velocity -= _deccelerationStrength * Time.deltaTime * _rb2D.velocity.normalized;

        if (_rb2D.velocity.sqrMagnitude < _minSpeed * _minSpeed)
            _rb2D.velocity = Vector2.zero;
    }

    /// <summary> Accelere le joueur au cours du temps dans la direction donnee. </summary>
    private void Acceleration(float direction)
    {
        if (GroundCheck(LayerMask.GetMask("Speed Platform")))
            return;
        
        if (GroundCheck(~LayerMask.GetMask("Bouncing Platform")))
            _rb2D.velocity += _accelerationStrength * Time.deltaTime * new Vector2(direction, 0).normalized;
        else
            _rb2D.velocity += _accelerationStrength * _airAccelerationFactor * Time.deltaTime * new Vector2(direction, 0).normalized;
    }

    private IEnumerator JumpCoroutine()
    {
        // Ajustement des valeurs avant d'initier le saut
        _defaultGravityScale = _rb2D.gravityScale;
        _rb2D.gravityScale = 0;
        _currentJumpDuration = _minJumpDuration;

        // Boucle de saut, change la vitesse verticale du joueur au cours du temps
        float t = 0;
        while(t < _currentJumpDuration)
        {
            // Petit check pour voir si on se cogne pas
            if (HeadCheck())
                break;

            _rb2D.velocity = new Vector2(_rb2D.velocity.x, _jumpStrength * _jumpVelocityCurve.Evaluate(t / _currentJumpDuration));

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
