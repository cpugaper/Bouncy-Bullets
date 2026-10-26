using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEventChannelSO", menuName = "Events/Game Event Channel")]
public class GameEventChannelSO : ScriptableObject
{
    private Action OnEventRaised;

    public void Subscribe(Action listener) => OnEventRaised += listener;
    public void Unsubscribe(Action listener) => OnEventRaised -= listener;

    public void RaiseEvent() => OnEventRaised?.Invoke();
}
