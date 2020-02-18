using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDetector : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.inst.isGameStarted || 
            GameManager.inst.isLevelingUp ||
            GameManager.inst.isInProtection ||
            GameManager.inst.isPlayerDied
            )
            return;

        GameManager.inst.death();
    }
}
