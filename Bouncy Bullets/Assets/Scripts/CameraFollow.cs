using UnityEngine;

[DefaultExecutionOrder(-50)]
public class CameraFollow : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float smoothTime = 0.2f;

    private Transform playerTransform;
    private float baseY;
    private Vector3 velocity;

    private void Awake() => baseY = transform.position.y;

    private void LateUpdate()
    {
        ScreenBounds bounds = ScreenBounds.Instance;
        if (bounds == null) return;

        if (playerTransform == null)
        {
            if (PlayerController.Instance == null) return;
            playerTransform = PlayerController.Instance.transform;
        }

        Vector3 desiredPos = bounds.GetCameraPositionFor(playerTransform.position, baseY);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
    }
}