using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MoveTileType
{
    Verticle, Oblique
}

public class MoveTile : MonoBehaviour
{
    private PlayerFSM player;
    private Tilemap lightTile;
    private BoundsInt bounds;
    public MoveTileType moveTileType;   //change in inspector----you need to sign this variable for each single moveTile
    public AnimationCurve speedCurve;   //change in inspector
    public AnimationCurve obliqueCurve; //change in inspector
    public float moveSpeedFix;          //change in inspector
    public float fadeDuration;          //change in inspector
    public int diverseDirection;        //change in inspector--default direction is up or up-right!   diversedirection please set -1
    private float moveSpeed;
    private float t;
    public bool isLightUp;

    private void Awake()
    {
        t = 0f;
        isLightUp = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerFSM>();
        lightTile = GetComponent<Tilemap>();
    }

    private void Start()
    {
        //Initialize all tiles with alpha = 0
        bounds = lightTile.cellBounds;
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                //Color color = new Color(1, 1, 1, 0);
                Color color = lightTile.GetColor(pos);
                color.a = 0;
                lightTile.SetTileFlags(pos, TileFlags.None);
                lightTile.SetColor(pos, color);
            }
        }
    }

    private void FixedUpdate()
    {
        PositionChange();
    }

    private void Update()
    {
        if(!player.parameter.inJumpState)
        {
            if (moveTileType == MoveTileType.Verticle)
            {
                VerticleMove();
            }
            else
            {
                ObliqueMove();
            }
        }
        else
        {
            moveSpeed = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isLightUp)
        {
            player.gameObject.transform.SetParent(transform, true);
        }
        if (collision.CompareTag("CircleLoop") && !isLightUp)
        {
            isLightUp = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.gameObject.transform.SetParent(null, true);
        }
    }

    void PositionChange()
    {
        if(moveTileType == MoveTileType.Verticle)
        {
            Vector3 deltaPos = new Vector3(0, moveSpeed * Time.fixedDeltaTime * diverseDirection, 0);
            transform.position += deltaPos;
        }
        else
        {
            Vector3 deltaPos = new Vector3(moveSpeed * Time.fixedDeltaTime * diverseDirection, moveSpeed * Time.fixedDeltaTime * diverseDirection, 0);
            transform.position += deltaPos;
        }
    }

    void VerticleMove()
    {
        t += Time.deltaTime;
        if(t < speedCurve.keys[speedCurve.length - 1].time)
        {
            moveSpeed = speedCurve.Evaluate(t) * moveSpeedFix;
        }
        else if(t >= speedCurve.keys[speedCurve.length - 1].time && t < 2 * speedCurve.keys[speedCurve.length - 1].time)
        {
            moveSpeed = -speedCurve.Evaluate(t - speedCurve.keys[speedCurve.length - 1].time) * moveSpeedFix;
        }
        else if(t >= 2*speedCurve.keys[speedCurve.length - 1].time)
        {
            t = 0f;
        }
    }

    void ObliqueMove()
    {
        t += Time.deltaTime;
        if (t < obliqueCurve.keys[obliqueCurve.length - 1].time)
        {
            moveSpeed = obliqueCurve.Evaluate(t) * moveSpeedFix;
        }
        else if (t >= obliqueCurve.keys[obliqueCurve.length - 1].time && t < 2 * obliqueCurve.keys[obliqueCurve.length - 1].time)
        {
            moveSpeed = -obliqueCurve.Evaluate(t - obliqueCurve.keys[obliqueCurve.length - 1].time) * moveSpeedFix;
        }
        else if (t >= 2 * obliqueCurve.keys[obliqueCurve.length - 1].time)
        {
            t = 0f;
        }
    }
}
