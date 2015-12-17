using UnityEngine;

namespace TapRoot.Tendril
{
    public class Leeching : TendrilNodeState
    {
        ResourcePool pool;

        public Leeching(TendrilNode obj, GameObject foodPool)
                 : base(obj)
        {
            pool = foodPool.GetComponent<ResourcePool>();
        }

        internal override void OnStateEnter()
        {
            base.OnStateEnter();

            if (owner.GetComponent<AudioSource>() && AudioClipManager.instance.LeechingSound)
            {
                owner.GetComponent<AudioSource>().PlayOneShot(AudioClipManager.instance.LeechingSound);
            }

            if (pool != null)
            {
                pool.Feed += owner.AddResources;
            }
        }
        internal override void UpdateState(float deltaTime)
        {
            base.UpdateState(deltaTime);
        }
        internal override void OnStateExit()
        {
            base.OnStateExit();

            if (pool != null)
            {
                pool.Feed -= owner.AddResources;
            }
        }
    }
}