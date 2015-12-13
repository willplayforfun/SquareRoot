using UnityEngine;
using System.Collections;

public class Dead : State {
    /**
     * State of dying nodes
     * Count down until decompose, at which point
     * children nodes have their state set to "Dead"
     * and this node is destroyed
     */
    static float timeUntilDecompose = 4.0f;

    public Dead(TendrilNode obj)
        : base(obj)
    {

    }
	
	public override void UpdateState (float deltaTime) {
        base.UpdateState(deltaTime);
        if (timeInState > timeUntilDecompose)
        {
            /*Todo*/
            foreach (TendrilNode t in mOwner.GetChildren())
            {
                //t.mState = new Dead(t);
            }
            //GameObject.Destroy(mOwner);
        }
	}
}
