using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    
    
    [SerializeField] 
    private Vector2 depthRange = new Vector2(0,1);
    
    [SerializeField]
    private AnimationCurve depthMin;
    [SerializeField]
    private AnimationCurve depthMax;
    
    
    [SerializeField]
    private AnimationCurve rotationMin;
    [SerializeField]
    private AnimationCurve rotationMax;
    
    [SerializeField]
    private AnimationCurve distanceMin;
    [SerializeField]
    private AnimationCurve distanceMax;

    
    
    [SerializeField]
    private AnimationCurve curveRotation;
    [SerializeField]
    private AnimationCurve curveDistance;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            var z = target.transform.position.z;
            var x = target.transform.position.x;
            var maxZ = depthMax.Evaluate(x);
            var minZ = depthMin.Evaluate(x);
            var depth = maxZ - minZ;
            var percent = (z - minZ) / depth;
            
            
            var distanceRange = distanceMax.Evaluate(x) - distanceMin.Evaluate(x);
            var rotationRange = rotationMax.Evaluate(x) - rotationMin.Evaluate(x);

            var distance = Mathf.Lerp( distanceMin.Evaluate(x),distanceMax.Evaluate(x),curveDistance.Evaluate(percent));
            var rotation = Mathf.Lerp( rotationMin.Evaluate(x),rotationMax.Evaluate(x),curveRotation.Evaluate(percent));

            
            transform.rotation = Quaternion.Euler(rotation,0,0);
            var cinemachineFramingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
            cinemachineFramingTransposer.m_CameraDistance = distance;
        }



    }
}
