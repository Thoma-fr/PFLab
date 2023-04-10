using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFBack : MonoBehaviour
{

    public Vector3 playerTransfome;
    public float speed;
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, playerTransfome, speed*Time.deltaTime);
        if(Vector3.Distance(playerTransfome,transform.position)<1)
            Destroy(gameObject,2f);
    }
}
