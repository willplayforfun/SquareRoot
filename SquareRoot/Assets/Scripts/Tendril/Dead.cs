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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame in mOwner
	public override void UpdateState () {
        base.UpdateState();
        if (timeInState > timeUntilDecompose)
        {
            /*Todo*/
            foreach (TendrilNode t in mOwner.GetChildren())
            {
                t.mState = new Dead(t);
            }
            GameObject.Destroy(mOwner);
        }
	}
}
