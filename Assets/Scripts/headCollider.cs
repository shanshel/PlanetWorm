using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class headCollider : MonoBehaviour
{



    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.inst.isGameStarted)
            return;

        if (other.name == "Food")
        {
            other.gameObject.SetActive(false);
            var parentTransform = other.gameObject.GetComponentInParent<Transform>();
            Instantiate(GameManager.inst.eatEffect, parentTransform.position, Quaternion.identity, Planet.inst.currentPlanetContainer.transform);

            StartCoroutine(eatEffect(parentTransform));

            PlayerTrail.inst.eatBall(parentTransform);
        }
       
        
    }

    IEnumerator eatEffect(Transform parentTransform)
    {
        yield return parentTransform.DOShakeScale(.3f, 1.5f, 5, 30f).WaitForCompletion();

        yield return parentTransform.DOScale(0f, .2f).WaitForCompletion();

        Destroy(parentTransform.gameObject);
    }



}
