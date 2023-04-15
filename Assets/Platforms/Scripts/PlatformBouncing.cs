using System.Collections;
using UnityEngine;

public class PlatformBouncing : Platform
{
    [Header("Bounce settings")]
    [SerializeField, Tooltip("Minimum force of the bounce (exemple : if the player simply walks on the platform)")]
    private float _minBounceForce;
    [SerializeField, Tooltip("Maximum force of the bounce")]
    private float _maxBounceForce;
    [SerializeField, Tooltip("How much of the objects own force do we reflect back ?")]
    private float _bounceForceScale;
    [SerializeField, Tooltip("Duration of the bounce animation.")]
    private float _bounceAnimationDuration;

    //===========================================================

    /// <summary> Bounces the given RigidBody2D using its velocity and the platform's rotation. </summary>
    public void Bounce(URigidbody2D urb)
    {
        // Reflect the object's velocity for accurate bounce direction.
        Vector2 reflect = Vector2.Reflect(urb.LastFrameVelocity, transform.up.normalized);

        // We check newVelocity to make sure it is within limits.
        Vector2 newVelocity = reflect * _bounceForceScale;
        if(newVelocity.sqrMagnitude > _maxBounceForce * _maxBounceForce)
        {
            newVelocity = newVelocity.normalized * _maxBounceForce;
        }
        else if(newVelocity.sqrMagnitude < _minBounceForce * _minBounceForce)
        {
            newVelocity = newVelocity.normalized * _minBounceForce;
        }

        urb.RigidBody2D.velocity = newVelocity;
    }

    /// <summary> The platform's bounce animation. </summary>
    private IEnumerator BounceAnimation()
    {
        // TODO
        yield break;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out URigidbody2D urb))
            return;


        // Ignore if the object comes from the back
        if (transform.InverseTransformPoint(collision.transform.position).y <= 0)
            return;

        Bounce(urb);
    }
}
