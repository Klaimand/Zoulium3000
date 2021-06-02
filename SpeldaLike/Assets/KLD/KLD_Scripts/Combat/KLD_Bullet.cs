using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Bullet : MonoBehaviour
{

    [SerializeField] float speed = 10f;

    [SerializeField] int damage = 1;

    [SerializeField] float destroyTime = 3f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SendMessage("DamagePlayer", damage, SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);
    }
}
