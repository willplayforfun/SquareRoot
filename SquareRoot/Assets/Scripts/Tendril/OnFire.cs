using UnityEngine;
using System.Collections.Generic;

namespace TapRoot.Tendril
{
    public class OnFire : TendrilNodeState
    {

        /**
         * State for nodes that are on fire
         * Fire begins to spread in timeUntilFireSpread seconds
         * Node's state becomes "Dead" in timeUntilFireOut seconds from fire's start
         */
        static float fireSpreadRate = 4.0f;
        static float timeUntilFireOut = 6.0f;

        private List<float> spreadProgressToNeighbors;
        private List<float> distanceToNeighbors;

        public OnFire(TendrilNode obj)
               : base(obj)
        {
            spreadProgressToNeighbors = new List<float>();
            distanceToNeighbors = new List<float>();

            foreach (TendrilNode neighbor in owner.GetNeighbors())
            {
                distanceToNeighbors.Add(Vector3.Distance(owner.transform.position, neighbor.transform.position));
                spreadProgressToNeighbors.Add(0);
            }
        }

        internal override void OnStateEnter()
        {
            base.OnStateEnter();

            if (owner.nodeFirePrefab != null && owner.fireInstance == null)
            {
                owner.fireInstance = GameObject.Instantiate(owner.nodeFirePrefab);
                owner.fireInstance.transform.position = owner.transform.position + Vector3.back;
                owner.fireInstance.transform.SetParent(owner.transform);
            }
        }
        internal override void UpdateState(float deltaTime)
        {
            base.UpdateState(deltaTime);

            for (int i = 0; i < spreadProgressToNeighbors.Count; i++)
            {
                if (spreadProgressToNeighbors[i] / distanceToNeighbors[i] > 1f)
                {
                    owner.GetNeighbors()[i].CatchFire();
                }
                else
                {
                    spreadProgressToNeighbors[i] += fireSpreadRate * deltaTime;

                    // TODO update mesh and effects and stuff
                    Debug.DrawLine(owner.transform.position, owner.transform.position + (owner.GetNeighbors()[i].transform.position - owner.transform.position).normalized * spreadProgressToNeighbors[i]);
                }
            }

            // stop burning
            if (timeInState > timeUntilFireOut)
            {
                owner.Die();
            }
        }
        internal override void OnStateExit()
        {
            base.OnStateExit();
            GameObject.Destroy(owner.fireInstance);
        }
    }
}