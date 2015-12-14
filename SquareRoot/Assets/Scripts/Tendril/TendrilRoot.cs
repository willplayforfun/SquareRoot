using UnityEngine;
using System.Collections;

public class TendrilRoot : TendrilNode
{
    public TendrilTip activeTip;
    public PlayerObject player;

    protected override void Start()
    {
        base.Start();
        
        activeTip.growDirection = transform.up;
        activeTip.SetParent(this);
        AddChild(activeTip);

        onFireIndicator.SetActive(false);
    }

    public override void AddResources(float amount)
    {
        player.AddResources(amount);
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
        Die();
        onFireIndicator.SetActive(false);
    }

    public override void CatchFire()
    {
        base.CatchFire();
        player.Lose();
    }

    public GameObject onFireIndicator;

    public void TipCaughtFire()
    {
        onFireIndicator.SetActive(true);
    }
}
