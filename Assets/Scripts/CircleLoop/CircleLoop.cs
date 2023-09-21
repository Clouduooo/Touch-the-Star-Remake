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
    [SerializeField] Material SpriteDefault;
    [SerializeField] Material Circle;
    private float fixedDuration;    //Get the value of extendDuration OnEnable, then fixed
    private float lerpRatio;
    [SerializeField] float radiusSpeedFix;      //fix to keep on add up circleloop's radius
    float lastRadius;

    private void Awake()
    {
        player = GameObject.Find("cat").GetComponent<PlayerFSM>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
    }
    private void OnEnable()
    {
        spriteRenderer.color = Color.white;
        transform.localScale = Vector2.zero;
        fixedDuration = player.parameter.extendDuration;
        t = 0;
        radius = 0;
        lastRadius=0;
        if (player.parameter.isWalking)
        {
            spriteRenderer.material = Circle;
        }
        else
        {
            spriteRenderer.material = SpriteDefault;
        }
    }

    private void Update()
    {
        ExtendAndFade();
    }

    // circle loop's function
    void ExtendAndFade()
    {
        if (t < fixedDuration)
        {
            middlePos = transform.position;
            t += Time.deltaTime;
            //lerpRatio = t / fixedDuration;      //value in 0~1
            //radius += lightTileCurve.Evaluate(t) * 20 * fixedDuration;
            if(fixedDuration <= 1f)
            {
                radius += lightTileCurve.Evaluate(t) * Time.deltaTime * radiusSpeedFix;
            }else if(fixedDuration > 1f)
            {
                radius += lightTileCurve.Evaluate(t) * Time.deltaTime * radiusSpeedFix * 1.8f;
            }
            transform.localScale = new Vector3(radius/2.0f, radius/2.0f, radius/2.0f);
            if (t >= fixedDuration - 1f)         
            {
                alpha = fadeCurve.Evaluate(fixedDuration - t);    //value in 0~1
                spriteRenderer.color = new Color(1, 1, 1, alpha);
            }
            for(int r=Mathf.FloorToInt(lastRadius);r<=Mathf.Ceil(radius);r++)
            {
                tileManager.SetTileColor(middlePos, r);
            }
            lastRadius=radius;
        }
        else
        {
            //change after the function of objectpool
            //Destroy(gameObject);
            backToPool.Invoke();    //boardcast the action
        }
    }
}
