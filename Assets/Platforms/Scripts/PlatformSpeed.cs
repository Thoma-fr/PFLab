using UnityEngine;

public class PlatformSpeed : Platform
{
    [Header("Speed settings")]
    [SerializeField, Tooltip("How much is the object's speed going to be multiplied ? 1 = no change")]
    private float _speedScale;

    //=======================================================

    /// <summary> Changes the object's speed using _speedScale. </summary>
    public void ChangeSpeed(Rigidbody2D rb)
    {
        // TODO
    }
}
