using UnityEngine;

// "Useful" Rigidbody2D. Adds some useful physics stuff.
public class URigidbody2D : MonoBehaviour
{
    [SerializeField]
    private float _defaultGravityScale;

    private Rigidbody2D _rigidBody2D;
    private Collider2D _collider2D;
    public Vector2 LastFrameVelocity { get; private set; }
    public Rigidbody2D RigidBody2D => _rigidBody2D;

    public bool InGravityTube;

    // Player stuff
    private InputController _inputs;

    //=========================================================

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _rigidBody2D.gravityScale = _defaultGravityScale;
        TryGetComponent(out _inputs);
    }

    void LateUpdate() // Changer en fixedUpdate si la physique est caca
    {
        LastFrameVelocity = _rigidBody2D.velocity;
    }

    public void DisableControls()
    {
        if(_inputs)
            _inputs.DisableControls();
    }
    public void EnableControls()
    {
        if (_inputs)
            _inputs.EnableControls();
    }

    public void ResetGravityScale() => _rigidBody2D.gravityScale = _defaultGravityScale;
}
