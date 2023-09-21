using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class TileManager : MonoBehaviour
{
    [SerializeField] GridLayout grid;
    GameObject[] Tiles;
    [SerializeField] float fadeDuration;
    int x0,x1,y0,y1,z;

    private void Start()
    {
        Tiles = GameObject.FindGameObjectsWithTag("LightTile");
    }

    public bool SearchColor(Tilemap tilemap, Vector3 worldPos)
    {
        Vector3Int pos = grid.WorldToCell(worldPos);
        if (tilemap.GetColor(pos).a == 0.9f)
        {
            return true;
        }
        return false;
    }
    public void SetTileColor(Vector3 middlePos, float radius)
    {

        for (int i = 0; i < Tiles.Length; i++)
        {
            //x0 = (int)Mathf.Ceil(Tiles[i].GetComponent<Tilemap>().WorldToCell(new Vector3(middlePos.x - radius, middlePos.y, middlePos.z)).x);
            //x1 = (int)Mathf.Floor(Tiles[i].GetComponent<Tilemap>().WorldToCell(new Vector3(middlePos.x + radius, middlePos.y, middlePos.z)).x);
            //y0 = (int)Mathf.Ceil(Tiles[i].GetComponent<Tilemap>().WorldToCell(new Vector3(middlePos.x, middlePos.y - radius, middlePos.z)).y);
            //y1 = (int)Mathf.Floor(Tiles[i].GetComponent<Tilemap>().WorldToCell(new Vector3(middlePos.x, middlePos.y + radius, middlePos.z)).y);
            //z = (int)Mathf.Floor(Tiles[i].GetComponent<Tilemap>().WorldToCell(new Vector3(middlePos.x, middlePos.y, middlePos.z)).z);

            Tilemap tilemap = Tiles[i].GetComponent<Tilemap>();
            // Debug.DrawLine(new Vector3(x0,y0),new Vector3(x0,y1),Color.red,1);
            // Debug.DrawLine(new Vector3(x1,y0),new Vector3(x1,y1),Color.red,1);
            // Debug.DrawLine(new Vector3(x0,y0),new Vector3(x1,y0),Color.red,1);
            // Debug.DrawLine(new Vector3(x0,y1),new Vector3(x1,y1),Color.red,1);

            int mx = Mathf.RoundToInt(middlePos.x);
            int my = Mathf.RoundToInt(middlePos.y);
            int x = 0;
            int y = Mathf.RoundToInt(radius);
            int d = Mathf.RoundToInt(1 - radius);

            while (y >= x)
            {
                Vector3Int pos;

                pos = new Vector3Int(mx + x, my + y, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx + x, my - y, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx - x, my + y, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx - x, my - y, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx + y, my + x, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx + y, my - x, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx - y, my + x, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx - y, my - x, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));



                pos = new Vector3Int(mx + x - 1, my + y, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx + x - 1, my - y, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx - x + 1, my + y, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx - x + 1, my - y, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx + y - 1, my + x, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx + y - 1, my - x, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx - y + 1, my + x, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));

                pos = new Vector3Int(mx - y + 1, my - x, 0);
                if (tilemap.GetColor(pos).a == 0f)
                    StartCoroutine(TileAppearAlpha(tilemap, pos));


                if (d < 0)
                {
                    d = d + 2 * x + 3;
                }
                else
                {
                    d = d + 2 * (x - y) + 5;
                    y--;
                }
                x++;
            }

            //for (int x = x0; x <= x1; x++)
            //{
            //    Vector3Int pos = new Vector3Int(x, y0, 0);
            //    if (tilemap.GetColor(pos).a == 0f)
            //    {
            //        StartCoroutine(TileAppearAlpha(tilemap, pos));
            //    }
            //}
            //for (int x = x0; x <= x1; x++)
            //{
            //    Vector3Int pos = new Vector3Int(x, y1, 0);
            //    if (tilemap.GetColor(pos).a == 0f)
            //    {
            //        StartCoroutine(TileAppearAlpha(tilemap, pos));
            //    }
            //}
            //for (int y = y0; y <= y1; y++)
            //{
            //    Vector3Int pos = new Vector3Int(x0, y, 0);
            //    if (tilemap.GetColor(pos).a == 0f)
            //    {
            //        StartCoroutine(TileAppearAlpha(tilemap, pos));
            //    }
            //}
            //for (int y = y0; y <= y1; y++)
            //{
            //    Vector3Int pos = new Vector3Int(x1, y, 0);
            //    if (tilemap.GetColor(pos).a == 0f)
            //    {
            //        StartCoroutine(TileAppearAlpha(tilemap, pos));
            //    }
            //}
        }

        // for (int x = x0; x <= x1; x++)
        // {
        //     for (int y = y0; y <= y1; y++)
        //     {
        //         Vector3Int pos = new Vector3Int(x, y, 0);
        //         for (int i = 0; i < Tiles.Length; i++)
        //         {
        //             Tilemap tilemap = Tiles[i].GetComponent<Tilemap>();
        //             if (tilemap.GetColor(pos).a == 0f)
        //             {
        //                 StartCoroutine(TileAppearAlpha(tilemap, pos));
        //             }
        //         }
        //     }
        // }
    }

    IEnumerator TileAppearAlpha(Tilemap lightTile, Vector3Int pos)
    {
        float startTime = Time.time;
        float endTime = startTime + fadeDuration; // Tile fade in duration
        Color color = lightTile.GetColor(pos);

        while (Time.time < endTime)
        {
            float t1 = (Time.time - startTime) / (endTime - startTime); // get current alpha
            color.a = t1;
            lightTile.SetTileFlags(pos, TileFlags.None);   //unlock Tile
            lightTile.SetColor(pos, color);                //change color
            yield return null;
        }

        // make sure the final alpha is 0.9f
        color.a = 0.9f;
        lightTile.SetTileFlags(pos, TileFlags.None);
        lightTile.SetColor(pos, color);
    }
}
