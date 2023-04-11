using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserDetector : MonoBehaviour, ILaserable
{
    public enum Type
    {
        GateOpener,
        PlateformeMover,

    }
    public GameObject reactiveGO;
    public Type type;
    public float speed;
    public bool isActivate;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void LaserReaction()
    {

        isActivate = true;
        switch(type)
        {
            case Type.GateOpener:
                reactiveGO.SetActive(false);
                break;
            case Type.PlateformeMover:
                reactiveGO.GetComponent<ActivateMovingPlateform>().canMove=true;
                break;
        }
    }
    public void LaserStop()
    {

        isActivate = false;
        switch (type)
        {
            case Type.GateOpener:
                reactiveGO.SetActive(true);
                break;
            case Type.PlateformeMover:
                reactiveGO.GetComponent<ActivateMovingPlateform>().canMove = false;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(isActivate)
            transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
