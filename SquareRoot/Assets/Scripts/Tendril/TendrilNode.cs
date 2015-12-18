using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TapRoot.Tendril
{
    public class TendrilNode : MonoBehaviour
    {
        protected GameController gameController;
        protected AudioSource mAudioSource;

        public GameObject minimapVis;

        public GameObject nodeFirePrefab;
        internal List<GameObject> fireInstances;

        public MeshMaker mainMeshMaker;
        public MeshMaker sideMeshMaker;

        public event Action NodeCaughtFire;

        internal void UpdateMainMesh(Vector3 newPosition, Vector3 up)
        {
            mainMeshMaker.UpdateMesh(newPosition, up);
        }
        internal void UpdateSideMesh(Vector3 newPosition, Vector3 up)
        {
            sideMeshMaker.UpdateMesh(newPosition, up);
        }

        protected TendrilNodeState mState;
        protected Type state
        {
            get
            {
                return mState.GetType();
            }
        }

        protected void SetState(TendrilNodeState newState)
        {
            if (mState != null)
            {
                mState.OnStateExit();
            }
            mState = newState;
            newState.OnStateEnter();
        }

        public bool IsAlive()
        {
            return state == typeof(Alive) || state == typeof(Growing) || state == typeof(Leeching);
        }
        public bool IsOnFire()
        {
            return state == typeof(OnFire);
        }
        public virtual void CatchFire()
        {
            // if it's dead or burning, won't catch fire again
            if (this.IsAlive())
            {
                SetState(new OnFire(this));

                if(NodeCaughtFire != null)
                {
                    NodeCaughtFire.Invoke();
                }
            }
        }
        public void Die()
        {
            SetState(new Dead(this));
        }

        void OnDestroy()
        {
            mState.OnStateExit();
        }

        protected TendrilNode parent;
        protected List<TendrilNode> children;

        protected float creationTime;

        protected virtual void Awake()
        {
            children = new List<TendrilNode>();
            SetState(new Alive(this));
            mAudioSource = GetComponent<AudioSource>();
            fireInstances = new List<GameObject>();
        }
        protected virtual void Start()
        {
            if (gameController == null)
            {
                gameController = GameObject.FindObjectOfType<GameController>();
            }
            creationTime = Time.time;
        }
        protected virtual void Update()
        {
            mState.UpdateState(Time.deltaTime);
        }

        public void SafeDestroy()
        {
            foreach (TendrilNode child in children)
            {
                child.SetParent(null);
            }
            if (parent != null)
            {
                parent.RemoveChild(this);
            }
            if (mainMeshMaker != null)
            {
                mainMeshMaker.AnimateDestroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            if (sideMeshMaker != null)
            {
                sideMeshMaker.AnimateDestroy();
            }
        }


        // input functions (called down through tree from TendrilRoot)
        public virtual void AccelerateGrowth()
        {
            foreach (TendrilNode child in children)
            {
                child.AccelerateGrowth();
            }
        }

        public List<TendrilNode> GetNeighbors()
        {
            List<TendrilNode> neighbors = new List<TendrilNode>();
            if (parent != null)
            {
                neighbors.Add(parent);
            }
            foreach (TendrilNode child in children)
            {
                neighbors.Add(child);
            }
            return neighbors;
        }

        public void AddChild(TendrilNode t)
        {
            children.Add(t);
        }
        public void RemoveChild(TendrilNode t)
        {
            children.Remove(t);
        }
        public void SetParent(TendrilNode t)
        {
            parent = t;
        }
        public TendrilNode GetParent()
        {
            return parent;
        }
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public List<TendrilNode> GetChildren()
        {
            return children;
        }

        public virtual void AddResources(float amount)
        {
            parent.AddResources(amount);
        }
    }
}