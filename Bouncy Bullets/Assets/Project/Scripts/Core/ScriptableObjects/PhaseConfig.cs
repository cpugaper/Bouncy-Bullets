using System;
using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "Phase_", menuName = "DataContainer/PhaseConfig")]
    public class PhaseConfig : ScriptableObject
    {
        [Serializable]
        private struct EnemyEntry
        {
            public EnemyType Type;
            [Min(0f)] public float Weight;
        }

        [Header("Timing")]
        [SerializeField, Min(1f)] private float duration = 30f;
        [SerializeField, Min(0.1f)] private float startInterval = 4f;
        [SerializeField, Min(0.1f)] private float endInterval = 3.2f;

        [Header("Mode")]
        [Tooltip("If it is active, the phase does not end: after 'duration' the interval remains at 'endInterval'.")]
        [SerializeField] private bool isEndless;

        [Header("Enemies")]
        [SerializeField] private EnemyEntry[] enemies;

        public float Duration => duration;
        public bool IsEndless => isEndless;

        public float GetInterval(float elapsedSeconds)
        {
            float t = Mathf.Clamp01(elapsedSeconds / duration);
            return Mathf.Lerp(startInterval, endInterval, t);
        }

        public EnemyType PickEnemy()
        {
            if (enemies == null || enemies.Length == 0) return default;

            float total = 0f;
            foreach (EnemyEntry entry in enemies) total += entry.Weight;

            float roll = UnityEngine.Random.Range(0f, total);
            foreach (EnemyEntry entry in enemies)
            {
                roll -= entry.Weight;
                if (roll <= 0f) return entry.Type;
            }
            return enemies[^1].Type;
        }

        private void OnValidate()
        {
            if (enemies == null || enemies.Length == 0)
            {
                Debug.LogWarning($"{name}: does not have enemies configured.", this);
            }
        }
    }
}
