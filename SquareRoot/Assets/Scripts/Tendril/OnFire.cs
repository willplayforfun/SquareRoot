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
        static float fireSpreadRate = 2.0f;
        static float timeUntilFireOut = 6.0f;
        static float distancePerFire = 1f;

        private List<TendrilNode> neighbors;
        private List<float> spreadProgressToNeighbors;
        private List<float> lastFirePoint;
        private List<float> distanceToNeighbors;

        public OnFire(TendrilNode obj)
               : base(obj)
        {
            spreadProgressToNeighbors = new List<float>();
            distanceToNeighbors = new List<float>();
            lastFirePoint = new List<float>();
            neighbors = new List<TendrilNode>();

            foreach (TendrilNode neighbor in owner.GetNeighbors())
            {
                // dont spread to nodes already on fire
                if (!neighbor.IsOnFire())
                {
                    neighbors.Add(neighbor);
                    distanceToNeighbors.Add(Vector3.Distance(owner.transform.position, neighbor.transform.position));
                    spreadProgressToNeighbors.Add(0);
                    lastFirePoint.Add(0);
                }
            }
        }

        internal override void OnStateEnter()
        {
            base.OnStateEnter();
            /*
            if (owner.nodeFirePrefab != null && owner.fireInstances != null)
            {
                GameObject fire = GameObject.Instantiate(owner.nodeFirePrefab);
                fire.transform.position = owner.transform.position + Vector3.back;
                fire.transform.SetParent(owner.transform);
                owner.fireInstances.Add(fire);
            }*/
        }
        internal override void UpdateState(float deltaTime)
        {
            base.UpdateState(deltaTime);

            bool haveSpreadFire = true;

            for (int i = 0; i < spreadProgressToNeighbors.Count; i++)
            {
                if (spreadProgressToNeighbors[i] / distanceToNeighbors[i] > 1f)
                {
                    // TODO indices break on very long branches
                    neighbors[i].CatchFire();
                }
                else
                {
                    spreadProgressToNeighbors[i] += fireSpreadRate * deltaTime;

                    // fire spawn check
                    if(spreadProgressToNeighbors[i] - lastFirePoint[i] > distancePerFire)
                    {
                        Debug.Log("Spawning fire");
                        Debug.DrawLine(owner.transform.position, owner.transform.position + Vector3.up, Color.red, 1);

                        lastFirePoint[i] = spreadProgressToNeighbors[i];

                        // create fire
                        if (owner.nodeFirePrefab != null && owner.fireInstances != null)
                        {
                        Debug.DrawLine(owner.transform.position, owner.transform.position + Vector3.down, Color.green, 1);
                            GameObject fire = GameObject.Instantiate(owner.nodeFirePrefab);
                            fire.transform.position = owner.transform.position + (neighbors[i].transform.position - owner.transform.position).normalized * spreadProgressToNeighbors[i];
                            fire.transform.SetParent(owner.transform);
                            owner.fireInstances.Add(fire);
                        }
                    }

                    // we haven't spread fire to all neighbors yet, we can't die
                    haveSpreadFire = false;

                    // TODO update mesh and effects and stuff
                    Debug.DrawLine(owner.transform.position, owner.transform.position + (neighbors[i].transform.position - owner.transform.position).normalized * spreadProgressToNeighbors[i]);
                }
            }

            // stop burning
            if (timeInState > timeUntilFireOut && haveSpreadFire)
            {
                owner.Die();
            }
        }
        internal override void OnStateExit()
        {
            base.OnStateExit();
            foreach(GameObject fire in owner.fireInstances)
            {
                GameObject.Destroy(fire);
            }
        }
    }
}