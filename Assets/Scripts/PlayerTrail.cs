using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerTrail : MonoBehaviour
{

    public static PlayerTrail inst;
    [SerializeField]
    private GameObject bodyPiece, slotPiece;
    [SerializeField]
    private int maxBodyPieceCount, maxSlotCount;
    private bool isReachedMax;
    private int lastMaxBodyPieceCount;
    public List<PlayerBodyPiece> bodyPices = new List<PlayerBodyPiece>();
    public List<GameObject> bodySlots = new List<GameObject>();

    private Transform playerPos;
    private GameObject playerHeadObject;


    private void Awake()
    {
        inst = this;
    }
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        generateObjectsLoop();
        speed = Planet.inst.getPlanetSpeed() / 50f;
        Invoke("slowStart", .5f);

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
            
            if (GameManager.inst.isPlayerDied == true)
            {
                yield return null;
                continue;
            }
               
            var playerTrans = Player.inst.getPlayerHeadTrans();
  
            for (var x = 0; x <= maxBodyPieceCount; x++)
            {

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

    void Update()
    {

    }

    public void whenPlayerDie()
    {

        StartCoroutine(playerDieCo());
    }

    IEnumerator playerDieCo()
    {
        playerHeadObject.SetActive(false);
        Instantiate(GameManager.inst.bodyBreakEffect, playerHeadObject.transform.position, Quaternion.identity, Planet.inst.currentPlanetContainer.transform);

        yield return new WaitForSeconds(.05f);


        for (var x = 0; x <= maxBodyPieceCount; x++)
        {
            bodyPices[x].transform.DOScale(0f, .3f);
            yield return new WaitForSeconds(.08f);
            Instantiate(GameManager.inst.bodyBreakEffect, bodyPices[x].transform.position, Quaternion.identity, Planet.inst.currentPlanetContainer.transform);
            AudioManager.inst.playSFX(EnumsData.SFXEnum.bodyBreak);
        }


        for (var y = 0; y <= maxSlotCount; y++)
        {
            if (y >= maxBodyPieceCount)
            {
                bodySlots[y].transform.DOScale(0f, .3f);
                yield return new WaitForSeconds(.08f);
                AudioManager.inst.playSFX(EnumsData.SFXEnum.bodyBreak);

            }

        }


        AudioManager.inst.playSFX(EnumsData.SFXEnum.fail);

    }

    void slowStart()
    {
        speed = Planet.inst.getPlanetSpeed() / 50f;
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
                gobject.SetActive(true);
            }
            var gameObject = gobject.GetComponent<PlayerBodyPiece>();
            bodyPices.Add(gameObject);




            var bodySlotObject = Instantiate(slotPiece, transform.position, Quaternion.identity, Planet.inst.getTrailContainerTransform());

            if (x < maxSlotCount)
            {
                bodySlotObject.SetActive(true);
            }
            bodySlots.Add(bodySlotObject);

        }

        updateObjectsLoop();
    }

    void updateObjectsLoop(bool eating = false)
    {
        for (var x = 0; x < bodyPices.Count; x++)
        {
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

    public void eatBall(Transform target)
    {
        if (isReachedMax) return;
        ScreenEffects.inst.flashScreen(new Color(0.88f, 1f, 0.14f), 150f, .35f);
        AudioManager.inst.playSFX(EnumsData.SFXEnum.eating);
       
        ScoreEffects.inst.doPointEffect(target);
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
    }

    public int getSlotCount()
    {
        return maxSlotCount;
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
