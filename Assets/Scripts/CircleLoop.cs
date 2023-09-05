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
    [SerializeField] float fadeDuaration;   //��Ƭ����ʱ�䳤��
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

    ///��Ȧ��ɢ������ʧ
    void ExtendAndFade()
    {
        if (t < player.parameter.extendDuration)
        {
            middlePos = transform.position;
            t += Time.deltaTime;
            radius = lightTileCurve.Evaluate(t) * 5;        //�뾶��ʱ����������
            transform.localScale = new Vector3(radius/2.0f, radius/2.0f, radius/2.0f);
            if (t >= player.parameter.extendDuration - 1f)               //�𽥵���
            {
                alpha = fadeCurve.Evaluate(player.parameter.extendDuration - t);    //0-1֮��
                spriteRenderer.color = new Color(1, 1, 1, alpha);
            }
            //����boudingbox
            x0 = (int)Mathf.Ceil(grid.WorldToCell(new Vector3(middlePos.x - radius, middlePos.y, middlePos.z)).x);
            x1 = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x + radius, middlePos.y, middlePos.z)).x);
            y0 = (int)Mathf.Ceil(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y - radius, middlePos.z)).y);
            y1 = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y + radius, middlePos.z)).y);
            z = (int)Mathf.Floor(grid.WorldToCell(new Vector3(middlePos.x, middlePos.y, middlePos.z)).z);
            Debug.DrawLine(new Vector3(middlePos.x - radius, middlePos.y - radius, middlePos.z), new Vector3(middlePos.x + radius, middlePos.y - radius, middlePos.z), Color.green, .0f, false);
            Debug.DrawLine(new Vector3(middlePos.x - radius, middlePos.y - radius, middlePos.z), new Vector3(middlePos.x - radius, middlePos.y + radius, middlePos.z), Color.green, .0f, false);
            Debug.DrawLine(new Vector3(middlePos.x + radius, middlePos.y - radius, middlePos.z), new Vector3(middlePos.x + radius, middlePos.y + radius, middlePos.z), Color.green, .0f, false);
            Debug.DrawLine(new Vector3(middlePos.x - radius, middlePos.y + radius, middlePos.z), new Vector3(middlePos.x + radius, middlePos.y + radius, middlePos.z), Color.green, .0f, false);
            //�����޸�tilemap��͸���� ��1s��alpha��0-1
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
            //�ջص������
            //Destroy(gameObject);
        }
    }

    //��Ƭ�𽥳���
    IEnumerator TileAppearAlpha(Vector3Int pos)
    {
        float startTime = Time.time;
        float endTime = startTime + fadeDuaration; // ����ʱ��ΪfadeDuaration��
        //Color color = new Color(1, 1, 1, 0);
        Color color = lightTile.GetColor(pos);

        while (Time.time < endTime)
        {
            float t1 = (Time.time - startTime) / (endTime - startTime); // ���㽥�����(0-1֮��)
            color.a = t1; // ���ݽ�������͸����
            player.parameter.lightTile.SetTileFlags(pos, TileFlags.None);   //����Tile
            player.parameter.lightTile.SetColor(pos, color);                //������ɫ
            yield return null;
        }

        // ������ɺ󣬽�͸��������Ϊ����ֵ1
        color.a = 1f;
        player.parameter.lightTile.SetTileFlags(pos, TileFlags.None);
        player.parameter.lightTile.SetColor(pos, color);
    }
}
