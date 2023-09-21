using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMove : MonoBehaviour
{
    public float totalDistance;
    public CinemachineVirtualCamera virtualCamera;
    private float lerp;
    public AnimationCurve moveCurve;
    public AnimationCurve scaleUpCurve;
    public float moveFix;
    public float scaleUpFix;
    private float targetSize;

    private void Awake()
    {
        totalDistance = 0f;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        targetSize=100+totalDistance/5f;
        virtualCamera.m_Lens.OrthographicSize=Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize,targetSize,Time.deltaTime*5);
    }
}
