using UnityEngine;

// "Useful" Rigidbody2D. Adds some useful physics stuff.
public class URigidbody2D : MonoBehaviour
{
    private Rigidbody2D _rigidBody2D;
    public Vector2 LastFrameVelocity { get; private set; }
    public Rigidbody2D RigidBody2D => _rigidBody2D;

    //=========================================================

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        LastFrameVelocity = _rigidBody2D.velocity;
    }
}
