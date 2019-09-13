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
    public float timeWhenLevelUp;
    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
        bodySlotPerLevel = 5;
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
        EatableManager.inst.onGameStart();
        isGameStarted = true;
        StartCoroutine(prepareLevel());


        

        AudioManager.inst.playMusic(EnumsData.MusicEnum.inGameMusic);
        startPlayerProtection();
    }

    public void resetGame(bool playAgain)
    {
   
        PlayerTrail.inst.whenPlayResetGame();

        level = 0;
        score = 0;
        isLevelingUp = false;
        isInProtection = false;

        if (playAgain)
        {
            isPlayerDied = false;
            isLevelingUp = true;
            
            //2.Move the Planet toward behind of the camera 
            Planet.inst.moveActivePlanetBehindCamera();
            //3. Move the next Planet to the default location 
            Planet.inst.spawnNextPlanet();

            Invoke("attachPlayerTrailToActivePlanet", 1.8f);
            Invoke("endLevelingUp", 1.9f);

            startGame();
            Planet.inst.onResetGame();
            
        }
    }

    public void increaseScore(int point = 1)
    {
        score += point;
        UIManager.inst.setScore(score);
    }

    public void levelUp()
    {
        startPlayerProtection(2f);
        level += 1;
        isLevelingUp = true;
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

    private int getExtraSlotLevel()
    {
        return (level + 1) * 2;
    }

    IEnumerator prepareLevel()
    {
        var eatableCount = bodySlotPerLevel + getExtraFoodLevel();
        Debug.Log("Level: " + level + " - Slot: " + (PlayerTrail.inst.getSlotCount() + eatableCount) + " - Eat: " + eatableCount);

        EatableManager.inst.spawnEatable(eatableCount);
        PlayerTrail.inst.setSlotCount( PlayerTrail.inst.getSlotCount() + eatableCount ) ;


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
        UIManager.inst.onGameOver();

        AudioManager.inst.playMusic(EnumsData.MusicEnum.gameOverMusic);
    }
}
