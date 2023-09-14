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
        if(totalDistance != 0f)
        {
            targetSize = 100f + totalDistance / 5f;
            //virtualCamera.m_Lens.OrthographicSize = targetSize;
            if(totalDistance / 5f > 16f)
            {
                StartCoroutine(move());
            }
            else
            {
                virtualCamera.m_Lens.OrthographicSize = 100f;
            }
            totalDistance = 0f;
        }
    }

    IEnumerator move()
    {
        while (virtualCamera.m_Lens.OrthographicSize < targetSize)
        {
            lerp = (virtualCamera.m_Lens.OrthographicSize - 100f) / (targetSize - 100f); //value in 0~1
            virtualCamera.m_Lens.OrthographicSize += (scaleUpCurve.Evaluate(lerp)+0.01f) * scaleUpFix;
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = targetSize;
        yield return null;

        float length = virtualCamera.m_Lens.OrthographicSize - 100f;
        while (virtualCamera.m_Lens.OrthographicSize > 100f)
        {
            lerp = virtualCamera.m_Lens.OrthographicSize - 100f / length; //value in 0~1
            virtualCamera.m_Lens.OrthographicSize -= moveCurve.Evaluate(lerp) * moveFix;
            yield return null;
        }
        virtualCamera.m_Lens.OrthographicSize = 100f;
    }
}
