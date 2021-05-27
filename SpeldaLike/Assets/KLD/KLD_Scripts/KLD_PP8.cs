using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PP8 : MonoBehaviour
{

    Transform player;

    [SerializeField] float maxAngleSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void DoRotation()
    {
        //Vector3 vectorToTarget = _target.position - _looker.position;
        //float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        //Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //_looker.rotation = Quaternion.RotateTowards(_looker.rotation, q, Time.deltaTime * _maxRotateSpeed);







        //Quaternion q = Quaternion.AngleAxis()

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, )
    }
}
