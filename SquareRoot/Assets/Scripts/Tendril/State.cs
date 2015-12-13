using UnityEngine;
using System.Collections;

public class State {

    protected TendrilNode owner;
    protected float creationTime;

    protected float timeInState
    {
        get
        {
            return Time.time - creationTime;
        }
    }

    public State(TendrilNode obj)
    {
        owner = obj;
    }

    public virtual void OnStateEnter()
    {
        creationTime = Time.time;
    }

    // Gets called every frame by the owner node
    public virtual void UpdateState(float deltaTime) 
    {

    }

    public virtual void OnStateExit()
    {

    }
}
