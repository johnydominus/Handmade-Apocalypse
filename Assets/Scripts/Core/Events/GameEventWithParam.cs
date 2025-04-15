using System;
using UnityEngine;

public class GameEventWithParam<T>
{
    private event Action<T> listeners;

    public void RegisterListener(Action<T> listener) => listeners += listener;
    public void UnregisterListener(Action<T> listener) => listeners -= listener;
    public void Raise(T param) => listeners?.Invoke(param);
}
