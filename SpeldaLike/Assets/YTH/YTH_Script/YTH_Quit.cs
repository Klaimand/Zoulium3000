using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTH_Quit : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Bien ouej t'as quitté");
        Application.Quit();
    }
}
