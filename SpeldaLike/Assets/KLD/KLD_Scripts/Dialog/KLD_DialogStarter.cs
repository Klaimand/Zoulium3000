using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class KLD_DialogStarter : MonoBehaviour
{

    KLD_DialogManager manager;

    public UnityEvent onDialogStart;
    public UnityEvent onDialogEnd;

    [SerializeField] KLD_Dialog dialog;


    bool playerIsInTrigger = false;

    bool gotButton = false;


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
            if (gotButton)
            {
                gotButton = false;
                manager.StartDialog(dialog, this);
            }
        }
    }
}
