using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumsData;

public class AudioManager : MonoBehaviour
{

    public static AudioManager inst;
    public AudioSource eatSFX, levelUpSFX, levelUp2SFX;

    private void Awake()
    {
        inst = this;
    }

    public void playSFX(SFXEnum sfx)
    {

        if (sfx == SFXEnum.eating)
            eatSFX.Play();
        else if (sfx == SFXEnum.levelUp)
        {
            levelUpSFX.Play();
        }
        else if (sfx == SFXEnum.levelUp2)
        {
            levelUp2SFX.Play();
        }
            
        
    }

    void playLevelUpSFX()
    {
        levelUpSFX.Play();

    }
}
