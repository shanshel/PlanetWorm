using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtJoystickDir : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var worldUp = Vector3.up;
        var lastDir = Planet.inst.getLastDir();
        transform.LookAt(new Vector3(-lastDir.x * 5f, -lastDir.y * 5f, 7f), Vector3.up);
    }
}
