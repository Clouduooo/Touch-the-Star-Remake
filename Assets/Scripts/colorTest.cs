using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorTest : MonoBehaviour
{
    [SerializeField] TileManager tile;

    private void OnEnable()
    {
        tile.SearchColor(transform.position);
    }
}
