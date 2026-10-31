using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IntEventChannelSO", menuName = "Events/Int Event Channel")]
public class IntEventChannelSO : ScriptableObject
{
    private Action<int> OnEventRaised;

    public void Subscribe(Action<int> action) => OnEventRaised += action;
    public void Unsubscribe(Action<int> action) => OnEventRaised -= action;

    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}
