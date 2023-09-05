using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircleLoop : MonoBehaviour
{
    GridLayout grid;
    Tilemap lightTile;
    PlayerFSM player;

    SpriteRenderer spriteRenderer;
    [SerializeField] float radius;
    [SerializeField] Vector3 middlePos;
    private float t;
    private float alpha;
    [SerializeField] float fadeDuaration;   //瓦片显现时间长度
    [SerializeField] AnimationCurve lightTileCurve;
    [SerializeField] AnimationCurve fadeCurve;

    private int x0, x1, y0, y1, z;    //boudingbox

    private void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<GridLayout>();
        player = GameObject.Find("cat").GetComponent<PlayerFSM>();
        lightTile = player.parameter.lightTile;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        t = 0;
    }

    private void Update()
    {
        ExtendAndFade();
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("LightTile"))
    //    {
    //        collision.gameObject.GetComponent<SpriteRenderer>().color = lightTileColor;
    //    }
    //}

    ///光圈扩散并逐渐消失
    void ExtendAndFade()
    {
        if (t < player.parameter.extendDuration)
        {
            middlePos = transform.position;
            t += Time.deltaTime;
            radius = lightTileCurve.Evaluate(t) * 5;        //半径随时间曲线扩大
            transform.localScale = new Vector3(radius, radius, radius);
            if (t >= player.parameter.extendDuration - 1f)               //逐渐淡出
            {
                alpha = fadeCurve.Evaluate(player.parameter.extendDuration - t);    //0-1之间
                spriteRenderer.color = new Color(1, 1, 1, alpha);
            }
            //设置boudingbox
            x0 = (int)Mathf.Ceil(grid.WorldToCell(new Vector3(middlePos.x - radius, middlePos.y, middlePos.z)).x);
            x1 = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x + radius, middlePos.y, middlePos.z)).x);
            y0 = (int)Mathf.Ceil(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y - radius, middlePos.z)).y);
            y1 = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y + radius, middlePos.z)).y);
            z = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y, middlePos.z)).z);
            Debug.DrawLine(new Vector3(middlePos.x - radius, middlePos.y - radius, middlePos.z), new Vector3(middlePos.x + radius, middlePos.y - radius, middlePos.z), Color.green, .5f, false);
            Debug.DrawLine(new Vector3(middlePos.x - radius, middlePos.y - radius, middlePos.z), new Vector3(middlePos.x - radius, middlePos.y + radius, middlePos.z), Color.green, .5f, false);
            Debug.DrawLine(new Vector3(middlePos.x + radius, middlePos.y - radius, middlePos.z), new Vector3(middlePos.x + radius, middlePos.y + radius, middlePos.z), Color.green, .5f, false);
            Debug.DrawLine(new Vector3(middlePos.x - radius, middlePos.y + radius, middlePos.z), new Vector3(middlePos.x + radius, middlePos.y + radius, middlePos.z), Color.green, .5f, false);
            //遍历修改tilemap的透明度 在1s内alpha从0-1
            for (int x = x0; x <= x1; x++)
            {
                for (int y = y0; y <= y1; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (lightTile.GetColor(pos).a == 0f)
                    {
                        StartCoroutine(TileAppearAlpha(pos));
                    }
                }
            }
        }
        else
        {
            //收回到对象池
            //Destroy(gameObject);
        }
    }

    //瓦片逐渐出现
    IEnumerator TileAppearAlpha(Vector3Int pos)
    {
        float startTime = Time.time;
        float endTime = startTime + fadeDuaration; // 渐变时长为fadeDuaration秒
        Color color = new Color(1, 1, 1, 0);

        while (Time.time < endTime)
        {
            float t1 = (Time.time - startTime) / (endTime - startTime); // 计算渐变进度(0-1之间)
            color.a = t1; // 根据进度设置透明度
            player.parameter.lightTile.SetTileFlags(pos, TileFlags.None);   //解锁Tile
            player.parameter.lightTile.SetColor(pos, color);                //更改颜色
            yield return null;
        }

        // 渐变完成后，将透明度设置为最终值1
        color.a = 1f;
        player.parameter.lightTile.SetTileFlags(pos, TileFlags.None);
        player.parameter.lightTile.SetColor(pos, color);
    }
}
