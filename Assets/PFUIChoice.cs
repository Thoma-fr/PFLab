using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PFUIChoice : MonoBehaviour,IPointerEnterHandler
{
    [SerializeField] private int id;


    public void OnPointerEnter(PointerEventData eventData)
    {
        Player2DController.instance.SelectPF(id);
        Debug.Log("mouse enter");
    }
}
