using UnityEngine;

// "Useful" Rigidbody2D. Adds some useful physics stuff.
public class URigidbody2D : MonoBehaviour
{
    private Rigidbody2D rb;
    public Vector2 LastFrameVelocity { get; private set; }
    public Rigidbody2D Rb => rb;

    //=========================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        LastFrameVelocity = rb.velocity;
    }
}
