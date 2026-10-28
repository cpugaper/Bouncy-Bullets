using UnityEngine;
using UnityEngine.UI;

public class ShockwaveUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image[] chargeImages;

    [Header("Sprites")]
    [SerializeField] private Sprite activeSprite;    
    [SerializeField] private Sprite inactiveSprite;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO shockwaveChangedEventChannel;

    private void OnEnable()
    {
        if (shockwaveChangedEventChannel != null) shockwaveChangedEventChannel.Subscribe(UpdateUI);
    }

    private void OnDisable()
    {
        if (shockwaveChangedEventChannel != null) shockwaveChangedEventChannel.Unsubscribe(UpdateUI);
    }

    private void UpdateUI(int currentCharges)
    {
        for (int i = 0; i < chargeImages.Length; i++)
        {
            if (chargeImages[i] != null)
            {
                chargeImages[i].sprite = i < currentCharges ? activeSprite : inactiveSprite;
            }
        }
    }
}
