using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerBodyController : MonoBehaviour
{
    [SerializeField]
    SpriteShapeController left,right;
    [SerializeField]
    Transform headleft,headright,legleft,legright;

    void Start()
    {

    }

    void Update()
    {
        left.spline.SetPosition(0,headleft.position-left.gameObject.transform.position);
        left.spline.SetPosition(1,legleft.position-left.gameObject.transform.position);
        right.spline.SetPosition(0,headright.position-right.gameObject.transform.position);
        right.spline.SetPosition(1,legright.position-right.gameObject.transform.position);
    }
}
