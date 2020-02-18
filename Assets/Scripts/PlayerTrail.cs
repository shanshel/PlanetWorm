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
    private bool isReachedMax, isArrayReachedSafeIndex;

    public PlayerBodyPiece[] bodyPices;
    public GameObject[] bodySlots;

    private Transform playerPos;
    private GameObject playerHeadObject;

    private Transform defaultBodySlotScale, defaultBodyPartScale;
    private Transform playerTrans;

    
    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update

    public void onLoadGame()
    {

        bodyPices = new PlayerBodyPiece[140];
        bodySlots = new GameObject[140];


        generateObjectsLoop();
    }


    void Start()
    {
        baseMaxBodyPieceCount = maxBodyPieceCount;
        baseMaxSlotCount = maxSlotCount;
        trustedSlotCount = baseMaxBodyPieceCount;
    }

    private void FixedUpdate()
    {

        var planetLoc = Planet.inst.currentPlanetContainer.transform.position;
        if (
            Planet.inst.currentPlanetContainer == null ||
            !isArrayReachedSafeIndex
            )
        {
            return;
        }

        if (GameManager.inst.isLevelingUp)
            commonSpeed = .9f;
        playerTrans = Player.inst.getPlayerHeadTrans();


        for (var x = 0; x < maxBodyPieceCount; x++)
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
            if (Vector3.Distance(planetLoc, currentBodyPart.position) > 1.5f && !GameManager.inst.isLevelingUp)
            {
                Vector3 maturePoint;
                maturePoint = playerHeadObject.transform.position;
                if (x > 0)
                {
                    maturePoint = bodyPices[x - 1].transform.position;
                }

                currentBodyPart.position.Set(currentBodyPart.position.x, currentBodyPart.position.y, maturePoint.z);
            }
        }
 



        for (var y = 0; y < maxSlotCount; y++)
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

        for (var x = 0; x < bodyPices.Length; x++)
        {
            bodyPices[x].gameObject.SetActive(false);
        }

        for (var x = 0; x < bodySlots.Length; x++)
        {
            bodySlots[x].SetActive(false);
        }

        for (var x = 0; x <= maxBodyPieceCount; x++)
        {
            bodyPices[x].transform.position = defaultBodyPartScale.position;
            bodyPices[x].transform.localScale = defaultBodyPartScale.localScale;
            bodyPices[x].gameObject.SetActive(true);
        }


        for (var y = 0; y <= maxSlotCount; y++)
        {
            bodySlots[y].transform.position = defaultBodySlotScale.position;
            bodySlots[y].transform.localScale = defaultBodySlotScale.localScale;
            bodySlots[y].SetActive(true);
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


        for (var x = 0; x < bodyPices.Length; x++)
        {
            var gobject = Instantiate(bodyPiece, transform.position, Quaternion.identity, Planet.inst.getTrailContainerTransform());
            gobject.gameObject.SetActive(false);
            if (x < maxBodyPieceCount)
            {
                gobject.gameObject.SetActive(true);
            }
 
            bodyPices[x] = gobject;
  
            var bodySlotObject = Instantiate(slotPiece, transform.position, Quaternion.identity, Planet.inst.getTrailContainerTransform());
            bodySlotObject.SetActive(false);
            if (x < maxSlotCount)
            {
                bodySlotObject.SetActive(true);
            }
          
            bodySlots[x] = bodySlotObject;
        }

        defaultBodySlotScale = bodySlots[0].transform;
        defaultBodyPartScale = bodyPices[0].transform;

        isArrayReachedSafeIndex = true;

        updateObjectsLoop();
    }

    void updateObjectsLoop(bool eating = false)
    {
        var _bodyPieces = bodyPices;
        for (var x = 0; x < _bodyPieces.Length; x++)
        {
            if (x < 4)
            {
                _bodyPieces[x].toggleCollider(false);
            }
            if (x < maxBodyPieceCount)
            {
                if (eating && (x+1 == maxBodyPieceCount))
                {
                    _bodyPieces[x].gameObject.SetActive(true);
                    _bodyPieces[x].moveTowardScale(x, maxBodyPieceCount - 1);
                }
                else
                {
                    _bodyPieces[x].gameObject.SetActive(true);
                    _bodyPieces[x].setScale(x, maxBodyPieceCount - 1);
                }
    
            }
            else
            {

                if (x + 1 == bodyPices.Length) return;
                if (!_bodyPieces[x+1].gameObject.activeInHierarchy)
                {
                    break;
                }
                _bodyPieces[x].gameObject.SetActive(false);
            }

            
        }

        var _bodySlots = bodySlots;
        for (var x = 0; x < _bodySlots.Length; x++)
        {
            if (x < maxSlotCount)
            {
                if (x >= maxBodyPieceCount)
                {
                    _bodySlots[x].gameObject.SetActive(true);
                }
                else
                {
                    _bodySlots[x].gameObject.SetActive(false);
                }

            }
            else
            {
                break;
            }
           
        }
    
    }

    float lastEat;
    float comboWaitLength = 3f;
    int comboCount;
    int maxComboCount = 25;

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
            ScreenEffects.inst.flashScreen(new Color(1f, 0.58f, 0.46f), 20f, .35f);
            AudioManager.inst.playDangerEatSFX();
        }
        else
        {
            ScreenEffects.inst.flashScreen(new Color(0.89f, 0.83f, 0.46f), 20f, .35f);

            AudioManager.inst.playEatSFX(comboCount);

        }

        GameManager.inst.updateMaxCombo(comboCount);
        ScoreEffects.inst.doPointEffect(target, comboCount, pointCount);

        if (!addBodyPart)
            return;
      

        if (maxBodyPieceCount < bodyPices.Length)
            maxBodyPieceCount += 1;


        GameManager.inst.totalEatedCount++;
        updateObjectsLoop(true);
        if (GameManager.inst.totalEatedCount == GameManager.inst.requiredEatCountForNextLevel)
        {
            isReachedMax = true;
            GameManager.inst.levelUp();
        }
   
    }


    public void setSlotCount(int count)
    {
        if (count > bodyPices.Length)
        {
            maxSlotCount = bodyPices.Length;
            trustedSlotCount = bodyPices.Length;
        }
        else
        {
            maxSlotCount = count;
            trustedSlotCount = count;
        }
  
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
        return bodyPices.Length;
    }

    public int getBaseBodyCount()
    {
        return baseMaxBodyPieceCount;
    }
}
