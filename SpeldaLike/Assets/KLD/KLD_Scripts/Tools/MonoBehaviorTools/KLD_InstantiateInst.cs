using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_InstantiateInst : MonoBehaviour
{
    public void InstantiateHere(GameObject _prefab)
    {
        Instantiate(_prefab, transform.position, transform.rotation);
    }
}
