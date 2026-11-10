using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhaseManager : MonoBehaviour
{
    [Header("Phases")]
    [SerializeField] private PhaseConfig[] phases;
    [SerializeField, Min(0f)] private float transitionDelay = 2.5f;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO phaseChangedEventChannel;
    [SerializeField] private PositionEventChannelSO playerDeathEventChannel;
    [SerializeField] private GameEventChannelSO runStartedEventChannel;

    public bool IsSpawning { get; private set; }
    public PhaseConfig CurrentPhase { get; private set; }
    public float CurrentInterval { get; private set; }
    public static int CurrentPhaseNumber { get; private set; } = 1;


    private Coroutine phasesCoroutine;

    private void OnEnable()
    {
        if (playerDeathEventChannel != null) playerDeathEventChannel.Subscribe(OnPlayerDeath);
        if (runStartedEventChannel != null) runStartedEventChannel.Subscribe(HandleRunStarted);
    }

    private void OnDisable()
    {
        if (playerDeathEventChannel != null) playerDeathEventChannel.Unsubscribe(OnPlayerDeath);
        if (runStartedEventChannel != null) runStartedEventChannel.Unsubscribe(HandleRunStarted);
    }

    private void HandleRunStarted()
    {
        if (phases.Length == 0) return;
        if (phasesCoroutine != null) StopCoroutine(phasesCoroutine);

        int savedIndex = SaveManager.Instance.SavedPhaseIndex;
        int startIndex = Mathf.Clamp(savedIndex, 0, phases.Length - 1);  

        phasesCoroutine = StartCoroutine(RunPhases(startIndex));
    }

    private void OnPlayerDeath(Vector3 position)
    {
        IsSpawning = false;
        if (phasesCoroutine != null)
        {
            StopCoroutine(phasesCoroutine);
            phasesCoroutine = null;
        }
    }

    private IEnumerator RunPhases(int startIndex)
    {
        for (int i = startIndex; i < phases.Length; i++)
        {
            yield return null; 
            
            // The display phase number (1-based), NOT the array index
            CurrentPhaseNumber = i + 1;

            PhaseConfig phase = phases[i];
            CurrentPhase = phase;
            CurrentInterval = phase.GetInterval(0f);
            IsSpawning = false;

         
            if (phaseChangedEventChannel != null)
                phaseChangedEventChannel.RaiseEvent(CurrentPhaseNumber);

            yield return new WaitForSeconds(transitionDelay);

            IsSpawning = true;
            float elapsed = 0f;

            while (phase.IsEndless || elapsed < phase.Duration)
            {
                elapsed += Time.deltaTime;
                CurrentInterval = phase.GetInterval(elapsed);
                yield return null; 
            }
        }
    }
}
