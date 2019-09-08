using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Player : MonoBehaviour
{

    public static Player inst;
    [SerializeField]
    private GameObject playerHead, followPoint, playerHeadModel;
    Vector3 playerHeadPos;



    private void Awake()
    {
        inst = this;
    }
    void Start()
    {
        playerHeadPos = playerHead.transform.position;
    }

    void Update()
    {
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

    public void whileLevelingUp()
    {
        StartCoroutine(updateFollowPointInLevelingUp());
    }

    IEnumerator updateFollowPointInLevelingUp()
    {
        yield return null;
        
        /*
        var targetPos = new Vector3(playerHead.transform.position.x, playerHead.transform.position.y, playerHead.transform.position.z + 10f);
        //targetPos = new Vector3(2f, 9f, 8f);
        PlayerTrail.inst.setSpeedLevel(1);
        yield return playerHead.transform.DOMove(targetPos, 1f).WaitForCompletion();
        PlayerTrail.inst.setSpeedLevel(2);
        //playerHead.transform.position = targetPos;
        yield return new WaitForSeconds(.3f);
        playerHead.transform.DOMove(playerHeadPos, .1f);
        yield return new WaitForSeconds(.3f);
        PlayerTrail.inst.setSpeedLevel(1);

        yield return new WaitForSeconds(.4f);
        PlayerTrail.inst.setSpeedLevel(0);
        yield return null;
        */
   
    }
}
