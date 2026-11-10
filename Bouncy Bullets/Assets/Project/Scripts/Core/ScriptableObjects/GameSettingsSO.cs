using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "GameSettingsSO", menuName = "DataContainer/GameSettings")]
    public class GameSettingsSO : ScriptableObject
    {
        [Header("Defaults")]
        [SerializeField, Range(0f, 1f)] private float defaultVolume = 0.75f;
        [SerializeField] private float defaultSensitivity = 1f;

        [Header("Sensitivity Range")]
        [SerializeField, Min(0.01f)] private float minSensitivity = 0.1f;
        [SerializeField, Min(0.01f)] private float maxSensitivity = 3f;

        private const string MusicKey = "MusicVolume";
        private const string SfxKey = "SFXVolume";
        private const string SensitivityKey = "AimSensitivity";

        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }
        public float AimSensitivity { get; private set; }

        public float MinSensitivity => minSensitivity;
        public float MaxSensitivity => maxSensitivity;

        private void OnEnable()
        {
            MusicVolume = PlayerPrefs.GetFloat(MusicKey, defaultVolume);
            SfxVolume = PlayerPrefs.GetFloat(SfxKey, defaultVolume);
            AimSensitivity = ClampSensitivity(PlayerPrefs.GetFloat(SensitivityKey, defaultSensitivity));
        }

        private void OnValidate() => maxSensitivity = Mathf.Max(maxSensitivity, minSensitivity);

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
            AimSensitivity = ClampSensitivity(value);
            PlayerPrefs.SetFloat(SensitivityKey, AimSensitivity);
        }

        public void Save() => PlayerPrefs.Save();

        private float ClampSensitivity(float value) => Mathf.Clamp(value, minSensitivity, maxSensitivity);
    }
}
