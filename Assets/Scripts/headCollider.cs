using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class headCollider : MonoBehaviour
{



    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Food")
        {
            other.gameObject.SetActive(false);
            var parentTransform = other.gameObject.GetComponentInParent<Transform>();

            StartCoroutine(eatEffect(parentTransform));

            PlayerTrail.inst.eatBall(parentTransform);
        }
       
        
    }

    IEnumerator eatEffect(Transform parentTransform)
    {
        yield return parentTransform.DOShakeScale(.6f, 1.5f, 5, 30f).WaitForCompletion();
        yield return parentTransform.DOScale(0f, .2f).WaitForCompletion();

        Destroy(parentTransform.gameObject);
    }



}
