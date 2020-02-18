using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    private string currentScene;
    private int score, maxCombo;
    public int level;
    public bool isLevelingUp, isGameStarted, isInProtection, isPlayerDied;
    private int bodySlotPerLevel;
    public GameObject bodyBreakEffect, eatEffect, hitGroundEffect;
    public float timeWhenLevelUp;
    private bool playedBeforeInCurrentSession = false;
    public int requiredEatCountForNextLevel;
    public int totalEatedCount = 0;
    private void Awake()
    {
        
        inst = this;
        //GameAnalytics.Initialize();
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

    void Start()
    {
        Application.targetFrameRate = 100;
        bodySlotPerLevel = 5;
        StartCoroutine(loadGame());
    }

    IEnumerator loadGame()
    {

        AudioManager.inst.playMusic(EnumsData.MusicEnum.mainManuMusic);
        yield return null;
        UIManager.inst.onLoadGame();
        yield return null;
        PlayerTrail.inst.onLoadGame();
        yield return null; 
        ScreenEffects.inst.onLoadGame();
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
        TinySauce.OnGameStarted();
 
        if (playedBeforeInCurrentSession)
        {
            isPlayerDied = false;
            isLevelingUp = true;
            //2.Move the Planet toward behind of the camera 
            Planet.inst.moveActivePlanetBehindCamera();
            //3. Move the next Planet to the default location 
            Planet.inst.spawnNextPlanet();

            Invoke("attachPlayerTrailToActivePlanet", 1.8f);
            Invoke("endLevelingUp", 1.9f);

      
    
        }

        EatableManager.inst.onGameStart();
        isGameStarted = true;
        StartCoroutine(prepareLevel());


        

        AudioManager.inst.playMusic(EnumsData.MusicEnum.inGameMusic);
        startPlayerProtection(1.8f);
        //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, Application.version, "game");

        if (playedBeforeInCurrentSession)
        {
            Planet.inst.onResetGame();
        }
    }

    public void resetGame(bool playAgain)
    {
   
        PlayerTrail.inst.whenPlayResetGame();

        totalEatedCount = 0;
        level = 0;
       

        score = 0;
        maxCombo = 0;
        isLevelingUp = false;
        isInProtection = false;
        UIManager.inst.setScore(score);
        playedBeforeInCurrentSession = true;
        if (playAgain)
        {
            startGame();
        }
   
    }

    public void increaseScore(int point = 1)
    {
        score += point;
        UIManager.inst.setScore(score);
    }

    public void levelUp()
    {
        startPlayerProtection(2.5f);
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
        var slotPerLevel = bodySlotPerLevel + getExtraFoodLevel();
        int slotCountUsed = PlayerTrail.inst.getSlotCount() + slotPerLevel;
      

        if (slotCountUsed > PlayerTrail.inst.bodyPices.Length)
        {
            slotCountUsed = PlayerTrail.inst.bodyPices.Length;
            requiredEatCountForNextLevel += 30;
        }
        else
        {
            requiredEatCountForNextLevel = slotCountUsed;
        }

        int spawnCount = requiredEatCountForNextLevel - totalEatedCount;
        if (level == 0)
        {
            totalEatedCount = PlayerTrail.inst.getBaseBodyCount();
            spawnCount = requiredEatCountForNextLevel - totalEatedCount;
        }

  

        EatableManager.inst.spawnEatable(spawnCount);

        PlayerTrail.inst.setSlotCount(slotCountUsed) ;


        PlayerTrail.inst.prepareTrailForLevel();


        //ScreenEffects.inst.flashScreen(new Color(.92f, .48f, .68f), 20f);
        ScreenEffects.inst.setVignette(.55f);
        yield return new WaitForSeconds(.2f);
  
        AudioManager.inst.playSFX(EnumsData.SFXEnum.levelUp);

        yield return new WaitForSeconds(1.75f);
        ScreenEffects.inst.setVignette(0.494f);
        yield return null;
       
    }
 
    public void updateMaxCombo(int currentCombo)
    {
        if (currentCombo > maxCombo)
        {
            maxCombo = currentCombo;
        }
    }
    public void death()
    {
        TinySauce.OnGameFinished(score);
        //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, Application.version, "game", score);
        if (!isGameStarted)
            return;
        StartCoroutine(saveMatchInfoCorot());

        isPlayerDied = true;
        PlayerTrail.inst.whenPlayerDie();
        UIManager.inst.onGameOver();

        AudioManager.inst.playMusic(EnumsData.MusicEnum.gameOverMusic);
    }

    IEnumerator saveMatchInfoCorot()
    {
        
        var newScore = score;
        var newMaxCombo = maxCombo;
        var newLevel = level;
        GameServices.ReportScore(newScore, EM_GameServicesConstants.Leaderboard_HighScore);
        UIManager.inst.setGameOverScreenInfo(newScore, newMaxCombo, newLevel);

        var oldScore = PlayerPrefs.GetInt("topScore", 0);

        var oldMaxCombo = PlayerPrefs.GetInt("topCombo", 0);

        var oldLevel = PlayerPrefs.GetInt("topLevel", 0);

       
        if (newScore > oldScore)
            PlayerPrefs.SetInt("topScore", newScore);

        if (newMaxCombo > oldMaxCombo)
            PlayerPrefs.SetInt("topCombo", newMaxCombo);

        if (newLevel > oldLevel)
            PlayerPrefs.SetInt("topLevel", newLevel);

        
        yield return null;
    }

}
