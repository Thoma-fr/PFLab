using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserDetector : MonoBehaviour, IlaserAble
{
    public GameObject gateToOpen;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void LaserReaction()
    {
        gateToOpen.SetActive(false);
    }
    public void LaserStop()
    {
        gateToOpen.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
