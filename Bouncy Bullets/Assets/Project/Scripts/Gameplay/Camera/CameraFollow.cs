using Game.Core;
using UnityEngine;

namespace Game.Gameplay
{
    [DefaultExecutionOrder(-50)]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float smoothTime = 0.2f;

        [Header("References")]
        [SerializeField] private TransformReferenceSO playerReference;

        private float baseY;
        private Vector3 velocity;

        private void Awake() => baseY = transform.position.y;

        private void LateUpdate()
        {
            Transform player = playerReference.Value;
            if (player == null) return;

            Vector3 desiredPos = ScreenBounds.Instance.GetCameraPositionFor(player.position, baseY);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);
        }
    }
}