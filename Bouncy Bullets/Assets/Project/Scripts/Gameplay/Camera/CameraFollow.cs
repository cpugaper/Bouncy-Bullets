using UnityEngine;

namespace Game.Gameplay
{
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
            if (playerTransform == null)
            {
                if (PlayerMovement.Instance == null) return;
                playerTransform = PlayerMovement.Instance.transform;
            }

            Vector3 desiredPos = ScreenBounds.Instance.GetCameraPositionFor(playerTransform.position, baseY);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
        }
    }
}