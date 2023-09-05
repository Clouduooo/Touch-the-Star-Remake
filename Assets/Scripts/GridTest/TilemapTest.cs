using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TilemapTest : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap,tilemap1,tilemap2,tilemap3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition=Input.mousePosition;
            Vector2 worldPosition=Camera.main.ScreenToWorldPoint(mousePosition);
            Debug.Log("Mouse Position:"+mousePosition);
            Debug.Log("World Position:"+worldPosition);
            // Debug.Log("0:"+tilemap.WorldToLocal(worldPosition));
            // Debug.Log("1:"+tilemap1.WorldToLocal(worldPosition));
            // Debug.Log("2:"+tilemap2.WorldToLocal(worldPosition));
            // Debug.Log("3:"+tilemap3.WorldToLocal(worldPosition));
            // Debug.Log("0:"+tilemap.WorldToCell(worldPosition));
            // Debug.Log("1:"+tilemap1.WorldToCell(worldPosition));
            // Debug.Log("2:"+tilemap2.WorldToCell(worldPosition));
            // Debug.Log("3:"+tilemap3.WorldToCell(worldPosition));
            Debug.Log("0:"+tilemap.cellBounds);
            Debug.Log("1:"+tilemap1.cellBounds);
            Debug.Log("2:"+tilemap2.cellBounds);
            Debug.Log("3:"+tilemap3.cellBounds);
            Debug.Log("0:"+tilemap.localBounds);
            Debug.Log("1:"+tilemap1.localBounds);
            Debug.Log("2:"+tilemap2.localBounds);
            Debug.Log("3:"+tilemap3.localBounds);
        }
    }
}
