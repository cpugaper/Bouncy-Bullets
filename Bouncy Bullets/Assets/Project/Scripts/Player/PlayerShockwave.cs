using System.Collections;
using UnityEngine;

public class PlayerShockwave : MonoBehaviour
{
    [Header("Shockwave Settings")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private float explosionRadius = 8f;
    [SerializeField] private LayerMask destructionMask;
    [SerializeField] private float expansionDuration = 0.2f;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO shockwaveChangedEventChannel; 

    [Header("Visual Reference")]
    [SerializeField] private Transform shockwaveVisualQuad;

    private InputSystem_Actions controls;
    private int currentCharges = 0;
    private bool isExploding = false;

    public bool HasCharges => currentCharges > 0;
    public bool IsFull => currentCharges >= maxCharges;

    private void Awake()
    {
        controls = new InputSystem_Actions();
    }

    private void Start()
    {
        NotifyUI();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

        if (HasCharges && !isExploding && controls.Player.ShockWave.WasPressedThisFrame())
        {
            UseShockwave();
        }
    }

    public bool TryAddShockwave()
    {
        if (currentCharges >= maxCharges) return false;

        currentCharges++;
        NotifyUI();
        return true;
    }

    private void UseShockwave()
    {
        currentCharges--;
        NotifyUI(); 
        StartCoroutine(ShockwaveSequence());
    }

    private void NotifyUI()
    {
        if (shockwaveChangedEventChannel != null)
        {
            shockwaveChangedEventChannel.RaiseEvent(currentCharges);
        }
    }

    private IEnumerator ShockwaveSequence()
    {
        isExploding = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayShockwaveCharge();
        }

        float timer = 0f;
        while (timer < expansionDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / expansionDuration;

            if (shockwaveVisualQuad != null)
            {
                float quadSize = Mathf.Lerp(0f, explosionRadius * 2f, progress);
                shockwaveVisualQuad.localScale = new Vector3(quadSize, quadSize, 1f);
            }

            yield return null;
        }

        Detonate();
    }

    private void Detonate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, destructionMask);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            if (hit.TryGetComponent(out Enemy enemyHit))
            {
                enemyHit.Die();
            }
            else if (hit.TryGetComponent(out BouncyBullet bullet))
            {
                bullet.ReturnToPool();
            }
            else if (hit.TryGetComponent(out HomingBullet enemyBullet)) 
            {
                enemyBullet.ReturnToPool();
            }
        }

        if (shockwaveVisualQuad != null) shockwaveVisualQuad.localScale = Vector3.zero;
        isExploding = false;
    }
}
