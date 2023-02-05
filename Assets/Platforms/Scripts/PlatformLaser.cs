using UnityEngine;

public class PlatformLaser : Platform
{
    [Header("Laser settings")]
    [SerializeField, Tooltip("How much damage the laser deals per second.")]
    private float _damagePerSecond;

    //=========================================================

    /// <summary> Shoot a laser towards the direction the platform is facing. </summary>
    protected void ShootLaser()
    {
        // TODO
    }
}
