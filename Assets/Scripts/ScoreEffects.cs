using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class ScoreEffects : MonoBehaviour
{
    public static ScoreEffects inst;

    public Point pointPrefab;

    private void Awake()
    {
        inst = this;
        return;
        if (inst != null && inst != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            inst = this;
        }
    }
    public void doPointEffect(Transform target, int comboCount, int pointCount)
    {
        Point pointInst = Instantiate(pointPrefab, target.position, Quaternion.identity);
        
        if (pointCount > 1)
        {
            pointInst.textMesh.color = new Color(0.09f, 0.25f, 0.85f);
        }

        if (comboCount > 0)
        {
            pointInst.comboTextMesh.text = "Combo x" + comboCount;
            pointInst.comboTextMesh.gameObject.SetActive(true);
            pointInst.textMesh.text = "+" + (pointCount + comboCount);
        }
        else
        {
            if (pointCount < 0)
                pointInst.textMesh.text = pointCount.ToString();
            else
                pointInst.textMesh.text = "+" + pointCount;

        }
        StartCoroutine(runEffect(pointInst, comboCount, pointCount));
       
    }

    IEnumerator runEffect(Point pointInst, int comboCount, int pointCount)
    {
        //pointInst.comboTextMesh.transform.DOShakePosition(.6f, 2f, 8, 30f);
        pointInst.comboTextMesh.transform.DOShakePosition(.6f, 2f, 8, 30f);
        pointInst.transform.DOMoveY(pointInst.transform.position.y + 1f, .5f);
        yield return pointInst.transform.DOScale(.2f, .5f).WaitForCompletion();
        pointInst.transform.DOMoveY(pointInst.transform.position.y + 1f, .4f);
        pointInst.transform.DOScale(0f, .4f).WaitForCompletion();
        GameManager.inst.increaseScore(pointCount + comboCount);
        Destroy(pointInst.gameObject, .62f);

    }

    
}
