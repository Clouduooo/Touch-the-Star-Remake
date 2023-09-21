using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CircleObjectPool : MonoBehaviour
{
    [SerializeField] GameObject loop_Prefab;

    public enum PoolType
    {
        Stack, LinkedList
    }

    public PoolType poolType;

    //Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 40;

    IObjectPool<GameObject> c_pool;     //for player

    public IObjectPool<GameObject> C_Pool
    {
        get 
        { 
            if (c_pool == null)
            {
                if(poolType == PoolType.Stack)
                {
                    c_pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 20, maxPoolSize);
                }
                else
                {
                    c_pool = new LinkedPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, maxPoolSize);
                }
            }
            return c_pool;
        }
    }

    GameObject CreatePooledItem()
    {
        var go = Instantiate(loop_Prefab, transform.position, Quaternion.identity);
        var cl = go.GetComponent<CircleLoop>();         //Get the reference of the Script Component on the gameOject

        //This is used to return gameObject to the pool when they have stopped.
        var returnToPool = go.GetComponent<ReturnToPool>();
        returnToPool.pool = C_Pool;

        return go;
    }

    //Called when an item is returned to the pool using Release
    void OnReturnedToPool(GameObject circleLoop)
    {
        circleLoop.SetActive(false);
    }

    //Called when an item is taken from the pool using Get
    void OnTakeFromPool(GameObject circleLoop)
    {
        circleLoop.transform.position = transform.position;
        circleLoop.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(GameObject circleLoop)
    {
        Destroy(circleLoop);
    }
}
