using UnityEngine;
using System.Collections;

public class OnFire : State {
    static double timeUntilFireSpread = 3.0f;
    static double timeUntilFireOut = 10.0f;

    public OnFire(TendrilNode obj)
        : base(obj)
    {

    }

    public override void UpdateState() {
        timeInState += Time.deltaTime;
        Debug.Log("AAHHHH BURNING " + mOwner.GetPosition());
        if (timeInState > timeUntilFireSpread && timeInState < timeUntilFireOut)
        {
            foreach (TendrilNode neighbor in mOwner.GetNeighbors())
            {
                if (neighbor.mState.GetType() == typeof(Alive)) // if it's dead or burning, won't catch fire again
                    neighbor.mState = new OnFire(neighbor);
            }
        }
        if (timeInState > timeUntilFireOut)
        {
            GameObject.Destroy(mOwner.gameObject);
        }
    }
}
