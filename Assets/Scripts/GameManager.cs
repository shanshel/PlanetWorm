using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    private string currentScene;
    private int score;
    public int level;
    public bool isLevelingUp, isGameStarted, isInProtection, isPlayerDied;
    private int bodySlotPerLevel;
    public GameObject bodyBreakEffect, eatEffect, hitGroundEffect;
    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
        currentScene = "MainMenu";
        bodySlotPerLevel = 5;
        Planet.inst.spawnEatable(bodySlotPerLevel + getExtraFoodLevel());
        AudioManager.inst.playMusic(EnumsData.MusicEnum.mainManuMusic);

    }


    void Update()
    {
        
    }

  
    public void startPlayerProtection(float protectionTime = .6f)
    {
        isInProtection = true;
        Invoke("stopPlayerProtection", protectionTime);


    }

    public void stopPlayerProtection()
    {
        isInProtection = false;

    }
    public void startGame()
    {
        currentScene = "InGame";
        isGameStarted = true;
        AudioManager.inst.playMusic(EnumsData.MusicEnum.inGameMusic);
        startPlayerProtection();


    }

    public void increaseScore()
    {
        score += 1;
        UIManager.inst.setScore(score);
    }

    public void levelUp()
    {
        startPlayerProtection(2f);
        level += 1;
        isLevelingUp = true;
        //1.Unattach the Trail from the Plant
        //Planet.inst.getTrailContainerTransform().SetParent(Planet.inst.transform);
        Player.inst.whileLevelingUp();
        //2.Move the Planet toward behind of the camera 
        Planet.inst.moveActivePlanetBehindCamera();
        //3. Move the next Planet to the default location 
        Planet.inst.spawnNextPlanet();
        StartCoroutine(prepareLevel());
        //4. attach the trail to the new planet
        //
        Invoke("attachPlayerTrailToActivePlanet", 1.8f);
        Invoke("endLevelingUp", 1.9f);

    }

    private void attachPlayerTrailToActivePlanet()
    {
       Planet.inst.getTrailContainerTransform().SetParent(Planet.inst.getCurrentPlanetContainer().transform);
    }

    private void endLevelingUp()
    {
        isLevelingUp = false;
    }

    private int getExtraFoodLevel()
    {
        return level * 2;
    }

    IEnumerator prepareLevel()
    {
        Planet.inst.spawnEatable(bodySlotPerLevel + getExtraFoodLevel());
        PlayerTrail.inst.setSlotCount(PlayerTrail.inst.getSlotCount() + bodySlotPerLevel + getExtraFoodLevel());
        PlayerTrail.inst.prepareTrailForLevel();


        //ScreenEffects.inst.flashScreen(new Color(.92f, .48f, .68f), 20f);
        ScreenEffects.inst.setVignette(.55f);
        yield return new WaitForSeconds(.2f);
  
        AudioManager.inst.playSFX(EnumsData.SFXEnum.levelUp);

        yield return new WaitForSeconds(1.75f);
        ScreenEffects.inst.setVignette(0.494f);
        yield return null;
    }
 
    public void death()
    {
        isPlayerDied = true;
        PlayerTrail.inst.whenPlayerDie();
        AudioManager.inst.playMusic(EnumsData.MusicEnum.gameOverMusic);
    }
}
