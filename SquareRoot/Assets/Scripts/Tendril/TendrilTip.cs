using UnityEngine;
using System.Collections;

public class TendrilTip : TendrilNode
{
    // reference to the tendril base node
    public TendrilRoot tendrilRoot;
    public TendrilNode meshRoot;

    public TendrilTip tipPrefab;
    public GameObject nodePrefab;

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
            transform.rotation = Quaternion.LookRotation(Vector3.back, direction);
        }
    }

    // distance between nodes
    public float nodeDropRate = 1;
    //speed of the tip
    public float growthRate = 2.0f;

    protected override void Awake()
    {
        base.Awake();
        SetState(new Growing(this));
    }
    protected override void Start()
    {
        base.Start();

        GetComponent<MeshRenderer>().material.color = PlayerUI.playerColors[(int)tendrilRoot.player.number];

        if (hud != null)
        {
            hud.maxAngle = maxBranchAngle;
            hud.minAngle = minBranchAngle;
            hud.Show();
        }
    }

    private float lastVertexDrop;
    protected override void Update()
    {
        base.Update();

        if(Time.time - lastVertexDrop > 0.2f && meshRoot != null)
        {
            lastVertexDrop = Time.time;
            // update mesh
            meshRoot.UpdateMesh(transform.position, -transform.up);
        }
    }

    public override void CatchFire()
    {
        base.CatchFire();

        tendrilRoot.TipCaughtFire();
    }

    void CreateNewBranch(Vector2 newDirection)
    {
        if(state == typeof(Growing))
        {
            TendrilNode parentNode;
            if (Vector3.Distance(transform.position, parent.transform.position) > 0.5f * growthRate / nodeDropRate)
            {
                parentNode = CreateNewNode();
            }
            else
            {
                parentNode = parent;
            }

            TendrilTip newtip = Instantiate(tipPrefab);
            //newtip.transform.SetParent(parentNode.transform);
            newtip.transform.position = transform.position + 0.4f * transform.up;
            newtip.SetParent(parentNode);
            newtip.tendrilRoot = tendrilRoot;
            newtip.growDirection = direction;

            newtip.meshRoot = parentNode;

            growDirection = newDirection;
        }
    }

    public TendrilNode CreateNewNode()
    {
        // TODO: proper tip spawning, not debug cube

        // spawn new node object and setup position/rotation
        GameObject newNode = Instantiate(nodePrefab);
        newNode.layer = Layers.Tendril;
        newNode.transform.localScale *= 0.5f;
        //newNode.transform.SetParent(parent.transform);
        newNode.transform.position = transform.position;
        newNode.transform.rotation = Quaternion.LookRotation(Vector3.back, parent.transform.position - transform.position);

        //newNode.GetComponent<MeshRenderer>().material.color = PlayerUI.playerColors[(int)tendrilRoot.player.number];

        //set collider
        float length = 2 * Vector2.Distance(parent.transform.position, newNode.transform.position);
        BoxCollider2D newCollider = newNode.AddComponent<BoxCollider2D>();
        newCollider.size = new Vector2(1, length);
        newCollider.offset = new Vector2(0, 0.5f * length);

        // setup node component
        TendrilNode newNodeComponent = newNode.AddComponent<TendrilNode>();
        newNodeComponent.AddChild(this);    //new node children set
        newNodeComponent.SetParent(parent); //new node parent set
        newNodeComponent.nodeFirePrefab = nodeFirePrefab;

        // update parent references
        if (parent != null)
        {
            parent.RemoveChild(this);
            parent.AddChild(newNodeComponent); //old parent child set
        }

        parent = newNodeComponent; //my parent set

        return newNodeComponent;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(state == typeof(Growing))
        {
            switch (collision.gameObject.layer)
            {
                case Layers.Fire:
                    if (typeof(OnFire) != mState.GetType())
                    {
                        CatchFire();
                        if (hud != null) hud.Hide();
                    }
                    break;
                case Layers.Resources:
                    SetState(new Leeching(this, collision.gameObject));
                    if (hud != null) hud.Hide();
                    break;
                case Layers.Rock:
                    SetState(new Alive(this));
                    if(hud != null) hud.Hide();
                    break;
                case Layers.Tendril:
                    TendrilNode node = collision.gameObject.GetComponent<TendrilNode>();
                    if (node != null && node != parent)
                    {
                        if (node.IsOnFire())
                        {
                            CatchFire();
                        }
                        else
                        {
                            SetState(new Alive(this));
                        }
                        if (hud != null) hud.Hide();
                    }
                    break;
            }
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
        if (state == typeof(Growing))
        {
            // show UI
            hud.Show();
        }
    }
    public void EndBranch()
    {
        if (state == typeof(Growing))
        {
            // hide UI
            hud.Hide();

            // branch off
            if (ValidateBranchDirection(currentBranchAim))
            {
                CreateNewBranch(currentBranchAim.normalized);
            }
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
        return currentBranchAim.magnitude > 0.2f && Vector2.Angle(transform.up, dir) < maxBranchAngle 
                                                 && Vector2.Angle(transform.up, dir) > minBranchAngle;
    }
}
