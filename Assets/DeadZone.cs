using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
public class DeadZone : MonoBehaviour
{
    public Transform respawn;
    public Volume volume;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.transform.position = respawn.position;
            volume.weight = 1;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => volume.weight, x => volume.weight = x, 1, 0.3f));
            sequence.Append(DOTween.To(() => volume.weight, x => volume.weight = x, 0, 0.3f));

        }
    }

}
