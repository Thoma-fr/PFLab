using UnityEngine;

public class LaserMovingActionner : MonoBehaviour, ILaserable
{
    public ActivateMovingPlateform plateform;

    public void LaserReaction()
    {
        plateform.canMove = true;
        transform.rotation *= Quaternion.Euler(0, 0, Time.deltaTime * 100);
    }
    public void LaserStop()
    {
        plateform.canMove = false;
    }
}
