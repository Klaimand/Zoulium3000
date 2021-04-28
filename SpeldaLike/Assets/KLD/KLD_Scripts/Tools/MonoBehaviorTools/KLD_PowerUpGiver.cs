using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PowerUpGiver : MonoBehaviour
{

    KLD_PlayerController playerController = null;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<KLD_PlayerController>();
    }

    public void GivePowerUp(int _powerUpID)
    {
        playerController.GivePowerUp((KLD_PlayerController.PowerUp)_powerUpID);
    }
}
