using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsData;
using DG.Tweening;
using TMPro;

public class EatableMargin : MonoBehaviour
{
    
    
    [SerializeField]
    private GameObject childFood;
    public EatableType type;
    public SphereCollider _spCollider;
    public TextMeshPro pointText;

    private Vector3 baseScale;
    public int eatPoint = 1;

    float timeToEnableSphere = .4f;
    bool isTimerDone = false;
    bool isAboutDelete = false;
    private void Start()
    {
        
        baseScale = transform.localScale;
        transform.localScale.Set(0f, 0f, 0f);
        transform.DOScale(baseScale.x, .8f);

        pointText.text = "+" + eatPoint;
        if (type == EatableType.danger)
        {
            eatPoint = Random.Range(-7, -2);
            pointText.text = eatPoint.ToString();
        }
    }

    private void Update()
    {
        timeToEnableSphere -= Time.deltaTime;
        if (!isTimerDone && timeToEnableSphere <= 0f)
        {
            isTimerDone = true;
            _spCollider.enabled = true;
        }
    }



    private void FixedUpdate()
    {
        transform.LookAt(Vector3.up, Vector3.up);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.inst.isGameStarted || GameManager.inst.isPlayerDied)
            return;

        if (isAboutDelete) return;
        if (other.name == "BigHeadCollider")
        {
            _spCollider.enabled = false;
            Instantiate(GameManager.inst.eatEffect, transform.position, Quaternion.identity, Planet.inst.currentPlanetContainer.transform);

            StartCoroutine(eatEffect());
            EatableManager.inst.removeFromSpawnedList(gameObject.name);

            if (type == EatableType.better)
            {
                EatableManager.inst.setExtraExpire();
                PlayerTrail.inst.eatBall(transform, eatPoint, false);
                Player.inst.onPlayerEat();
            }
            else  if (type == EatableType.danger)
            {
                PlayerTrail.inst.eatBall(transform, eatPoint, false);
                Player.inst.onPlayerEat();

            }
            else
            {
                PlayerTrail.inst.eatBall(transform, eatPoint);
                Player.inst.onPlayerEat();

            }
        }
    }

    IEnumerator eatEffect()
    {
        yield return transform.DOShakeScale(.3f, 1.5f, 5, 30f).WaitForCompletion();
        yield return transform.DOScale(0f, .2f).WaitForCompletion();

        Destroy(gameObject, 2f);
    }

    IEnumerator hideAndDeleteCorot()
    {
       
        yield return transform.DOScale(0f, .5f).WaitForCompletion();
        Destroy(this.gameObject, 2f);

    }
    public void hideAndDelete()
    {
        isAboutDelete = true;
        StartCoroutine(hideAndDeleteCorot());
    }

}
