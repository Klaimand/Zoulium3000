using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KLD_FirstSelect : MonoBehaviour
{

    [SerializeField] Selectable toSelect;

    // Start is called before the first frame update
    void Start()
    {
        toSelect.Select();
    }
}
