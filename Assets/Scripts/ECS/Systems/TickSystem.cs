using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Linq;

public class TickSystem : ComponentSystem
{
    public int TickCounter = 0;

    private float TickLength = 1;
    private float TickTimer = 0;

    private Dictionary<int,List<ITickListener>> TickListenersPerPriority;
    private List<ITickListener> TickListeners;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Enabled = GameStateSystem.Instance != null && GameStateSystem.Instance.IsPlaying;
    }

    protected override void OnUpdate()
    {
        if (TickTimer < TickLength)
        {
            TickTimer += Time.deltaTime;
        }
        else
        {
            TickTimer = 0;
            TickCounter++;

            foreach(KeyValuePair<int,List<ITickListener>> TickListeners in TickListenersPerPriority.Reverse())
            {
                List<ITickListener> TickListenersToNotify = new List<ITickListener>(TickListeners.Value);
                TickListenersToNotify.ForEach(x => x.OnTick(TickCounter));
            }
        }
    }

    public void Restart()
    {
        TickCounter = 0;
    }

    public void AssignTickListener(ITickListener listener,int priority = 0)
    {
        if (TickListenersPerPriority == null)
            TickListenersPerPriority = new Dictionary<int, List<ITickListener>>();
        if (!TickListenersPerPriority.ContainsKey(priority))
            TickListenersPerPriority.Add(priority, new List<ITickListener>());

        UnassignTickListener(listener);

        TickListenersPerPriority[priority].Add(listener);
    }

    public void UnassignTickListener(ITickListener listener)
    {
        foreach (KeyValuePair<int, List<ITickListener>> TickListeners in TickListenersPerPriority)
        { 
            if (TickListeners.Value.Count == 0) continue;
            TickListeners.Value.Remove(listener);
        }
    }
}
