using UnityEngine;
using System.Collections;

public class Dead : State {
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
        }
	}
}
