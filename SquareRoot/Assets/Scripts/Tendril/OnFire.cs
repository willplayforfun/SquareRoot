using UnityEngine;
using System.Collections;

public class OnFire : State {

    /**
     * State for nodes that are on fire
     * Fire begins to spread in timeUntilFireSpread seconds
     * Node's state becomes "Dead" in timeUntilFireOut seconds from fire's start
     */
    static float timeUntilFireSpread = 1.0f;
    static float timeUntilFireOut = 6.0f;

    private bool haveSpreadFire;

    public OnFire(TendrilNode obj)
           : base(obj)
    {

    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        owner.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);

        //Debug.Log("AAHHHH BURNING " + owner.GetPosition());

        // spread fire once after a time
        if (timeInState > timeUntilFireSpread && !haveSpreadFire)
        {
            haveSpreadFire = true;

            // spread to all neighbors, up or down
            foreach (TendrilNode neighbor in owner.GetNeighbors())
            {
                // if it's dead or burning, won't catch fire again
                neighbor.CatchFire();
                Debug.Log("Fire Spreading to " + neighbor.GetPosition());
            }
        }

        // stop burning
        if (timeInState > timeUntilFireOut)
        {
            owner.Die();
        }
    }
}
