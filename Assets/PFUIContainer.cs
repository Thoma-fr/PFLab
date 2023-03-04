using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PFUIContainer : MonoBehaviour
{
    public List<GameObject> pfChild = new List<GameObject>();
    
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
            mySequence.Append(go.transform.DOScale(new Vector3(1, 1, 1), 1f));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
