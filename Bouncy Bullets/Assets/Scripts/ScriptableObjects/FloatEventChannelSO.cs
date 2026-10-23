using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatEventChannelSO", menuName = "Events/Float Event Channel")]
public class FloatEventChannelSO : ScriptableObject
{
    private Action<float> OnEventRaised;

    public void Subscribe(Action<float> action) => OnEventRaised += action;
    public void Unsubscribe(Action<float> action) => OnEventRaised -= action;

    public void RaiseEvent(float value)
    {
        OnEventRaised?.Invoke(value);
    }
}
