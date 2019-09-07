using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ScreenEffects : MonoBehaviour
{

    public static ScreenEffects inst;
    Bloom bloomLayer = null;
    PostProcessVolume volume;
    Camera mainCamera;
    Color cameraBackColor;

    private void Awake()
    {
        inst = this;
    }
    void Start()
    {

        mainCamera = Camera.main;
        cameraBackColor = mainCamera.backgroundColor;
        PostProcessVolume volume = mainCamera.GetComponentInChildren<PostProcessVolume>();
        volume.profile.TryGetSettings(out bloomLayer);

        
    }


    bool isFlashing;
    public void flashScreen()
    {
        isFlashing = true;
    }
    // Update is called once per frame
    void Update()
    {



        if (isFlashing)
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, new Color(0.88f, 1f, 0.14f), .35f);

            bloomLayer.intensity.value = Mathf.MoveTowards(bloomLayer.intensity.value, 50f, Time.deltaTime * 150f);
            if (bloomLayer.intensity.value >= 50f)
                isFlashing = false;
        }
        else
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, cameraBackColor, .8f);

            bloomLayer.intensity.value = Mathf.MoveTowards(bloomLayer.intensity.value, 14f, Time.deltaTime * 40f);
        }
    }
}
