using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(CircleLoop))]
public class ReturnToPool : MonoBehaviour
{
    public CircleLoop circleLoop;
    public IObjectPool<GameObject> pool;

    void Start()
    {
        circleLoop = GetComponent<CircleLoop>();
        circleLoop.backToPool += OnCircleLoopStopped;   //subscribe the action
    }

    void OnCircleLoopStopped()
    {
        //Return to the pool
        pool.Release(gameObject);
    }
}
