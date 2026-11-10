using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEventChannelSO", menuName = "Events/Game Event Channel")]
public class GameEventChannelSO : ScriptableObject
{
    private Action onEventRaised;

    public void Subscribe(Action listener) => onEventRaised += listener;
    public void Unsubscribe(Action listener) => onEventRaised -= listener;

    public void RaiseEvent() => onEventRaised?.Invoke();
}
