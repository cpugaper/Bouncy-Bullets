using UnityEngine;

public class PlayerTilt : MonoBehaviour
{
    public Transform modelTransform; 
    [SerializeField] private float maxTiltAngle = 15f;
    [SerializeField] private float tiltSpeed = 10f; 

    public void ApplyTilt(Vector3 worldMoveDir)
    {
        if (modelTransform == null) return;
        Vector3 localMoveDir = transform.InverseTransformDirection(worldMoveDir);

        float targetZAngle = -localMoveDir.x * maxTiltAngle;
        float targetXAngle = localMoveDir.z * maxTiltAngle;

        Quaternion targetRotation = Quaternion.Euler(targetXAngle, 0f, targetZAngle);

        modelTransform.localRotation = Quaternion.Slerp(modelTransform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }
}
