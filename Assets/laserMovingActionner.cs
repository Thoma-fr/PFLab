using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserMovingActionner : MonoBehaviour, IlaserAble
{
    public ActivateMovingPlateform plateform;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void LaserReaction()
    {
        plateform.canMove = true;
    }
    public void LaserStop()
    {
        plateform.canMove = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
