using UnityEngine;
using System.Collections;

public class OnFire : State {

    /**
     * State for nodes that are on fire
     * Fire begins to spread in timeUntilFireSpread seconds
     * Node's state becomes "Dead" in timeUntilFireOut seconds from fire's start
     */
    static double timeUntilFireSpread = 3.0f;
    static double timeUntilFireOut = 10.0f;

    public OnFire(TendrilNode obj)
        : base(obj)
    {

    }

    public override void UpdateState(float deltaTime) {
        base.UpdateState(deltaTime);

        timeInState += deltaTime;
        Debug.Log("AAHHHH BURNING " + mOwner.GetPosition());
        if (timeInState > timeUntilFireSpread && timeInState < timeUntilFireOut)
        {
            foreach (TendrilNode neighbor in mOwner.GetNeighbors())
            {
                if (neighbor.mState.GetType() == typeof(Alive)) // if it's dead or burning, won't catch fire again
                {
                    neighbor.mState = new OnFire(neighbor);
                    Debug.Log("Fire Spreading to " + neighbor.GetPosition());
                }
            }
        }
        if (timeInState > timeUntilFireOut)
        {
            mOwner.mState = new Dead(mOwner);
        }
    }
}
