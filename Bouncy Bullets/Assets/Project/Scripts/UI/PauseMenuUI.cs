using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider; 
    [SerializeField] private Slider sfxSlider; 
    [SerializeField] private Slider sensitivitySlider;

    [Header("Event Channels")]
    [SerializeField] private FloatEventChannelSO musicVolumeChangedEventChannel;
    [SerializeField] private FloatEventChannelSO sfxVolumeChangedEventChannel;

    [SerializeField] private GameSettingsSO settings;

    private void OnEnable()
    {
        sensitivitySlider.minValue = settings.MinSensitivity;
        sensitivitySlider.maxValue = settings.MaxSensitivity;

        musicSlider.SetValueWithoutNotify(settings.MusicVolume);
        sfxSlider.SetValueWithoutNotify(settings.SfxVolume);
        sensitivitySlider.SetValueWithoutNotify(settings.MouseSensitivity);

        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnDisable()
    {
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);

        settings.Save();
    }

    private void OnMusicChanged(float value)
    {
        settings.SetMusicVolume(value);
        if (musicVolumeChangedEventChannel != null) musicVolumeChangedEventChannel.RaiseEvent(value);
    }

    private void OnSfxChanged(float value)
    {
        settings.SetSfxVolume(value);
        if (sfxVolumeChangedEventChannel != null) sfxVolumeChangedEventChannel.RaiseEvent(value);
    }

    private void OnSensitivityChanged(float v) => settings.SetSensitivity(v);

}
