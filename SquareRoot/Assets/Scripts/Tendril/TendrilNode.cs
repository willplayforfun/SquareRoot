using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TendrilNode : MonoBehaviour
{
    protected static GameController gameController;
    protected AudioSource mAudioSource;
    public GameObject nodeFirePrefab;
    public GameObject fireInstance;

    public MeshMaker mainMeshMaker;
    public MeshMaker sideMeshMaker;

    public void UpdateMainMesh(Vector3 newPosition, Vector3 up)
    {
        mainMeshMaker.UpdateMesh(newPosition, up);
    }
    public void UpdateSideMesh(Vector3 newPosition, Vector3 up)
    {
        sideMeshMaker.UpdateMesh(newPosition, up);
    }

    protected State mState;
    protected Type state
    {
        get
        {
            return mState.GetType();
        }
    }

    protected void SetState(State newState)
    {
        if(mState != null)
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
        if(this.IsAlive())
        {
            SetState(new OnFire(this));
            if (nodeFirePrefab != null && fireInstance == null)
            {
                fireInstance = Instantiate(nodeFirePrefab);
                fireInstance.transform.position = transform.position + Vector3.back;
                fireInstance.transform.SetParent(this.transform);
            }
        }
    }
    public void Die()
    {
        SetState(new Dead(this));

        if(fireInstance != null)
        {
            fireInstance.GetComponent<ParticleSystem>().Stop();
            Destroy(fireInstance, 0.5f);
        }
    }
    void OnDestroy()
    {
        if (fireInstance != null)
        {
            Destroy(fireInstance);
        }
    }

    public void TreeDie()
    {
        Die();
        foreach (TendrilNode t in GetChildren())
        {
            t.TreeDie();
        }
    }

    protected TendrilNode parent;
    protected List<TendrilNode> children;

    protected float creationTime;

    protected virtual void Awake()
    {
        children = new List<TendrilNode>();
        SetState(new Alive(this));
        mAudioSource = GetComponent<AudioSource>();
    }
    protected virtual void Start()
    {
        if (gameController == null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();  
        }
        SplitscreenAudioListener volumeAdjuster = gameController.GetComponent<SplitscreenAudioListener>();
        if (volumeAdjuster)
        {
            volumeAdjuster.RegisterAudioSource(mAudioSource);
            Debug.Log("Audio Source Registered for Volume Adjustment");
        }
        creationTime = Time.time;
    }
    protected virtual void Update()
    {
        mState.UpdateState(Time.deltaTime);
        
        //DEBUG
        if (children.Count > 0)
        {
            foreach (TendrilNode t in children)
            {
                if(t != null)
                {
                    Debug.DrawLine(transform.position, t.GetPosition(), Color.blue);
                }
            }
            if (parent != null)
            {
                Debug.DrawLine(transform.position, parent.GetPosition(), Color.red);
            }
        }
    }

    public void SafeDestroy()
    {
        foreach(TendrilNode child in children)
        {
            child.SetParent(null);
        }
        if(parent != null)
        {
            parent.RemoveChild(this);
        }
        if(mainMeshMaker != null)
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
        if(parent != null)
        {
            neighbors.Add(parent);
        }
        foreach (TendrilNode child in children){
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
