using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TendrilNode : MonoBehaviour
{
    public GameObject nodeFirePrefab;
    public GameObject fireInstance;

    private Color deadColor_ = new Color(0.3f, 0.3f, 0.3f);
    public Color deadColor
    {
        get
        {
            return deadColor_;
        }
    }

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> tris;
    private Mesh tendrilMesh;
    private MeshFilter meshfilter;

    public float uvScale = 0.5f;

    public void UpdateMesh(Vector3 newPosition, Vector3 up)
    {
        Vector3 right = Vector3.Cross(up, Vector3.back);

        float randomScale1 = UnityEngine.Random.Range(0.8f, 1.2f);
        float randomScale2 = UnityEngine.Random.Range(0.8f, 1.2f);

        vertices.Add(transform.InverseTransformPoint(newPosition + right * 0.25f * randomScale1));
        vertices.Add(transform.InverseTransformPoint(newPosition - right * 0.25f * randomScale2));

        int end = vertices.Count - 1;

        if (vertices.Count > 3)
        {
            float distanceFromLast = Vector3.Distance(vertices[end], vertices[end - 2]);

            uvs.Add(new Vector2(0, uvs[end - 2].y + distanceFromLast * uvScale));
            uvs.Add(new Vector2(uvScale, uvs[end - 2].y + distanceFromLast * uvScale));
        }
        else
        {
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(uvScale, 0));
        }

        normals.Add(Vector3.back);
        normals.Add(Vector3.back);


        if (vertices.Count > 3)
        {
            tris.Add(end);
            tris.Add(end - 1);
            tris.Add(end - 2);

            tris.Add(end - 3);
            tris.Add(end - 2);
            tris.Add(end - 1);
        }

        tendrilMesh.SetVertices(vertices);
        tendrilMesh.uv = uvs.ToArray();
        tendrilMesh.normals = normals.ToArray();
        tendrilMesh.triangles = tris.ToArray();

        tendrilMesh.RecalculateNormals();
        tendrilMesh.RecalculateBounds();

        meshfilter.mesh = tendrilMesh;
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
            if (nodeFirePrefab != null)
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


        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();
        tendrilMesh = new Mesh();
    }
    protected virtual void Start()
    {
        creationTime = Time.time;

        meshfilter = GetComponent<MeshFilter>();
        if (meshfilter != null)
        {
            if (this.GetType() == typeof(TendrilRoot))
            {
                UpdateMesh(transform.position, -transform.up);
            }
            else
            {
                UpdateMesh(transform.position, transform.up);

            }
        }
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
        StartCoroutine(DestructionAnimation());
    }

    IEnumerator DestructionAnimation()
    {
        float start = Time.time;

        while(Time.time - start < 0.4f)
        {
            float progress = (Time.time - start) / 0.4f;

            for(int i = 0; i < vertices.Count; i += 2)
            {
                Vector3 average = Vector3.Lerp(vertices[i], vertices[i + 1], 0.5f);
                vertices[i] = Vector3.Lerp(vertices[i], average, progress);
                vertices[i + 1] = Vector3.Lerp(vertices[i + 1], average, progress);

                tendrilMesh.SetVertices(vertices);
            }

            yield return null;
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
