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
    public void Bounce(Rigidbody2D rb)
    {
        // TODO
    }

    /// <summary> The platform's bounce animation. </summary>
    private IEnumerator BounceAnimation()
    {
        // TODO
        yield break;
    }
}
