using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_LaserBoss : MonoBehaviour
{

    Transform player;

    [SerializeField] Transform head;
    [SerializeField] float playerHeight = 1f;

    [SerializeField] LineRenderer lr;
    [SerializeField] LayerMask laserMask;

    [SerializeField] GameObject damageZone;

    bool stopLaser = false;

    Vector3 hitPoint;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopLaser)
        {

            DoGlobalRotation();

            head.LookAt(player.position + (Vector3.up * playerHeight));
        }

        RaycastHit hit;
        Physics.Raycast(head.position, head.forward, out hit, Mathf.Infinity, laserMask);
        hitPoint = hit.point;
        lr.SetPosition(1, head.InverseTransformPoint(hit.point));
    }


    void DoGlobalRotation()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float playerAngle = Vector3.SignedAngle(Vector3.forward, toPlayer, Vector3.up);

        transform.rotation = Quaternion.Euler(0f, playerAngle, 0f);
    }

    public void StopLaser()
    {
        stopLaser = true;

        Instantiate(damageZone, hitPoint, Quaternion.identity);
    }

    public void EndLaser()
    {
        //do


        Destroy(gameObject);
    }
}
