using UnityEngine;

namespace Game.Gameplay
{
    public class MoveBackground : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        [Header("Parallax Settings")]
        [SerializeField][Range(0f, 1f)] private float parallaxFactor = 0.15f;

        private Material backgroundMaterial;
        private int texturePropertyID;
        private Vector2 uvPerWorldUnit;

        private static readonly int BaseMapID = Shader.PropertyToID("_BaseMap");
        private static readonly int MainTexID = Shader.PropertyToID("_MainTex");

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;

            if (!TryGetComponent(out MeshRenderer meshRenderer)) return;

            backgroundMaterial = meshRenderer.material;
            texturePropertyID = backgroundMaterial.HasProperty(BaseMapID) ? BaseMapID : MainTexID;

            Vector2 tiling = backgroundMaterial.GetTextureScale(texturePropertyID);
            Vector3 size = meshRenderer.bounds.size;
            uvPerWorldUnit = new Vector2(tiling.x / size.x, tiling.y / size.z);
        }

        private void LateUpdate()
        {
            if (cameraTransform == null || backgroundMaterial == null) return;

            Vector3 cam = cameraTransform.position;
            transform.position = new Vector3(cam.x, transform.position.y, cam.z);

            // Moves the texture in the oposite direction of the camera
            Vector2 offset = -new Vector2(cam.x * uvPerWorldUnit.x, cam.z * uvPerWorldUnit.y) * parallaxFactor;
            backgroundMaterial.SetTextureOffset(texturePropertyID, offset);
        }

        private void OnDestroy()
        {
            if (backgroundMaterial != null) Destroy(backgroundMaterial);
        }
    }
}
