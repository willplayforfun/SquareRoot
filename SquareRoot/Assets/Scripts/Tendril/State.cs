using UnityEngine;
using System.Collections;

public class State {

    protected TendrilNode mOwner;
    protected float timeInState = 0;
    public State(TendrilNode obj)
    {
        mOwner = obj;
    }

    public virtual void UpdateState() // Gets called every frame by the owner node
    {
        timeInState += Time.deltaTime;
    }
}
