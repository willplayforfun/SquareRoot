using UnityEngine;

public class Leeching : State
{
    ResourcePool pool;

    public Leeching(TendrilNode obj, GameObject foodPool)
             : base(obj)
    {
        pool = foodPool.GetComponent<ResourcePool>();
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        if(pool != null)
        {
            pool.Feed += owner.AddResources;
        }
    }
    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
    }
    public override void OnStateExit()
    {
        base.OnStateExit();

        if (pool != null)
        {
            pool.Feed -= owner.AddResources;
        }
    }
}
