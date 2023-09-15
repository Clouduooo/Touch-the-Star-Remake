using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        x0 = (int)Mathf.Ceil(grid.WorldToCell(new Vector3(middlePos.x - radius, middlePos.y, middlePos.z)).x);
        x1 = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x + radius, middlePos.y, middlePos.z)).x);
        y0 = (int)Mathf.Ceil(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y - radius, middlePos.z)).y);
        y1 = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y + radius, middlePos.z)).y);
        z = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y, middlePos.z)).z);

        for (int x = x0; x <= x1; x++)
        {
            for (int y = y0; y <= y1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                for (int i = 0; i < Tiles.Length; i++)
                {
                    Tilemap tilemap = Tiles[i].GetComponent<Tilemap>();
                    if (tilemap.GetColor(pos).a == 0f)
                    {
                        StartCoroutine(TileAppearAlpha(tilemap, pos));
                    }
                }
            }
        }
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
