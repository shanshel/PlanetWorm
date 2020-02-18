using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ScreenEffects : MonoBehaviour
{

    public static ScreenEffects inst;
    Bloom bloomLayer = null;
    PostProcessVolume volume;
    Vignette _vignette;
    public Camera mainCamera;
    Color cameraBackColor;
    float baseBloomValue; 
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
    

    public void onLoadGame()
    {

        cameraBackColor = mainCamera.backgroundColor;
        PostProcessVolume volume = mainCamera.GetComponentInChildren<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomLayer);
        //volume.profile.TryGetSettings(out _vignette);
        baseBloomValue = bloomLayer.intensity.value;
    }

    Color flashColor;
    float flashMoveTowardSpeed;
    float flashBackgroundLerpTime;

    bool isFlashing;

    public void flashScreen(Color color, float _speed, float _lerp = 1f)
    {
        isFlashing = false;
        mainCamera.backgroundColor = cameraBackColor;
        bloomLayer.intensity.value = baseBloomValue;
        flashColor = color;
        flashMoveTowardSpeed = _speed;
        flashBackgroundLerpTime = _lerp;
        isFlashing = true;
    }

    public void setVignette(float intinsity = .6f)
    {
       // _vignette.intensity.value = intinsity;
    }


    // Update is called once per frame
    void Update()
    {

        if (!GameManager.inst.isGameStarted)
            return;

        if (isFlashing)
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, flashColor, flashBackgroundLerpTime);

            bloomLayer.intensity.value = Mathf.MoveTowards(bloomLayer.intensity.value, 12f, Time.deltaTime * flashMoveTowardSpeed);
            if (bloomLayer.intensity.value >= 12f)
                isFlashing = false;
        }
        else
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, cameraBackColor, .8f);

            bloomLayer.intensity.value = Mathf.MoveTowards(bloomLayer.intensity.value, baseBloomValue, Time.deltaTime * 40f);
        }

    }
}
