using UnityEngine;
using System.Collections;

public class TendrilRoot : TendrilNode
{
    public TendrilTip activeTip;

    protected override void Start()
    {
        base.Start();

        activeTip.growDirection = transform.up;
    }

    // input functions (called into by PlayerObject)
    public void StartBranch()
    {
        activeTip.StartBranch();
    }
    public void EndBranch()
    {
        activeTip.EndBranch();
    }
    public void BranchAim(Vector2 input)
    {
        activeTip.BranchAim(input);
    }

    public void CutTendril()
    {

    }
}
