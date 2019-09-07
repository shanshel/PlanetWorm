using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eatable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collid");
    }
}
