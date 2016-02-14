using UnityEngine;
using System.Collections.Generic;

namespace TapRoot.Tendril
{
    public class Dead : TendrilNodeState
    {
        /**
         * State of dying nodes
         * Count down until decompose, at which point
         * children nodes have their state set to "Dead"
         * and this node is destroyed
         */
        static float deathSpreadRate = 2.0f;
        static float timeUntilDecompose = 3.0f;

        private List<TendrilNode> neighbors;
        private List<float> spreadProgressToNeighbors;
        private List<float> distanceToNeighbors;

        public Dead(TendrilNode obj)
            : base(obj)
        {
            spreadProgressToNeighbors = new List<float>();
            distanceToNeighbors = new List<float>();
            neighbors = new List<TendrilNode>();

            foreach (TendrilNode neighbor in owner.GetNeighbors())
            {
                // dont spread to nodes that aren't alive
                if (neighbor.IsAlive())
                {
                    neighbors.Add(neighbor);
                    distanceToNeighbors.Add(Vector3.Distance(owner.transform.position, neighbor.transform.position));
                    spreadProgressToNeighbors.Add(0);
                }
            }
        }

        internal override void OnStateEnter()
        {
            base.OnStateEnter();

            //owner.mainMeshMaker.GetComponent<MeshRenderer>().material.color = owner.deadColor;
            //owner.sideMeshMaker.GetComponent<MeshRenderer>().material.color = owner.deadColor;
        }
        internal override void UpdateState(float deltaTime)
        {
            base.UpdateState(deltaTime);

            bool haveSpreadDeath = true;

            for (int i = 0; i < spreadProgressToNeighbors.Count; i++)
            {
                if (spreadProgressToNeighbors[i] / distanceToNeighbors[i] > 1f)
                {
                    // TODO indices break on very long branches
                    neighbors[i].Die();
                }
                else
                {
                    spreadProgressToNeighbors[i] += deathSpreadRate * deltaTime;

                    if (owner.mainMeshMaker != null)
                        owner.mainMeshMaker.SetDeath(spreadProgressToNeighbors[i]);
                    if (owner.sideMeshMaker != null)
                        owner.sideMeshMaker.SetDeath(spreadProgressToNeighbors[i]);
                    if (owner.GetParent() != null && owner.GetParent().mainMeshMaker != null)
                        owner.GetParent().mainMeshMaker.SetDeath(spreadProgressToNeighbors[i]);

                    // we haven't spread fire to all neighbors yet, we can't die
                    haveSpreadDeath = false;

                    // TODO update mesh and effects and stuff
                    Debug.DrawLine(owner.transform.position, owner.transform.position + (neighbors[i].transform.position - owner.transform.position).normalized * spreadProgressToNeighbors[i], Color.black);
                }
            }

            // stop burning
            if (timeInState > timeUntilDecompose && haveSpreadDeath)
            {
                owner.SafeDestroy();
            }
        }
    }
}