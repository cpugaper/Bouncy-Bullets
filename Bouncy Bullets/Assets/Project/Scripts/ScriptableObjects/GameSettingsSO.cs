using UnityEngine;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "DataContainer/GameSettings")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Defaults")]
    [SerializeField, Range(0f, 1f)] private float defaultVolume = 0.75f;
    [SerializeField] private float defaultSensitivity = 1f;

    private const string MusicKey = "MusicVolume";
    private const string SfxKey = "SFXVolume";
    private const string SensitivityKey = "MouseSensitivity";

    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public float MouseSensitivity { get; private set; }

    private void OnEnable()
    {
        MusicVolume = PlayerPrefs.GetFloat(MusicKey, defaultVolume);
        SfxVolume = PlayerPrefs.GetFloat(SfxKey, defaultVolume);
        MouseSensitivity = PlayerPrefs.GetFloat(SensitivityKey, defaultSensitivity);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        PlayerPrefs.SetFloat(MusicKey, value);
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = value;
        PlayerPrefs.SetFloat(SfxKey, value);
    }

    public void SetSensitivity(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat(SensitivityKey, value);
    }

    public void Save() => PlayerPrefs.Save();
}
