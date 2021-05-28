using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PP8 : MonoBehaviour
{

    Transform target;

    [SerializeField, Header("Movement settings")]
    float maxAngularSpeed = 10f;
    [SerializeField] float timeToReachTargetAngle = 0.2f;

    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float smoothFactor = 0.5f;

    Vector3 velocity; //ref

    [SerializeField, Header("Animation settings")]
    float floatSpeed = 2f;
    [SerializeField] float floatAmplitude = 0.3f;
    [SerializeField] float noise = 1f;

    Transform child;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.GetChild(5);
        child = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        DoRotation();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothFactor, maxSpeed);

        child.localPosition = (Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude) + (Vector3.up * floatAmplitude / 2f) +
        (Vector3.up * Random.value * noise);

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
