using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLoop : MonoBehaviour
{
    Color lightTileColor = Color.white;
    [SerializeField] AnimationCurve lightTileCurve;

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LightTile"))
        {
            collision.gameObject.GetComponent<SpriteRenderer>().color = lightTileColor;
        }
    }

    ///¹âÈ¦À©É¢²¢Öð½¥ÏûÊ§
    void ExtendAndFade()
    {
        
    }
}
