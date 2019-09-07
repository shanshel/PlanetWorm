using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 playerPos;
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
        speed = Planet.inst.getPlanetSpeed();
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
    IEnumerator coRotUpdate()
    {

        while (true)
        {
            playerPos = Player.inst.getPlayerHeadPos();
            playerHeadObject.transform.LookAt(playerPos, Vector3.forward);
            var _speed = speed;
            if (speedLevel == 1)
                _speed = 2f;
            else if (speedLevel == 2)
                _speed = 10f;

            Debug.Log("D: " + playerPos);

            if (Vector3.Distance(playerHeadObject.transform.position, playerPos) > .06f)
            {
                playerHeadObject.transform.position = Vector3.MoveTowards(playerHeadObject.transform.position, playerPos, Time.deltaTime * _speed);
            }


            //transform.position = Player.inst.getPlayerHead().transform.position;
            for (var x = 0; x <= maxBodyPieceCount; x++)
            {
                var targetPos = playerHeadObject.transform.position;
                if (x > 0)
                {
                    targetPos = bodyPices[x - 1].transform.position;
                }

                if (Vector3.Distance(bodyPices[x].transform.position, targetPos) > .8f)
                {
                    bodyPices[x].transform.position = Vector3.MoveTowards(bodyPices[x].transform.position, targetPos, Time.deltaTime * _speed * 50f);
                }
                else if (Vector3.Distance(bodyPices[x].transform.position, targetPos) > .12f)
                {
                    bodyPices[x].transform.position = Vector3.MoveTowards(bodyPices[x].transform.position, targetPos, Time.deltaTime * _speed);
                }


                bodyPices[x].transform.LookAt(Vector3.up);

            }



            for (var y = 0; y <= maxSlotCount; y++)
            {

                var targetPos = playerHeadObject.transform.position;
                if (y > 0)
                {
                    targetPos = bodySlots[y - 1].transform.position;
                }
                if (Vector3.Distance(bodySlots[y].transform.position, targetPos) > .12f)
                {
                    bodySlots[y].transform.position = Vector3.MoveTowards(bodySlots[y].transform.position, targetPos, Time.deltaTime * _speed);
                }



                bodySlots[y].transform.LookAt(Vector3.up);
            }

            yield return null;
        }
       
    }

    void Update()
    {
   
    }

    void slowStart()
    {
        speed = Planet.inst.getPlanetSpeed() / 50f;
    }

    Vector3 savedPlayerPosition;

    

 
    void generateObjectsLoop()
    {
        playerHeadObject = Instantiate(Player.inst.getPlayerHeadModel(), transform.position, Quaternion.identity, Planet.inst.getTrailContainerTransform());


        for (var x = 0; x < 50; x++)
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
