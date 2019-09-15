using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerTrail : MonoBehaviour
{

    public static PlayerTrail inst;
    [SerializeField]
    private PlayerBodyPiece bodyPiece;
    [SerializeField]
    private GameObject slotPiece;
    [SerializeField]
    private int maxBodyPieceCount, maxSlotCount;
    private int baseMaxBodyPieceCount, baseMaxSlotCount, trustedSlotCount;
    private bool isReachedMax;
    public List<PlayerBodyPiece> bodyPices = new List<PlayerBodyPiece>();
    public List<GameObject> bodySlots = new List<GameObject>();

    private Transform playerPos;
    private GameObject playerHeadObject;

    private Transform defaultBodySlotScale, defaultBodyPartScale;
    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        baseMaxBodyPieceCount = maxBodyPieceCount;
        baseMaxSlotCount = maxSlotCount;
        trustedSlotCount = baseMaxBodyPieceCount;

        generateObjectsLoop();
        StartCoroutine(coRotUpdate());
    }

    // Update is called once per frame
    float flashColor = 0f;

    int speedLevel = 0;
    public void setSpeedLevel(int _speedLevel)
    {
        speedLevel = _speedLevel;
    }

    float minDistance = .012f;
    float commonSpeed = 1f;
    IEnumerator coRotUpdate()
    {

        while (true)
        {
            
            if (Planet.inst.currentPlanetContainer == null)
            {
                yield return null;
                continue;
            }
               
            var playerTrans = Player.inst.getPlayerHeadTrans();
  
            for (var x = 0; x <= maxBodyPieceCount; x++)
            {
                if (Planet.inst.currentPlanetContainer == null)
                {
                    continue;
                }
                if (x == 0)
                {
                    float distanceToHead = Vector3.Distance(playerHeadObject.transform.position, playerTrans.position);

                    float headTimeCal = Time.smoothDeltaTime * distanceToHead / minDistance * commonSpeed;

                    if (headTimeCal > .5f)
                    {
                        headTimeCal = .5f;
                    }
                    playerHeadObject.transform.position = Vector3.Slerp(playerHeadObject.transform.position, playerTrans.position, headTimeCal);
                    playerHeadObject.transform.LookAt(playerTrans.position, Vector3.forward);
                }

                var currentBodyPart = bodyPices[x].transform;
                var prevBodyPart = playerHeadObject.transform;
                if (x > 0)
                {
                    prevBodyPart = bodyPices[x - 1].transform;
                }
       
                float distanceToTarget = Vector3.Distance(currentBodyPart.position, prevBodyPart.position);
                float timeCal = Time.smoothDeltaTime * distanceToTarget / minDistance * commonSpeed;

                if (timeCal > .5f)
                {
                    timeCal = .5f;
                }

                currentBodyPart.position = Vector3.Slerp(currentBodyPart.position, prevBodyPart.position, timeCal);
                currentBodyPart.rotation = Quaternion.Slerp(currentBodyPart.rotation, prevBodyPart.rotation, timeCal);

            }



            for (var y = 0; y <= maxSlotCount; y++)
            {
                if (Planet.inst.currentPlanetContainer == null)
                {
                    continue;
                }
                var currentBodyPart = bodySlots[y].transform;
                var prevBodyPart = playerHeadObject.transform;

                if (y > 0)
                {
                    prevBodyPart = bodySlots[y - 1].transform;
                }

                float distanceToTarget = Vector3.Distance(currentBodyPart.position, prevBodyPart.position);
                float timeCal = Time.smoothDeltaTime * distanceToTarget / minDistance * commonSpeed;
                if (timeCal > .5f)
                {
                    timeCal = .5f;
                }

                currentBodyPart.position = Vector3.Slerp(currentBodyPart.position, prevBodyPart.position, timeCal);
                currentBodyPart.rotation = Quaternion.Slerp(currentBodyPart.rotation, prevBodyPart.rotation, timeCal);
                //bodySlots[y].transform.LookAt(Vector3.up);
        
            }

            yield return null;
        }

       
       
    }

    public void whenPlayerDie()
    {
        StartCoroutine(playerDieCo());
    }

    public void whenPlayResetGame()
    {
        maxBodyPieceCount = baseMaxBodyPieceCount;
        maxSlotCount = baseMaxSlotCount;
        trustedSlotCount = baseMaxBodyPieceCount;

        playerHeadObject.SetActive(true);
        for (var x = 0; x <= maxBodyPieceCount; x++)
        {
            bodyPices[x].transform.position = defaultBodyPartScale.position;
            bodyPices[x].transform.localScale = defaultBodyPartScale.localScale;
        }


        for (var y = 0; y <= maxSlotCount; y++)
        {
            bodySlots[y].transform.position = defaultBodySlotScale.position;
            bodySlots[y].transform.localScale = defaultBodySlotScale.localScale;
        }

        updateObjectsLoop();
        
    }

    IEnumerator playerDieCo()
    {
        AudioManager.inst.playSFX(EnumsData.SFXEnum.fail);

        playerHeadObject.SetActive(false);
        Instantiate(GameManager.inst.bodyBreakEffect, playerHeadObject.transform.position, Quaternion.identity, Planet.inst.currentPlanetContainer.transform);

        yield return new WaitForSeconds(.05f);


        for (var x = 0; x <= maxBodyPieceCount; x++)
        {
            if (!GameManager.inst.isPlayerDied)
                break;
            bodyPices[x].transform.DOScale(0f, .3f);
            AudioManager.inst.playSFX(EnumsData.SFXEnum.bodyBreak);
            Instantiate(GameManager.inst.bodyBreakEffect, bodyPices[x].transform.position, Quaternion.identity, Planet.inst.currentPlanetContainer.transform);

            yield return new WaitForSeconds(.12f);
        }


        for (var y = 0; y <= maxSlotCount; y++)
        {
            if (!GameManager.inst.isPlayerDied)
                break;
            if (y >= maxBodyPieceCount)
            {
                bodySlots[y].transform.DOScale(0f, .3f);
                AudioManager.inst.playSFX(EnumsData.SFXEnum.bodyBreak);
                Instantiate(GameManager.inst.bodyBreakEffect, bodySlots[y].transform.position, Quaternion.identity, Planet.inst.currentPlanetContainer.transform);
                yield return new WaitForSeconds(.12f);
            }
        }
    }

    Vector3 savedPlayerPosition;

    

 
    void generateObjectsLoop()
    {
        playerHeadObject = Instantiate(Player.inst.getPlayerHeadModel(), transform.position, Quaternion.identity, Planet.inst.getTrailContainerTransform());


        for (var x = 0; x < 1000; x++)
        {
            var gobject = Instantiate(bodyPiece, transform.position, Quaternion.identity, Planet.inst.getTrailContainerTransform());

            if (x < maxBodyPieceCount)
            {
                gobject.gameObject.SetActive(true);
            }

            bodyPices.Add(gobject);




            var bodySlotObject = Instantiate(slotPiece, transform.position, Quaternion.identity, Planet.inst.getTrailContainerTransform());

            if (x < maxSlotCount)
            {
                bodySlotObject.SetActive(true);
            }
            bodySlots.Add(bodySlotObject);

        }

        defaultBodySlotScale = bodySlots[0].transform;
        defaultBodyPartScale = bodyPices[0].transform;


        updateObjectsLoop();
    }

    void updateObjectsLoop(bool eating = false)
    {
        for (var x = 0; x < bodyPices.Count; x++)
        {
            if (x < 3)
            {
                bodyPices[x].toggleCollider(false);
            }
            if (x < maxBodyPieceCount)
            {
                if (eating && (x+1 == maxBodyPieceCount))
                {
                    bodyPices[x].gameObject.SetActive(true);
                    bodyPices[x].moveTowardScale(x, maxBodyPieceCount - 1);
                }
                else
                {
                    bodyPices[x].gameObject.SetActive(true);
                    bodyPices[x].setScale(x, maxBodyPieceCount - 1);
                }
    
            }
        }


        for (var x = 0; x < bodySlots.Count; x++)
        {
            if (x < maxSlotCount)
            {
                if (x >= maxBodyPieceCount)
                {
                    bodySlots[x].gameObject.SetActive(true);
                }
                else
                {
                    bodySlots[x].gameObject.SetActive(false);
                }

            }
        }
    
    }

    float lastEat;
    float comboWaitLength = 3f;
    int comboCount;
    int maxComboCount = 100;

    public void eatBall(Transform target, int pointCount, bool addBodyPart = true)
    {
        if (isReachedMax) return;
        if (lastEat > 0f && lastEat + comboWaitLength >= Time.time)
        {
            comboCount++;
            if (comboCount > maxComboCount)
                comboCount = maxComboCount;

        }
        else
        {
            comboCount = 0;
        }

        


        lastEat = Time.time;
   

        if (pointCount < 0)
        {
            comboCount = 0;
            lastEat = 0f;
            ScreenEffects.inst.flashScreen(new Color(0.7f, .25f, .21f), 150f, .35f);
            AudioManager.inst.playDangerEatSFX();
        }
        else
        {
            ScreenEffects.inst.flashScreen(new Color(0.88f, 1f, 0.14f), 150f, .35f);

            AudioManager.inst.playEatSFX(comboCount);

        }

        GameManager.inst.updateMaxCombo(comboCount);
        ScoreEffects.inst.doPointEffect(target, comboCount, pointCount);

        if (!addBodyPart)
            return;
      
  
        maxBodyPieceCount += 1;

   
        updateObjectsLoop(true);
        if (maxBodyPieceCount == maxSlotCount)
        {
            isReachedMax = true;
            GameManager.inst.levelUp();
        }
   
    }


    public void setSlotCount(int count)
    {
        maxSlotCount = count;
        trustedSlotCount = count;
    }

    public int getSlotCount()
    {
        return trustedSlotCount;
    }

    public int getCurrentBodyCount()
    {
        return maxBodyPieceCount;
    }

    public void prepareTrailForLevel()
    {
        isReachedMax = false;
        updateObjectsLoop();
    }


    public int getBodyCount()
    {
        return bodyPices.Count;
    }
}
