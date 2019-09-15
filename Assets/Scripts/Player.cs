using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Player : MonoBehaviour
{

    public static Player inst;
    [SerializeField]
    private GameObject playerHead, followPoint, playerHeadModel;
    Vector3 playerHeadDefaultPos, playerHeadMainMenuPos;



    private void Awake()
    {
        inst = this;
    }
    void Start()
    {
        playerHeadDefaultPos = playerHead.transform.position;
        InvokeRepeating("updatePlayerPointInMainMenuScene", 0f, .5f);
    }

    void updatePlayerPointInMainMenuScene()
    {
        var randomX = Random.Range(-.4f, .4f);
        var randomY = Random.Range(-1f, .5f);
        var conZ = -1.75f;
        playerHeadMainMenuPos = new Vector3(randomX, randomY, conZ);
    }

    void Update()
    {
        if (!GameManager.inst.isGameStarted)
        {
            playerHead.transform.position = Vector3.MoveTowards(playerHead.transform.position , playerHeadMainMenuPos, Time.smoothDeltaTime);
            return;
        }
        else
        {
            playerHead.transform.position = Vector3.MoveTowards(playerHead.transform.position, playerHeadDefaultPos, Time.smoothDeltaTime * 2f);
        }
         

        var worldUp = Vector3.up;
        var lastDir = Planet.inst.getLastDir();

        playerHead.transform.LookAt(new Vector3(-lastDir.x * 5f, -lastDir.y * 5f, 7f), Vector3.up);

        /*
        if (!GameManager.inst.isLevelingUp)
        {
            playerHead.transform.LookAt(new Vector3(-lastDir.x * 5f, -lastDir.y * 5f, 7f), Vector3.up);

        }
        else
        {
            playerHead.transform.LookAt(Planet.inst.currentPlanetContainer.transform.position, Vector3.up);
        }
        */

    }

    public Transform getPlayerHeadTrans()
    {
        return playerHead.transform;
    }

    public GameObject getPlayerHeadModel()
    {
        return playerHeadModel;
    }
    public GameObject getFollowPoint()
    {
        return followPoint;
    }



   
}
