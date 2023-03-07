using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PFUIContainer : MonoBehaviour
{
    public List<GameObject> pfChild = new List<GameObject>();
    //public int id;
    public void AnimeUI()
    {
        StartCoroutine(AnimeCour());
    }

    private IEnumerator AnimeCour()
    {
        foreach (GameObject go in pfChild)
        {
            
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(go.transform.DOScale(new Vector3(0,0,0),0));
            mySequence.Append(go.transform.DOScale(new Vector3(0.65f, 0.65f, 0.65f), 0.05f).SetEase(Ease.OutBounce));
            yield return new WaitForSeconds(0.01f);
        }
    }
    private void OnDisable()
    {
        foreach (GameObject go in pfChild)
        {
            go.transform.localScale=new Vector3(0,0,0);
        }
    }
}
