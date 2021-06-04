using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_EnergyBall : MonoBehaviour
{

    //[SerializeField] float height = 30f;
    //[SerializeField] float timeToReach = 3f;
    //float curTime;
    //[SerializeField] AnimationCurve curve;
    //[SerializeField] GameObject damagePrefab;
    //Vector3 startPos, endPos;

    [SerializeField] float time = 3;
    [SerializeField] GameObject damagePrefab;


    void Start()
    {
        //curTime = 0f;
        //endPos = transform.position;
        //startPos = transform.position + Vector3.up * height;
        //transform.position = startPos;
        //StartCoroutine(StartFall());

        StartCoroutine(SpawnDamage());

        Destroy(gameObject, time + 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnDamage()
    {
        yield return new WaitForSeconds(time);
        Instantiate(damagePrefab, transform.position, Quaternion.identity);
    }

    //IEnumerator StartFall()
    //{
    //    while (curTime < timeToReach)
    //    {
    //        float t = curve.Evaluate(curTime / timeToReach);
    //        transform.position = Vector3.Lerp(startPos, endPos, t);
    //        curTime += Time.deltaTime;
    //        yield return null;
    //    }
    //
    //    Instantiate(damagePrefab, endPos, Quaternion.identity);
    //}
}
