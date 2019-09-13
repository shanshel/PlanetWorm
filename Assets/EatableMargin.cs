using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsData;
using DG.Tweening;
using TMPro;

public class EatableMargin : MonoBehaviour
{
    
    [SerializeField]
    private GameObject parent;
    public EatableType type;
    public SphereCollider _spCollider;
    public TextMeshPro pointText;

    private Vector3 baseScale;
    public int eatPoint = 1;
    private void Start()
    {
        
        baseScale = parent.transform.localScale;
        parent.transform.localScale.Set(0f, 0f, 0f);
        parent.transform.DOScale(baseScale.x, .8f);
        Invoke("lateStart", .8f);

        pointText.text = "+" + eatPoint;
        if (type == EatableType.danger)
        {
            eatPoint = Random.Range(-7, -2);
            pointText.text = eatPoint.ToString();
        }


    }

    private void lateStart()
    {
        _spCollider.enabled = true;
    }
    private void Update()
    {
        parent.transform.LookAt(Vector3.up, Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.inst.isGameStarted)
            return;

        if (other.name == "BigHeadCollider")
        {
            GetComponent<SphereCollider>().enabled = false;
            Instantiate(GameManager.inst.eatEffect, parent.transform.position, Quaternion.identity, Planet.inst.currentPlanetContainer.transform);

            StartCoroutine(eatEffect());
            EatableManager.inst.removeFromSpawnedList(parent.name);

            if (type == EatableType.better)
            {
                EatableManager.inst.setExtraExpire();
                PlayerTrail.inst.eatBall(parent.transform, eatPoint, false);
            }
            else  if (type == EatableType.danger)
            {
                PlayerTrail.inst.eatBall(parent.transform, eatPoint, false);
            }
            else
            {
                PlayerTrail.inst.eatBall(parent.transform, eatPoint);
            }
        }
    }

    IEnumerator eatEffect()
    {
        yield return parent.transform.DOShakeScale(.3f, 1.5f, 5, 30f).WaitForCompletion();
        yield return parent.transform.DOScale(0f, .2f).WaitForCompletion();

        Debug.Log("reached here:: " + parent.name);
        Destroy(parent.gameObject, 2f);
    }

}
