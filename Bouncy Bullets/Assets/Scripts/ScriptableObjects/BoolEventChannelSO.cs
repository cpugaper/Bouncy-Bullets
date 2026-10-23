using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolEventChannelSO", menuName = "Events/Bool Event Channel")]
public class BoolEventChannelSO : ScriptableObject
{
    private Action<bool> OnEventRaised;

    public void Subscribe(Action<bool> action) => OnEventRaised += action;
    public void Unsubscribe(Action<bool> action) => OnEventRaised -= action;

    public void RaiseEvent(bool value)
    {
        OnEventRaised?.Invoke(value);
    }
}
