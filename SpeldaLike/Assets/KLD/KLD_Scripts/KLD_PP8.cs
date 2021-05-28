using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PP8 : MonoBehaviour
{

    //Transform player;
    Transform target;

    [SerializeField] float maxAngularSpeed = 10f;
    [SerializeField] float timeToReachTargetAngle = 0.2f;

    //[SerializeField] Vector2 minMaxSpeed = new Vector2(3f, 10f);
    //float curSpeed = 5f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float smoothFactor = 0.5f;

    Vector3 velocity;

    Vector3 lastPos;

    //v = d/t

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.GetChild(5);
    }

    // Update is called once per frame
    void Update()
    {
        //lastPos = transform.position;

        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothFactor, maxSpeed);
        DoRotation();
    }


    void DoRotation()
    {
        float angle = Vector3.SignedAngle(transform.forward, target.forward, Vector3.up);
        angle = Mathf.Clamp(angle, -maxAngularSpeed, maxAngularSpeed);
        transform.Rotate(Vector3.up * angle * Time.deltaTime * (1f / timeToReachTargetAngle));

        //Vector3 direction = target.position - transform.position;
        //direction.y = 0f;
        //float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        //angle = Mathf.Clamp(angle, -maxAngleSpeed, maxAngleSpeed);
        //transform.Rotate(Vector3.up * angle * Time.deltaTime);

    }
}
