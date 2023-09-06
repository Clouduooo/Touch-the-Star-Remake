using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircleLoop : MonoBehaviour
{
    PlayerFSM player;

    SpriteRenderer spriteRenderer;
    float radius;
    Vector3 middlePos;
    private float t;
    private float alpha;
    [SerializeField] AnimationCurve lightTileCurve;
    [SerializeField] AnimationCurve fadeCurve;
    TileManager tileManager;
    public Action backToPool;       //boardcast the action to put this gameobject back to objectpool

    private void Start()
    {
        player = GameObject.Find("cat").GetComponent<PlayerFSM>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
    }
    private void OnEnable()
    {
        t = 0;
    }

    private void Update()
    {
        ExtendAndFade();
    }

    // circle loop's function
    void ExtendAndFade()
    {
        if (t < player.parameter.extendDuration)
        {
            middlePos = transform.position;
            t += Time.deltaTime;
            radius = lightTileCurve.Evaluate(t) * 20;        
            transform.localScale = new Vector3(radius/2.0f, radius/2.0f, radius/2.0f);
            if (t >= player.parameter.extendDuration - 1f)         
            {
                alpha = fadeCurve.Evaluate(player.parameter.extendDuration - t);    //value in 0~1
                spriteRenderer.color = new Color(1, 1, 1, alpha);
            }
            tileManager.SetTileColor(middlePos, radius);
        }
        else
        {
            //change after the function of objectpool
            //Destroy(gameObject);
            backToPool.Invoke();    //boardcast the action
        }
    }
}
