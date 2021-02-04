using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KLD_GravityObject : MonoBehaviour
{
    //references
    Rigidbody rb;

    //serialized variables
    [SerializeField]
    int id = 0;

    [SerializeField]
    Vector2 minMaxForceOnGravityDisable = Vector2.zero;

    [SerializeField]
    Vector2 minMaxTorqueOnGravityDisable = Vector2.zero;

    //variables
    bool isGravityDisabled = false;
    float startAngularDrag = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startAngularDrag = rb.angularDrag;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.onGravityDisable += OnGravityDisable;
        GameEvents.Instance.onGravityEnable += OnGravityEnable;
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onGravityDisable -= OnGravityDisable;
        GameEvents.Instance.onGravityEnable -= OnGravityEnable;
    }

    void OnGravityDisable(int _id)
    {
        if (!isGravityDisabled && _id == id)
        {
            isGravityDisabled = true;

            rb.useGravity = false;
            rb.angularDrag = 0f;
            rb.AddForce(Vector3.up * Random.Range(minMaxForceOnGravityDisable.x, minMaxForceOnGravityDisable.y), ForceMode.Force);
            rb.AddTorque(Random.onUnitSphere * Random.Range(minMaxTorqueOnGravityDisable.x, minMaxTorqueOnGravityDisable.y), ForceMode.Force);
        }
    }

    void OnGravityEnable(int _id)
    {
        if (_id == id)
        {
            rb.useGravity = true;
            isGravityDisabled = false;
            rb.angularDrag = startAngularDrag;
        }
    }

}
