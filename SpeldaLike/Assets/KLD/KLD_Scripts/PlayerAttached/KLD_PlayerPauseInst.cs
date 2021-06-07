using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PlayerPauseInst : MonoBehaviour
{

    KLD_PauseMenu pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenuCanvas").GetComponent<KLD_PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") && pauseMenu != null)
        {
            pauseMenu.PressedKey();
        }
    }
}
