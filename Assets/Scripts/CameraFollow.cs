using Mirror;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _target;
    public Vector3 offset;
    public float smoothSpeed;

    public void SetTarget(Transform target)
    {
        _target = target;
    }
    
    void FixedUpdate()
    {
        if (!_target)
        {
            return;
        }
        var targetPosition = _target.position;
        var desiredPosition = targetPosition + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(targetPosition);
    }
}
