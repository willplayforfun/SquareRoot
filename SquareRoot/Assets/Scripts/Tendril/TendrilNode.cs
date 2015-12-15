using UnityEngine;
using System;
using System.Collections.Generic;

public class TendrilNode : MonoBehaviour
{
    protected static GameController theGameController;
    protected AudioSource mAudioSource;

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
        }
    }
    public void Die()
    {
        SetState(new Dead(this));
        theGameController.GetComponent<SplitscreenAudioListener>().UnregisterAudioSource(mAudioSource);
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
        if (theGameController == null)
        {
            theGameController = GameObject.FindObjectOfType<GameController>();  
        }
        SplitscreenAudioListener volumeAdjuster = theGameController.GetComponent<SplitscreenAudioListener>();
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
                Debug.DrawLine(transform.position, t.GetPosition(), Color.blue);
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
        Destroy(this.gameObject);
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
