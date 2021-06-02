using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PopupInst : MonoBehaviour
{
    KLD_PopupManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("POPUP_Canvas").GetComponent<KLD_PopupManager>();
    }

    public void EnablePopup(string _id)
    {
        manager.EnablePopup(_id);
    }

    public void DisablePopup(string _id)
    {
        manager.DisablePopup(_id);
    }

    public void DisableAllPopups()
    {
        manager.DisableAllPopups();
    }
}
