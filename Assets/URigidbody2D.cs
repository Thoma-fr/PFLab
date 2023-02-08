using UnityEngine;

// "Useful" Rigidbody2D. Adds some useful physics stuff.
public class URigidbody2D : MonoBehaviour
{
    private Rigidbody2D rb;
    public Vector2 LastFrameVelocity { get; private set; }
    public Vector2 velocity { get { return rb.velocity; } set { rb.velocity = value; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        LastFrameVelocity = rb.velocity;
    }
}
