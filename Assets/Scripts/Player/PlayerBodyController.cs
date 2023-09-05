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

    private Vector3 lefthead,righthead,leftmid,rightmid,leftleg,rightleg;
    private Vector3 horizontalToLeft,horizontalToRight;

    void Start()
    {
        horizontalToLeft=headright.position-headleft.position;
        horizontalToLeft.Normalize();
        horizontalToRight=headleft.position-headright.position;
        horizontalToRight.Normalize();
    }

    void Update()
    {
        lefthead=headleft.position-left.gameObject.transform.position;
        righthead=headright.position-right.gameObject.transform.position;

        leftleg=legleft.position-left.gameObject.transform.position;
        rightleg=legright.position-right.gameObject.transform.position;

        leftmid=(lefthead+leftleg)/2;
        rightmid=(righthead+rightleg)/2;

        lefthead/=left.transform.localScale.x;
        righthead/=right.transform.localScale.x;
        leftmid/=left.transform.localScale.x;
        rightmid/=right.transform.localScale.x;
        leftleg/=left.transform.localScale.x;
        rightleg/=right.transform.localScale.x;

        left.spline.SetPosition(0,lefthead);
        left.spline.SetPosition(1,leftmid);
        left.spline.SetPosition(2,leftleg);

        right.spline.SetPosition(0,righthead);
        right.spline.SetPosition(1,rightmid);
        right.spline.SetPosition(2,rightleg);

        Vector3 vecMidTo,horiVec,vertVec,tangent;

        vecMidTo=lefthead-leftmid;
        horiVec=Vector3.Dot(vecMidTo,horizontalToLeft)*horizontalToLeft;
        vertVec=lefthead-(leftmid+horiVec);
        tangent=horiVec+0.5f*vertVec;
        left.spline.SetLeftTangent(1,tangent);

        vecMidTo=leftleg-leftmid;
        horiVec=Vector3.Dot(vecMidTo,horizontalToRight)*horizontalToRight;
        vertVec=leftleg-(leftmid+horiVec);
        tangent=horiVec+0.5f*vertVec;
        left.spline.SetRightTangent(1,tangent);

        vecMidTo=righthead-rightmid;
        horiVec=Vector3.Dot(vecMidTo,horizontalToLeft)*horizontalToLeft;
        vertVec=righthead-(rightmid+horiVec);
        tangent=horiVec+0.5f*vertVec;
        right.spline.SetLeftTangent(1,tangent);

        vecMidTo=rightleg-rightmid;
        horiVec=Vector3.Dot(vecMidTo,horizontalToRight)*horizontalToRight;
        vertVec=rightleg-(rightmid+horiVec);
        tangent=horiVec+0.5f*vertVec;
        right.spline.SetRightTangent(1,tangent);
    }
}
