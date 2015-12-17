using UnityEngine;
using System.Collections;

namespace TapRoot.Tendril
{
    public class TendrilNodeState
    {
        protected TendrilNode owner;
        protected float creationTime;

        protected float timeInState
        {
            get
            {
                return Time.time - creationTime;
            }
        }

        public TendrilNodeState(TendrilNode obj)
        {
            owner = obj;
        }

        internal virtual void OnStateEnter()
        {
            creationTime = Time.time;
        }

        // Gets called every frame by the owner node
        internal virtual void UpdateState(float deltaTime)
        {

        }

        internal virtual void OnStateExit()
        {

        }
    }
}