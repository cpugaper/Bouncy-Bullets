using UnityEngine;
using UnityEngine.Audio;
using Game.Core;

namespace Game.Gameplay
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private AudioSource backgroundMusicSource;
        [SerializeField] private AudioSource shieldMusicSource;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string musicVolumeParam = "MusicVolume";
        [SerializeField] private string sfxVolumeParam = "SFXVolume";

        [Header("Music Audio Clips")]
        [SerializeField] private AudioClip shieldActiveMusic;
        [SerializeField] private AudioClip backgroundMusic;

        [Header("Gameplay Audio Clips")]
        [SerializeField] private AudioClip enemyDeathSFX;
        [SerializeField] private AudioClip playerDeathSFX;
        [SerializeField] private AudioClip shootSFX;
        [SerializeField] private AudioClip turboSFX;
        [SerializeField] private AudioClip shockwaveChargeSFX;

        [Header("Pickup Audio Clips")]
        [SerializeField] private AudioClip energyPickupSFX;
        [SerializeField] private AudioClip scorePickupSFX;
        [SerializeField] private AudioClip shockwavePickupSFX;

        [Header("Enemy Audio Clips")]
        [SerializeField] private AudioClip shooterShootSFX;
        [SerializeField] private AudioClip volatileChargeSFX;

        [Header("UI Audio Clips")]
        [SerializeField] private AudioClip uiClickSFX;
        [SerializeField] private AudioClip uiHoverSFX;

        [Header("Event Channels")]
        [SerializeField] private PositionEventChannelSO enemyDeathEventChannel;
        [SerializeField] private PositionEventChannelSO playerDeathEventChannel;
        [SerializeField] private GameEventChannelSO shootEventChannel;
        [SerializeField] private GameEventChannelSO shieldActivatedEventChannel;
        [SerializeField] private GameEventChannelSO shieldDeactivatedEventChannel;

        [Header("Settings Event Channels")]
        [SerializeField] private FloatEventChannelSO musicVolumeChangedEventChannel;
        [SerializeField] private FloatEventChannelSO sfxVolumeChangedEventChannel;
        [SerializeField] private GameSettingsSO settings;

        private bool shieldWasPlayingBeforePause = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            if (enemyDeathEventChannel != null) enemyDeathEventChannel.Subscribe(PlayEnemyDeath);
            if (playerDeathEventChannel != null) playerDeathEventChannel.Subscribe(PlayPlayerDeath);
            if (shootEventChannel != null) shootEventChannel.Subscribe(PlayShoot);
            if (shieldActivatedEventChannel != null) shieldActivatedEventChannel.Subscribe(PlayShieldMusic);
            if (shieldDeactivatedEventChannel != null) shieldDeactivatedEventChannel.Subscribe(StopShieldMusic);
            if (musicVolumeChangedEventChannel != null) musicVolumeChangedEventChannel.Subscribe(SetMusicVolume);
            if (sfxVolumeChangedEventChannel != null) sfxVolumeChangedEventChannel.Subscribe(SetSFXVolume);
        }

        private void OnDisable()
        {
            if (enemyDeathEventChannel != null) enemyDeathEventChannel.Unsubscribe(PlayEnemyDeath);
            if (playerDeathEventChannel != null) playerDeathEventChannel.Unsubscribe(PlayPlayerDeath);
            if (shootEventChannel != null) shootEventChannel.Unsubscribe(PlayShoot);
            if (shieldActivatedEventChannel != null) shieldActivatedEventChannel.Unsubscribe(PlayShieldMusic);
            if (shieldDeactivatedEventChannel != null) shieldDeactivatedEventChannel.Unsubscribe(StopShieldMusic);
            if (musicVolumeChangedEventChannel != null) musicVolumeChangedEventChannel.Unsubscribe(SetMusicVolume);
            if (sfxVolumeChangedEventChannel != null) sfxVolumeChangedEventChannel.Unsubscribe(SetSFXVolume);
        }

        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            HandleGameStateChanged(GameManager.Instance.CurrentState);

            ApplySavedAudioSettings();

            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();

            shieldMusicSource.clip = shieldActiveMusic;
            shieldMusicSource.loop = true;
        }

        public void ApplySavedAudioSettings()
        {
            SetMusicVolume(settings.MusicVolume);
            SetSFXVolume(settings.SfxVolume);
        }

        public void SetMusicVolume(float value) => SetMixerVolume(musicVolumeParam, value);
        public void SetSFXVolume(float value) => SetMixerVolume(sfxVolumeParam, value);

        private void SetMixerVolume(string paramName, float value)
        {
            if (audioMixer == null) return;
            float dB = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
            audioMixer.SetFloat(paramName, dB);
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Paused)
            {
                if (sfxAudioSource != null && sfxAudioSource.isPlaying)
                {
                    sfxAudioSource.Pause();
                }

                shieldWasPlayingBeforePause = shieldMusicSource.isPlaying;

                if (shieldWasPlayingBeforePause)
                {
                    shieldMusicSource.Pause();
                }
                else if (backgroundMusicSource.isPlaying)
                {
                    backgroundMusicSource.Pause();
                }
            }
            else if (state == GameState.Playing)
            {
                if (sfxAudioSource != null)
                {
                    sfxAudioSource.UnPause();
                }

                if (shieldWasPlayingBeforePause)
                {
                    shieldMusicSource.UnPause();
                }
                else
                {
                    backgroundMusicSource.UnPause();
                }
            }
        }

        public void PlayEnemyDeath(Vector3 position) => PlaySFX(enemyDeathSFX);
        public void PlayPlayerDeath(Vector3 position)
        {
            StopAllSFX();
            StopShieldMusic();
            PlaySFX(playerDeathSFX);
        }
        public void PlayShoot() => PlaySFX(shootSFX);
        public void PlayTurbo() => PlaySFX(turboSFX);
        public void PlayShockwaveCharge() => PlaySFX(shockwaveChargeSFX);

        public void PlayEnergyPickup() => PlaySFX(energyPickupSFX);
        public void PlayScorePickup() => PlaySFX(scorePickupSFX);
        public void PlayShockwavePickup() => PlaySFX(shockwavePickupSFX);

        public void PlayUIClick() => PlayUISFX(uiClickSFX);
        public void PlayUIHover() => PlayUISFX(uiHoverSFX);

        public void PlayShooterShoot() => PlaySFX(shooterShootSFX);
        public void PlayVolatileCharge() => PlaySFX(volatileChargeSFX);

        public void PlayShieldMusic()
        {
            backgroundMusicSource.Pause();
            shieldMusicSource.Play();
        }

        public void StopShieldMusic()
        {
            shieldMusicSource.Stop();
            backgroundMusicSource.UnPause();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxAudioSource != null)
                sfxAudioSource.PlayOneShot(clip);
        }

        public void PlayUISFX(AudioClip clip)
        {
            if (clip != null && uiAudioSource != null)
                uiAudioSource.PlayOneShot(clip);
        }

        public void StopAllSFX()
        {
            if (sfxAudioSource != null)
                sfxAudioSource.Stop();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

            AudioListener.pause = false;
        }
    }
}
