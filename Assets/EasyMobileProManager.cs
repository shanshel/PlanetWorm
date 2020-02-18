using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyMobileProManager : MonoBehaviour
{
    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
    }


}
