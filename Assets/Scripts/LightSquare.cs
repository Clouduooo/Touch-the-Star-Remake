using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSquare : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    SquareObjectPool pool;
    [SerializeField] Sprite squre2;
    private bool isActive;
    private float t;
    public float extendDuration = 4f;
    [SerializeField] float initCD;

    private void Awake()
    {
        t = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        pool = GetComponent<SquareObjectPool>();
    }

    private void Update()
    {
        if(isActive)
        {
            t += Time.deltaTime;
            if(t >= initCD)
            {
                pool.C_Pool.Get();
                t = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.CompareTag("CircleLoop"))
        {
            spriteRenderer.sprite = squre2;
            isActive = true;
        }
    }
}
