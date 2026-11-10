using UnityEngine;

namespace Game.Gameplay
{
    public class VolatileEffect : MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private Renderer enemyRenderer;
        [SerializeField] private Transform warningAreaQuad;

        [Header("Blink Settings")]
        [ColorUsage(true, true)]
        [SerializeField] private Color warningColor = Color.red;
        [SerializeField] private float blinkSpeed = 20f;

        private MaterialPropertyBlock propertyBlock;
        private Color originalColor;

        private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();

            if (enemyRenderer != null && enemyRenderer.sharedMaterial != null
                && enemyRenderer.sharedMaterial.HasProperty(BaseColorID))
            {
                originalColor = enemyRenderer.sharedMaterial.GetColor(BaseColorID);
            }
        }

        private void OnEnable()
        {
            if (warningAreaQuad != null) warningAreaQuad.localScale = Vector3.zero;
            SetColor(originalColor);
        }

        public void AnimateOverload(float elapsed, float progress, float explosionRadius)
        {
            float lerpFactor = Mathf.PingPong(elapsed * blinkSpeed, 1f);
            SetColor(Color.Lerp(originalColor, warningColor, lerpFactor));

            if (warningAreaQuad != null)
            {
                float quadSize = explosionRadius * 2f / 0.8f;
                Vector3 targetScale = new Vector3(quadSize, quadSize, 1f);
                warningAreaQuad.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);
            }
        }

        private void SetColor(Color color)
        {
            if (enemyRenderer == null) return;

            enemyRenderer.GetPropertyBlock(propertyBlock, 0);
            propertyBlock.SetColor(BaseColorID, color);
            enemyRenderer.SetPropertyBlock(propertyBlock, 0);
        }
    }
}
