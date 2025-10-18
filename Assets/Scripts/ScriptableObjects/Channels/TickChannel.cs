using System;
using System.Collections.Generic;
using EchoesOfAetherion.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "TickChannel", menuName = "Scriptable Objects/Channels/TickChannel")]
public class TickChannel : ScriptableObject
{
    private IList<ITickable> tickables;

    public void Register(ITickable tickable)
    {
        if (tickables == null) tickables = new List<ITickable>();

        if (!tickables.Contains(tickable))
            tickables.Add(tickable);
    }

    public void UnRegister(ITickable tickable)
    {
        if (tickables == null) tickables = new List<ITickable>();

        if (tickables.Contains(tickable))
            tickables.Remove(tickable);
    }

    public void UpdateTickables()
    {
        foreach (ITickable tickable in tickables)
            tickable.Tick();
    }

    public void FixedUpdateTickables()
    {
        foreach (ITickable tickable in tickables)
            tickable.FixedTick();
    }
}
