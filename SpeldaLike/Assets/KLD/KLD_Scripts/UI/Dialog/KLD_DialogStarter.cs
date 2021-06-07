using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

//[RequireComponent(typeof(Collider))]
public class KLD_DialogStarter : SerializedMonoBehaviour
{

    KLD_DialogManager manager;

    [SerializeField] KLD_Dialog dialog;

    [SerializeField] bool forceDialog = false;
    bool alreadyDialoged = false;
    [SerializeField, ShowIf("forceDialog")] bool canRedialog = false;

    bool playerIsInTrigger = false;

    bool gotButton = false;

    public UnityEvent onDialogStart;
    public UnityEvent onDialogEnd;


    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("DIALOG_Canvas").GetComponent<KLD_DialogManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsInTrigger && Input.GetButtonDown("Interact"))
        {
            gotButton = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = true;

            if (forceDialog && !alreadyDialoged)
            {
                if (manager.StartDialog(dialog, this))
                {
                    alreadyDialoged = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (gotButton && (!forceDialog || (forceDialog && canRedialog)))
            {
                gotButton = false;
                manager.StartDialog(dialog, this);
            }
            else if (forceDialog && !alreadyDialoged)
            {
                if (manager.StartDialog(dialog, this))
                {
                    alreadyDialoged = true;
                }
            }
        }
    }

    public void ForceDialogEvent()
    {
        StartCoroutine(KeepForcing());
    }

    IEnumerator KeepForcing()
    {
        bool b = false;

        while (b == false)
        {
            b = manager.StartDialog(dialog, this);
            yield return null;
        }
    }
}
