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

    void LateUpdate()
    {
        lefthead=headleft.position-left.gameObject.transform.position;
        righthead=headright.position-right.gameObject.transform.position;

        leftleg=legleft.position-left.gameObject.transform.position;
        rightleg=legright.position-right.gameObject.transform.position;

        leftmid=(lefthead+leftleg)/2;
        rightmid=(righthead+rightleg)/2;

        lefthead.Scale(new Vector3(1.0f/left.transform.localScale.x,1.0f/left.transform.localScale.y,1.0f/left.transform.localScale.z));
        righthead.Scale(new Vector3(1.0f/right.transform.localScale.x,1.0f/right.transform.localScale.y,1.0f/right.transform.localScale.z));
        leftmid.Scale(new Vector3(1.0f/left.transform.localScale.x,1.0f/left.transform.localScale.y,1.0f/left.transform.localScale.z));
        rightmid.Scale(new Vector3(1.0f/right.transform.localScale.x,1.0f/right.transform.localScale.y,1.0f/right.transform.localScale.z));
        leftleg.Scale(new Vector3(1.0f/left.transform.localScale.x,1.0f/left.transform.localScale.y,1.0f/left.transform.localScale.z));
        rightleg.Scale(new Vector3(1.0f/right.transform.localScale.x,1.0f/right.transform.localScale.y,1.0f/right.transform.localScale.z));

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
