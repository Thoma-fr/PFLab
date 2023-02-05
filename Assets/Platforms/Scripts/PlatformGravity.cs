using UnityEngine;

public class PlatformGravity : Platform
{
    [Header("Gravity settings")]
    [SerializeField, Tooltip("How much force this platform exerts on the object in relation to its gravity.")]
    private float _gravityScale;

    //=========================================================

    /// <summary> Moves the given rigidbody in the direction the platform is facing. </summary>
    public void MoveRigidBody(Rigidbody2D rb)
    {
        // TODO
    }
}
