using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(CircleLoop2))]
public class ReturnPool2 : MonoBehaviour
{
    public CircleLoop2 circleLoop;
    public IObjectPool<GameObject> pool;

    void Start()
    {
        circleLoop = GetComponent<CircleLoop2>();
        circleLoop.backToPool += OnCircleLoopStopped;   //subscribe the action
    }

    void OnCircleLoopStopped()
    {
        //Return to the pool
        pool.Release(gameObject);
    }
}