using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase_", menuName = "DataContainer/PhaseConfig")]
public class PhaseConfig : ScriptableObject
{
    [Serializable]
    public struct EnemyEntry
    {
        public EnemyType type;
        [Min(0f)] public float weight;
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
        float total = 0f;
        foreach (EnemyEntry entry in enemies) total += entry.weight;

        float roll = UnityEngine.Random.Range(0f, total);
        foreach (EnemyEntry entry in enemies)
        {
            roll -= entry.weight;
            if (roll <= 0f) return entry.type;
        }
        return enemies[enemies.Length - 1].type;
    }

    private void OnValidate()
    {
        if (enemies == null || enemies.Length == 0)
        {
            Debug.LogWarning($"{name}: does not have enemies configured.", this);
        }
    }
}
