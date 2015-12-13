using UnityEngine;
using System.Collections.Generic;

public class TendrilNode : MonoBehaviour
{
    protected TendrilNode parent;
    protected List<TendrilNode> children;
    public State mState;
    protected float timeActive = 0;

    protected virtual void Awake()
    {
        children = new List<TendrilNode>();
    }
    protected virtual void Start()
    {
        mState = new Alive(this);
        //children = new List<TendrilNode>();
    }
    protected virtual void Update()
    {
        timeActive += Time.deltaTime;
        mState.UpdateState();
        if (children.Count > 0)
        {
           // Debug.DrawLine(transform.position, parent.GetPosition(), Color.red);
            foreach (TendrilNode t in children){
                Debug.DrawLine(transform.position, t.GetPosition(), Color.blue);
            }
            if (parent)
            {
                Debug.DrawLine(transform.position, parent.GetPosition(), Color.red);
            }
            //Debug.Log("drew line"); 
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
        neighbors.Add(parent);
        foreach(TendrilNode child in children){
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
}
