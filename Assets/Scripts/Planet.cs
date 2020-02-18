using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Planet : MonoBehaviour
{

    public static Planet inst;
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    public GameObject currentPlanetContainer, trailContainer;

    public List<GameObject> planets;
    public List<PlanetPlaceHolder> planetsPlaceholders;
    private Vector3 lastDir;

    [SerializeField]
    private float currentMoveSpeed = 50f, maxMoveSpeed = 130f, additnalMoveSpeedPerLevel = 5f;
    private GameObject oldPlanet;


    private void Awake()
    {
        inst = this;
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
    // Start is called before the first frame update

    public void onLoadGame()
    {

    }

  
    public Vector3 getLastDir()
    {
        return lastDir;
    }

    public float getPlanetSpeed()
    {
        return currentMoveSpeed;
    }
    // Update is called once per frame


    float jUsedSpeed;
    Vector2 jNormlized;
    private void FixedUpdate()
    {
    
        var usedSpeed = currentMoveSpeed + (additnalMoveSpeedPerLevel * GameManager.inst.level);
        if (usedSpeed > maxMoveSpeed)
        {
            usedSpeed = maxMoveSpeed;
        }
        if (GameManager.inst.level > 1)
        {
            usedSpeed += 10f;
        }
        if (GameManager.inst.isPlayerDied)
        {
            usedSpeed = 10f;
        }
    


        if ((jNormlized.y == 0 && jNormlized.x == 0))
        {
            if (lastDir.x == 0f && lastDir.y == 0f)
            {
                jNormlized.y = -1;
            }
            else
            {
                jNormlized = lastDir;
            }
            currentPlanetContainer.transform.Rotate(-jNormlized.y * (Time.deltaTime * Time.timeScale) * usedSpeed, jNormlized.x * (Time.deltaTime * Time.timeScale) * usedSpeed, 0f, Space.World);
        }

        else
        {
            if (Vector2.Distance(lastDir, jNormlized) > .1f)
            {
                usedSpeed += 15f;
            }
            currentPlanetContainer.transform.Rotate(-jNormlized.y * (Time.deltaTime * Time.timeScale) * usedSpeed, jNormlized.x * (Time.deltaTime * Time.timeScale) * usedSpeed, 0f, Space.World);
            lastDir = jNormlized;
        }


    }

    void Update()
    {
        if (!GameManager.inst.isGameStarted)
            return;


        jNormlized = joystick.Direction.normalized;


        /*

        var normalizeDir = joystick.Direction.normalized;
        var jVerical = normalizeDir.y;
        var jHorizontal = normalizeDir.x;

        var usedSpeed = currentMoveSpeed + (additnalMoveSpeedPerLevel * GameManager.inst.level);
        if (usedSpeed > maxMoveSpeed)
        {
            usedSpeed = maxMoveSpeed;
        }

        if (GameManager.inst.isPlayerDied)
        {
            usedSpeed = 10f;
        }

        if ((jVerical == 0 && jHorizontal == 0))
        {
            if (lastDir.x == 0f && lastDir.y == 0f)
            {
                lastDir.x = 0f;
                lastDir.y = -1f;
            }
            jVerical = lastDir.y;
            jHorizontal = lastDir.x;
            currentPlanetContainer.transform.Rotate(-jVerical * (Time.deltaTime * Time.timeScale) * usedSpeed, jHorizontal * (Time.deltaTime * Time.timeScale) * usedSpeed, 0f, Space.World);
        }
  
        else
        {
            if (Vector2.Distance(new Vector2(lastDir.x, lastDir.y), normalizeDir) > .05f)
            {
                usedSpeed += 40f;
            }
            currentPlanetContainer.transform.Rotate(-jVerical * (Time.deltaTime * Time.timeScale) * usedSpeed, jHorizontal * (Time.deltaTime * Time.timeScale) * usedSpeed, 0f, Space.World);
            lastDir = normalizeDir;
        }
        
        */
    }


    public GameObject getCurrentPlanetContainer()
    {
        return currentPlanetContainer;
    }

    public Transform getTrailContainerTransform()
    {
        return trailContainer.transform;
    }

    public void moveActivePlanetBehindCamera()
    {
        oldPlanet = currentPlanetContainer;
        var currentValue = oldPlanet.transform.position;
        var endValue = new Vector3(currentValue.x - 2f, currentValue.y - 3f, currentValue.z - 2f);
        oldPlanet.transform.DOMove(endValue, 1.8f);

        Invoke("pushOldPlanetFarrer", 1.8f);
       
    }

    public void pushOldPlanetFarrer()
    {
        oldPlanet.transform.DOMoveZ(oldPlanet.transform.position.z - 8f, 1.8f);
        Destroy(oldPlanet, 3f);
    }


    int _innerLevel, _nextInnerLevel, _nextNextInnerLevel;

    private void updateInnerLevelInfo(int level)
    {
        var maxInnerLevel = planets.Count-1;
        var innerLevel = 0;
        var nextInnerLevel = 1;
        var nextNextInnerLevel = 2;
        for (var x = 0; x < level; x++)
        {
            innerLevel++;
            nextInnerLevel++;
            nextNextInnerLevel++;

            if (innerLevel > maxInnerLevel)
            {
                innerLevel = 0;
            }
            if (nextInnerLevel > maxInnerLevel)
            {
                nextInnerLevel = 0;
            }
            if (nextNextInnerLevel > maxInnerLevel)
            {
                nextNextInnerLevel = 0;
            }
        }

        _innerLevel = innerLevel;
        _nextInnerLevel = nextInnerLevel;
        _nextNextInnerLevel = nextNextInnerLevel;
    }


    public void spawnNextPlanet()
    {
 
        updateInnerLevelInfo(GameManager.inst.level);
        planetsPlaceholders[_innerLevel].gameObject.SetActive(false);
        currentPlanetContainer = Instantiate(planets[_innerLevel], planetsPlaceholders[_innerLevel].transform.position, Quaternion.identity, transform);
        currentPlanetContainer.transform.DOMove(Vector3.zero, 2f);

        updatePlaceholderInScene();
    }

    private void updatePlaceholderInScene()
    {



        for (var x = 0; x < planetsPlaceholders.Count; x++)
        {
            planetsPlaceholders[x].gameObject.SetActive(false);
            planetsPlaceholders[x].setPos(EnumsData.PlaceHolderPos.outter);
        }

        planetsPlaceholders[_nextInnerLevel].setPos(EnumsData.PlaceHolderPos.far);
        planetsPlaceholders[_nextInnerLevel].gameObject.SetActive(true);
        planetsPlaceholders[_nextInnerLevel].moveToward(EnumsData.PlaceHolderPos.close);

        planetsPlaceholders[_nextNextInnerLevel].setPos(EnumsData.PlaceHolderPos.outter);
        planetsPlaceholders[_nextNextInnerLevel].gameObject.SetActive(true);
        planetsPlaceholders[_nextNextInnerLevel].moveToward(EnumsData.PlaceHolderPos.far);


    }
    
    public void onResetGame()
    {
        //Invoke("onResetGameLate", .5f);
    }

    public void onResetGameLate()
    {
       
    }



}
