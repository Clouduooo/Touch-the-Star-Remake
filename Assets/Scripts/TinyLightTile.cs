using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyLightTile : MonoBehaviour
{
    Color InitColor = new Color(1, 1, 1, 0);
    private void Start()
    {
        GetComponent<SpriteRenderer>().color = InitColor;
    }
}
