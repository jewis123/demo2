
using Cinemachine;
using UnityEngine;


public class CameraTeamLook : MonoBehaviour
{
    public Transform lookTarget;
    public float minOffset;
    public float minDis;
    public float maxOffset;
    public float maxDis;
    private FramingTarget2 transposer;
    public bool isLook;


    private void Awake()
    {
        var cam = GetComponent<CinemachineVirtualCamera>();
        if (cam)
        {
           transposer = cam.GetCinemachineComponent<FramingTarget2>();
        }
    }

    private void Update()
    {
        if (lookTarget == null) 
        {
            return;
        }


        if (isLook)
        {
            transform.LookAt(lookTarget);
        }

        var tarXZ = new Vector2(lookTarget.position.x, lookTarget.position.z);
        var camXZ = new Vector2(transform.position.x, transform.position.z);
        float distance = Vector2.Distance(tarXZ, camXZ);
        
        float newDis = 0 ;
        if (distance <= minDis)
        {
            newDis = minOffset;
        }

        if (distance > maxDis)
        {
            newDis = maxOffset;
        }

        if (distance > minDis && distance <= maxDis)
        {
            float percent = (float)(distance - minDis) / (maxDis - minDis);
            newDis = (minOffset + (maxOffset - minOffset) * percent);
        }

        transposer.m_CameraDistance = newDis;
    }
    
}