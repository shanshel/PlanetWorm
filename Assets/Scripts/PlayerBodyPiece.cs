using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerBodyPiece : MonoBehaviour
{
    [SerializeField]
    float gettingSmallerSpeed, timeOffset;

    [SerializeField]
    SphereCollider _shpereCollider;

    float currentTimer;

    Vector3 baseScale;
    bool isShrinking;
    // Start is called before the first frame update

    private void Awake()
    {
        baseScale = new Vector3(.8f, .8f, .8f) * 1.4f;

    }

    private float getDeltaScale(int index, int maxIndex)
    {
        var deltaScale = (index * -.04f);

        if (maxIndex > 40)
        {
            deltaScale = index * -.02f;
        }

        if (deltaScale < -.3f)
            deltaScale = -.3f;

        return deltaScale;
    }
    public void moveTowardScale(int index, int maxIndex)
    {
        var deltaScale = getDeltaScale(index, maxIndex);
        transform.localScale = Vector3.zero;
        transform.DOScale(new Vector3(baseScale.x + deltaScale, baseScale.y + deltaScale, baseScale.z + deltaScale), .7f);
    }
    public void setScale(int index, int maxIndex)
    {
        transform.localScale = baseScale;
        var deltaScale = getDeltaScale(index, maxIndex);
        transform.localScale = new Vector3(baseScale.x + deltaScale, baseScale.y + deltaScale, baseScale.z + deltaScale);
    }

    public void enableGettingSmaller()
    {
        isShrinking = true;
    }

    public void resetTransform()
    {
        isShrinking = false;
        currentTimer = 0f;
        transform.localScale = baseScale;
    }

    public void toggleCollider(bool enabled)
    {
        _shpereCollider.enabled = enabled;
    }
}
