using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFBack : MonoBehaviour
{

    private Vector3 playerTransfome;
    public float speed;

    private void OnEnable()
    {
       
    }
    void Update()
    {
        playerTransfome = MovementController.instance.gameObject.transform.position;
        transform.position = Vector3.Lerp(transform.position, playerTransfome, speed*Time.deltaTime);
        if(Vector3.Distance(playerTransfome,transform.position)<1)
            Destroy(gameObject,0.2f);
    }
}
