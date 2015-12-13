using UnityEngine;
using System.Collections;

public class TendrilTip : TendrilNode
{
    // reference to the tendril base node
    public TendrilRoot tendrilRoot;

    // direction of growth
    private Vector2 direction;
    public Vector2 growDirection
    {
        get
        {
            return direction;
        }
        set
        {
            direction = value;
        }
    }

    // distance between nodes
    public float nodeDropRate = 1;
    //speed of the tip
    public float growthRate = 2.0f;
    
    private float timeSinceNodeDropped;

    protected override void Start()
    {
        base.Start();

        hud.maxAngle = maxBranchAngle;
        hud.minAngle = minBranchAngle;
    }

    protected override void Update()
    {
        base.Update();

        // grow
        timeSinceNodeDropped += Time.deltaTime;
        transform.position += (Vector3) (growthRate * Time.deltaTime * direction);

        // if reached nodeDropRate distance since last node, make a new node
        if (nodeDropRate - timeSinceNodeDropped * growthRate <= 0)
        {
            timeSinceNodeDropped = 0;
            CreateNewNode();
        }
    }
    void CreateNewBranch(Vector2 newDirection)
    {
        /*TODO*/
        direction = newDirection;
    }
    void CreateNewNode()
    {
        // TODO: proper tip spawning, not debug cube
        GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newNode.transform.SetParent(parent.transform);
        //newNode.transform.localScale *= 0.5f;
        newNode.transform.position = transform.position;
        newNode.transform.rotation = Quaternion.LookRotation(Vector3.back, parent.transform.position - transform.position);

        TendrilNode newNodeComponent = newNode.AddComponent<TendrilNode>();
        newNodeComponent.AddChild(this);    //new node children set
        newNodeComponent.SetParent(parent); //new node parent set

        // update parent references
        if (parent != null)
        {
            parent.RemoveChild(this);
            parent.AddChild(newNodeComponent); //old parent child set
        }

        parent = newNodeComponent; //my parent set

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.layer)
        {
            case Layers.Fire:
                if (typeof(OnFire) != mState.GetType())
                {
                    Debug.Log("hit fire");
                    mState = new OnFire(this);
                }
                break;
            case Layers.Resources:
                break;
            case Layers.Rock:
                break;
            case Layers.Tendril:
                break;
        }
    }

    // input functions (called down into by parent)
    public override void AccelerateGrowth()
    {

    }

    public float minBranchAngle = 10;
    public float maxBranchAngle = 170;

    public BranchHUD hud;
    private Vector2 currentBranchAim;

    // input functions (called into by TendrilRoot)
    public void StartBranch()
    {
        // show UI
        hud.Show();
    }
    public void EndBranch()
    {
        // hide UI
        hud.Hide();

        // branch off
        if (ValidateBranchDirection(currentBranchAim))
        {
            CreateNewBranch(currentBranchAim.normalized);
        }
    }
    public void BranchAim(Vector2 input)
    {
        // TODO angle snapping

        currentBranchAim = input;
        hud.SetAngle(currentBranchAim);
    }
    public bool ValidateBranchDirection(Vector2 dir)
    {
        return currentBranchAim.magnitude > 0.3f;
    }
}
