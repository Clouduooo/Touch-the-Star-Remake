using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircleLoop2 : MonoBehaviour
{
    LightSquare lightSquare;

    SpriteRenderer spriteRenderer;
    float radius;
    Vector3 middlePos;
    private float t;
    private float alpha;
    [SerializeField] AnimationCurve lightTileCurve;
    [SerializeField] AnimationCurve fadeCurve;
    TileManager tileManager;
    public Action backToPool;       //boardcast the action to put this gameobject back to objectpool
    float lastRadius;

    private void Awake()
    {
        lightSquare = GameObject.Find("Square1_0").GetComponent<LightSquare>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
    }

    private void OnEnable()
    {
        radius = 0;
        t = 0;
        lastRadius=0;
        spriteRenderer.color = new Color(1, 1, 1, 0.2f);
        transform.localScale=Vector2.zero;
    }

    private void Update()
    {
        ExtendAndFade();
    }

    // circle loop's function
    void ExtendAndFade()
    {
        if (t < lightSquare.extendDuration)
        {
            middlePos = transform.position;
            t += Time.deltaTime;
            radius += lightTileCurve.Evaluate(t) * 25f * Time.deltaTime;
            transform.localScale = new Vector3(radius / 2.0f, radius / 2.0f, radius / 2.0f);
            if (t >= lightSquare.extendDuration - 1f)
            {
                alpha = fadeCurve.Evaluate(lightSquare.extendDuration - t);    //value in 0~0.2
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