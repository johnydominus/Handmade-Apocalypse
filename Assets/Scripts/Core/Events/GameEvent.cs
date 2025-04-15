using System;
using UnityEngine;

public class GameEvent
{
    private event Action listeners;

    public void RegisterListener(Action listener) => listeners += listener;
    public void UnregisterListener(Action listener) => listeners -= listener;
    public void Raise() => listeners?.Invoke();
}
