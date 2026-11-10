using UnityEngine;

namespace Game.Gameplay
{
    public class BomberEffect : MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private Renderer meshRenderer;

        [Header("Flash Settings")]
        [ColorUsage(true, true)]
        [SerializeField] private Color targetFlashColor = Color.white;

        private MaterialPropertyBlock propertyBlock;
        private Color originalColor;

        private static readonly int BaseColorID = Shader.PropertyToID("_Star_Base_Color");

        private void EnsureInitialized()
        {
            if (propertyBlock != null) return;

            propertyBlock = new MaterialPropertyBlock();
            if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();

            if (meshRenderer != null && meshRenderer.sharedMaterial != null)
                originalColor = meshRenderer.sharedMaterial.GetColor(BaseColorID);
        }

        private void OnEnable() => SetColor(originalColor);

        public void SetFlashProgress(float progress)
        {
            SetColor(Color.Lerp(originalColor, targetFlashColor, progress));
        }

        private void SetColor(Color color)
        {
            EnsureInitialized();
            if (meshRenderer == null) return;

            meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorID, color);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
