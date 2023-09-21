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
    AudioManager audioManager;
    bool isPlayed;

    private void Awake()
    {
        isPlayed = false;
        t = initCD;
        spriteRenderer = GetComponent<SpriteRenderer>();
        pool = GetComponent<SquareObjectPool>();
        //audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
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
        if (collision.CompareTag("CircleLoop"))
        {
            //audioManager.PlayEffect();
            spriteRenderer.sprite = squre2;
            isActive = true;
            //isPlayed = true;
        }
    }
}
