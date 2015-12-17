using UnityEngine;
using System.Collections;

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
        static float timeUntilDecompose = 3.0f;

        public Dead(TendrilNode obj)
            : base(obj)
        {

        }

        internal override void OnStateEnter()
        {
            base.OnStateEnter();

            //owner.mainMeshMaker.GetComponent<MeshRenderer>().material.color = owner.deadColor;
            //owner.sideMeshMaker.GetComponent<MeshRenderer>().material.color = owner.deadColor;

            foreach (TendrilNode t in owner.GetChildren())
            {
                t.Die();
            }
        }
        internal override void UpdateState(float deltaTime)
        {
            base.UpdateState(deltaTime);

            if (timeInState > timeUntilDecompose)
            {
                owner.SafeDestroy();
            }
        }
    }
}