using UnityEngine;

public class Growing : State
{
    public Growing(TendrilNode obj) : base(obj)
    {

    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);

        float growthRate = ((TendrilTip)mOwner).growthRate;
        float timeSinceNodeDropped = ((TendrilTip)mOwner).timeSinceNodeDropped;
        float nodeDropRate = ((TendrilTip)mOwner).nodeDropRate;
        Vector2 growDirection = ((TendrilTip)mOwner).growDirection;

        // grow
        ((TendrilTip)mOwner).timeSinceNodeDropped += deltaTime;
        mOwner.transform.position += (Vector3)(growthRate * deltaTime * growDirection);

        // update collider
        mOwner.GetComponent<BoxCollider2D>().size = new Vector2(1, timeSinceNodeDropped * growthRate);
        mOwner.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.5f * timeSinceNodeDropped * growthRate);

        // if reached nodeDropRate distance since last node, make a new node
        if (nodeDropRate - timeSinceNodeDropped * growthRate <= 0)
        {
            ((TendrilTip)mOwner).CreateNewNode();
        }
    }
}
