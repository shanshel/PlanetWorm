using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScoreEffects : MonoBehaviour
{
    public static ScoreEffects inst;
    [SerializeField]
    GameObject pointPrefab;

    private void Awake()
    {
        inst = this;
    }
    public void doPointEffect(Transform target)
    {
        var pointInst = Instantiate(pointPrefab, target.position, Quaternion.identity);
        StartCoroutine(runEffect(pointInst));
       
    }

    IEnumerator runEffect(GameObject pointInst)
    {
        pointInst.transform.DOMoveY(pointInst.transform.position.y + 1f, .5f);
        yield return pointInst.transform.DOScale(.2f, .5f).WaitForCompletion();
        pointInst.transform.DOMoveY(pointInst.transform.position.y + 1f, .4f);
        pointInst.transform.DOScale(0f, .4f).WaitForCompletion();
        GameManager.inst.increaseScore();
        Destroy(pointInst, .62f);

    }

    
}
