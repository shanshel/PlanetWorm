using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EatableManager : MonoBehaviour
{
    public static EatableManager inst;
    [SerializeField]
    EatableMargin eatablePrefab, extraEatablePrefab, dangerEatablePrefab;
    [SerializeField]
    int countOfDangerEatable = 3;

    List<EatableMargin> eatableList = new List<EatableMargin>();
    float minDistanceEatableSpawn = .5f;
    int lastSpawnedLevel;
    bool isExtraGainedForThisLevel;
   
    private void Awake()
    {
        inst = this;
    }

    public void onGameStart()
    {
        CancelInvoke("invokeRespawnEatable");
        InvokeRepeating("invokeRespawnEatable", 10f, 10f);
    }


    void invokeRespawnEatable()
    {
        if (
            !GameManager.inst.isGameStarted ||
            GameManager.inst.isPlayerDied
            )
            return;

        if (
            lastSpawnedLevel == GameManager.inst.level
            )
        {
            var spawnCount = GameManager.inst.requiredEatCountForNextLevel - GameManager.inst.totalEatedCount;
    
            EatableManager.inst.spawnEatable(spawnCount, true);
        }

        lastSpawnedLevel = GameManager.inst.level;

    }



    public void spawnEatable(int count, bool respawn = false)
    {
        StartCoroutine(spawnEatableCorot(count, respawn));
    }

    
    IEnumerator spawnEatableCorot(int count, bool respawn = false)
    {
        var _eatableList = eatableList;
        if (respawn && _eatableList.Count > 0)
        {
            for (var x = 0; x < _eatableList.Count; x++)
            {
                eatableList[x]._spCollider.enabled = false;
                eatableList[x].hideAndDelete();
            }
            eatableList.Clear();
        }
        else
        {
            isExtraGainedForThisLevel = false;
            eatableList.Clear();
        }

        var currentPlanetCont = Planet.inst.currentPlanetContainer;
        var i = 0;
        int tryCount = 0;


        if (!isExtraGainedForThisLevel)
        {
            EatableMargin extraEatable;
            var rand = Random.onUnitSphere * .95f;
            Vector3 spawnPosition = rand * (.5f + 1.3f * 0.5f) + currentPlanetCont.transform.position;
            extraEatable = Instantiate(extraEatablePrefab, spawnPosition, Quaternion.identity, currentPlanetCont.transform);
            extraEatable.transform.LookAt(currentPlanetCont.transform.position);
            extraEatable.name = spawnPosition.ToString() + "extra";
            eatableList.Add(extraEatable);
        }

        for (var x = 0; x < countOfDangerEatable; x++)
        {
            EatableMargin dangerEatable;
            var rand = Random.onUnitSphere * .95f;
            Vector3 spawnPosition = rand * (.5f + 1.3f * 0.5f) + currentPlanetCont.transform.position;
            dangerEatable = Instantiate(dangerEatablePrefab, spawnPosition, Quaternion.identity, currentPlanetCont.transform);
            dangerEatable.transform.LookAt(currentPlanetCont.transform.position);
            dangerEatable.name = spawnPosition.ToString() + "danger";
            eatableList.Add(dangerEatable);
        }
  

        while (i < count)
        {
            var rand = Random.onUnitSphere * .95f;
            bool canSpawn = true;
            if (tryCount > 4)
            {

                minDistanceEatableSpawn -= .05f;
                if (minDistanceEatableSpawn <= 0f)
                {
                    minDistanceEatableSpawn = 0.1f;
                }
            }

            for (var x = 0; x < eatableList.Count; x++)
            {
                if (Vector3.Distance(rand, eatableList[x].transform.position) < minDistanceEatableSpawn)
                {
                    canSpawn = false;
                }
            }
            if (canSpawn)
            {
                tryCount = 0;
                
                
                Vector3 spawnPosition = rand * (.5f + 1.3f * 0.5f) + currentPlanetCont.transform.position;
                EatableMargin eatable;
          

                eatable = Instantiate(eatablePrefab, spawnPosition, Quaternion.identity, currentPlanetCont.transform);
                eatable.transform.LookAt(currentPlanetCont.transform.position);
                eatable.name = spawnPosition.ToString();
                eatableList.Add(eatable);
                i++;
            }
            else
            {
                tryCount++;
            }
        }

        yield return null;
    }


    public void removeFromSpawnedList(string name)
    {
        for (var x = 0; x < eatableList.Count; x++)
        {
            if (eatableList[x].name == name)
            {
                eatableList.RemoveAt(x);
            }
        }
        
    }


    public void setExtraExpire()
    {
        isExtraGainedForThisLevel = true;
    }

}
