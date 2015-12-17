﻿using UnityEngine;

namespace TapRoot.Tendril
{
    public class Growing : TendrilNodeState
    {
        // reference to cast owner
        private TendrilTip ownerTip;

        private float timeSinceNodeDropped;
        public float timeSinceBranch
        {
            get
            {
                return timeSinceNodeDropped;
            }
        }
        private float lastVertexDrop;

        public Growing(TendrilNode obj) : base(obj)
        {
            ownerTip = ((TendrilTip)owner);
        }
        internal override void OnStateEnter()
        {
            base.OnStateEnter();

            ownerTip.newBranchCreated += NewBranchCreatedCallback;
        }
        private void NewBranchCreatedCallback()
        {
            timeSinceNodeDropped = 0;
        }
        internal override void UpdateState(float deltaTime)
        {
            base.UpdateState(deltaTime);

            // local variables for ease of reference
            float growthRate = ownerTip.growthRate;
            Vector2 growDirection = ownerTip.growDirection;

            // grow
            timeSinceNodeDropped += deltaTime;
            owner.transform.position += (Vector3)(growthRate * deltaTime * growDirection);

            // update collider
            ownerTip.tendrilCollider.size = new Vector2(1, 2f * timeSinceNodeDropped * growthRate);
            ownerTip.tendrilCollider.offset = new Vector2(0, -1f * timeSinceNodeDropped * growthRate);

            if (Time.time - lastVertexDrop > 0.2f && ownerTip.meshRoot != null)
            {
                lastVertexDrop = Time.time;
                // update mesh
                if (ownerTip.mainTip)
                {
                    ownerTip.meshRoot.UpdateMainMesh(owner.transform.position, -owner.transform.up);
                }
                else
                {
                    ownerTip.meshRoot.UpdateSideMesh(owner.transform.position, -owner.transform.up);
                }
            }
        }
        internal override void OnStateExit()
        {
            if (ownerTip.hud != null)
            {
                ownerTip.hud.Hide();
            }

            ownerTip.newBranchCreated -= NewBranchCreatedCallback;
        }
    }
}