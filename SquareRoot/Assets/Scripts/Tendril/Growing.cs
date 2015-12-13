using UnityEngine;

public class Growing : State
{
    // reference to cast owner
    private TendrilTip ownerTip;
    
    public float timeSinceNodeDropped;

    public Growing(TendrilNode obj) : base(obj)
    {
        ownerTip = ((TendrilTip)owner);
    }
    public override void OnStateEnter()
    {
        base.OnStateEnter();

        owner.GetComponent<BoxCollider2D>().enabled = false;
        owner.GetComponent<CircleCollider2D>().enabled = false;
    }
    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);

        float growthRate = ownerTip.growthRate;
        float nodeDropRate = ownerTip.nodeDropRate;
        Vector2 growDirection = ownerTip.growDirection;

        // one-time collision re-enable
        if (timeInState > 0.5f * 1f / nodeDropRate)
        {
            owner.GetComponent<BoxCollider2D>().enabled = true;
            owner.GetComponent<CircleCollider2D>().enabled = true;
        }

        // grow
        timeSinceNodeDropped += deltaTime;
        owner.transform.position += (Vector3)(growthRate * deltaTime * growDirection);

        // update collider
        owner.GetComponent<BoxCollider2D>().size = new Vector2(1, timeSinceNodeDropped * growthRate);
        owner.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.5f * timeSinceNodeDropped * growthRate);

        // if reached nodeDropRate distance since last node, make a new node
        if (nodeDropRate - timeSinceNodeDropped * growthRate <= 0)
        {
            timeSinceNodeDropped = 0;
            ownerTip.CreateNewNode();
        }
    }
}
