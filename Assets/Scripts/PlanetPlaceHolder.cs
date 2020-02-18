using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static EnumsData;

public class PlanetPlaceHolder : MonoBehaviour
{
    Sequence twSeq;
    private Vector3 outterPos, closePos, farPos;

    private void Awake()
    {
        outterPos = new Vector3(24f, 40f, 32f);
        closePos = new Vector3(2f, 9f, 8f);
        farPos = new Vector3(6f, 20f, 16f);
    }

    private void Start()
    {
        startPlaceHolderAnimation();
    }

    public void startPlaceHolderAnimation()
    {
        twSeq = DOTween.Sequence().SetLoops(-1);
        twSeq.Append(transform.DOShakePosition(6f, 1.2f, 2, 40f, false, false));
    }


    public void setPos(PlaceHolderPos _pos)
    {
        twSeq.Kill();
        if (_pos == PlaceHolderPos.close)
            transform.position = closePos;
        else if (_pos == PlaceHolderPos.outter)
            transform.position = outterPos;
        else if (_pos == PlaceHolderPos.far)
            transform.position = farPos;
    }

    public void moveToward(PlaceHolderPos _pos)
    {
        var target = closePos;
        if (_pos == PlaceHolderPos.close)
            target = closePos;
        else if (_pos == PlaceHolderPos.outter)
            target = outterPos;
        else if (_pos == PlaceHolderPos.far)
            target = farPos;

        transform.DOMove(target, 2f);
        Invoke("startPlaceHolderAnimation", 2f);
    }








}
