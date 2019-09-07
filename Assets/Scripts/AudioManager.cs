using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsData;

public class AudioManager : MonoBehaviour
{

    public static AudioManager inst;
    public AudioSource eatSFX;

    private void Awake()
    {
        inst = this;
    }

    public void playSFX(SFXEnum sfx)
    {
       
        if (sfx == SFXEnum.eating)
            eatSFX.Play();
    }
}
