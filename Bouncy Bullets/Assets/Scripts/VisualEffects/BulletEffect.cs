using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private Renderer bulletRenderer;
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Blink Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color warningColor = Color.white;
    [SerializeField] private float blinkSpeed = 15f;

    private MaterialPropertyBlock propertyBlock;
    private Color baseColor;

    private static readonly int ColorPropertyID = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        if (bulletRenderer == null)
            bulletRenderer = GetComponentInChildren<Renderer>();

        if (trailRenderer == null)
            trailRenderer = GetComponentInChildren<TrailRenderer>();

        if (bulletRenderer != null && bulletRenderer.sharedMaterial != null)
        {
            if (bulletRenderer.sharedMaterial.HasProperty(ColorPropertyID))
            {
                baseColor = bulletRenderer.sharedMaterial.GetColor(ColorPropertyID);
            }
            else
            {
                baseColor = bulletRenderer.sharedMaterial.color;
            }
        }
    }

    public void ResetVisuals()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }

        SetColor(baseColor);
    }

    public void AnimateBlink(float elapsedTime)
    {
        float lerpFactor = Mathf.PingPong(elapsedTime * blinkSpeed, 1f);
        Color currentColor = Color.Lerp(baseColor, warningColor, lerpFactor);
        SetColor(currentColor);
    }

    private void SetColor(Color color)
    {
        if (bulletRenderer == null) return;

        bulletRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorPropertyID, color);
        bulletRenderer.SetPropertyBlock(propertyBlock);
    }
}
