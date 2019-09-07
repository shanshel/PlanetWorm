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
      
        if (!GameManager.inst.isLevelingUp)
        {
            playerHead.transform.LookAt(new Vector3(-lastDir.x * 5f, -lastDir.y * 5f, 7f), Vector3.up);

        }
        else
        {
            playerHead.transform.LookAt(Planet.inst.currentPlanetContainer.transform.position, Vector3.up);
        }

    }

    public GameObject getPlayerHead()
    {
        return playerHead;
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

        var targetPos = new Vector3(playerHead.transform.position.x + 20f, playerHead.transform.position.y + 50f, playerHead.transform.position.z + 50f);
        //targetPos = new Vector3(2f, 9f, 8f);
        //yield return playerHead.transform.DOMove(targetPos, .95f).WaitForCompletion();
        //playerHead.transform.DOMove(playerHeadPos, .1f);
        yield return null;

    }
}
