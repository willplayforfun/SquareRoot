using UnityEngine;
using System.Collections.Generic;

namespace TapRoot.Tendril
{
    public class TendrilRoot : TendrilNode
    {
        public TendrilTip activeTip;

        private PlayerObject player;
        public void SetPlayer(PlayerObject newPlayer)
        {
            player = newPlayer;
        }
        internal PlayerNum GetPlayerNumber()
        {
            return player.number;
        }
        internal void ShakePlayerCamera(float amount)
        {
            player.playerCamera.GetComponent<FollowingCamera>().ApplyShock(amount);
        }

        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();

            activeTip.growDirection = transform.up;
            activeTip.SetParent(this);
            AddChild(activeTip);
            activeTip.meshRoot = this;

            minimapVis.SetActive(false);
            minimapVis.GetComponent<MeshRenderer>().material.color = PlayerUI.playerColors[(int)player.number];

            onFireIndicator.SetActive(false);
            GetComponent<AudioSource>().PlayOneShot(AudioClipManager.instance.NewBranchSound);
        }

        public override void AddResources(float amount)
        {
            player.AddResources(amount);
        }

        // input functions (called into by PlayerObject)
        public void StartBranch()
        {
            activeTip.StartBranch();
        }
        public void EndBranch()
        {
            activeTip.EndBranch();
        }
        public void BranchAim(Vector2 input)
        {
            activeTip.BranchAim(input);
        }

        public void CutTendril()
        {
            Die();
            onFireIndicator.SetActive(false);
            GetComponent<AudioSource>().PlayOneShot(AudioClipManager.instance.CutSound);
        }

        public override void CatchFire()
        {
            base.CatchFire();
            player.Lose();
        }

        public GameObject onFireIndicator;

        public void TipCaughtFire()
        {
            onFireIndicator.SetActive(true);
            player.TendrilCaughtOnFire();
        }
    }
}