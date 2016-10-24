using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TapRoot.Tendril;

public class MeshMaker : MonoBehaviour
{
    struct VertexSet
    {
        public Vector3 position;
        public Vector3 direction;

        //Vector3[] vertices;
        //Vector3[] normals;
        //Vector3[] uvs;
    }

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> tris;

    private List<VertexSet> sets;

    private Mesh primaryTendrilMesh;
    private Mesh secondaryTendrilMesh;
    public MeshFilter primaryMeshFilter;
    public MeshFilter secondaryMeshFilter;

    public float uvScale = 0.5f;

    void Awake()
    {
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();
        sets = new List<VertexSet>();
        primaryTendrilMesh = new Mesh();
        secondaryTendrilMesh = new Mesh();
        //primaryMeshFilter = GetComponent<MeshFilter>();
    }

    void Start()
    {
        if (primaryMeshFilter != null)
        {
            if (transform.parent.GetComponent<TendrilRoot>() != null)
            {
                UpdateMesh(transform.position, -transform.up);
            }
            else
            {
                UpdateMesh(transform.position, transform.up);

            }
        }
    }

    public void SetDeath(float t)
    {
        if (primaryTendrilMesh.vertexCount > 0)
        {
            Debug.LogFormat("Object ({0}) has [{1}] vertices", name, primaryTendrilMesh.vertexCount);
            Color[] colors = new Color[primaryTendrilMesh.vertexCount];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(1 - t, 1 - t, 1 - t, 1);
            }
            primaryTendrilMesh.colors = colors;
        }
    }

    public void UpdateMesh(Vector3 newPosition, Vector3 up)
    {
        VertexSet set = new VertexSet();
        set.position = newPosition;
        set.direction = up;
        sets.Add(set);

        Vector3 right = Vector3.Cross(up, Vector3.back);

        float randomScale1 = UnityEngine.Random.Range(0.8f, 1.2f);
        float randomScale2 = UnityEngine.Random.Range(0.8f, 1.2f);

        vertices.Add(transform.InverseTransformPoint(newPosition + right * 0.25f * randomScale1));
        vertices.Add(transform.InverseTransformPoint(newPosition - right * 0.25f * randomScale2));

        int end = vertices.Count - 1;

        if (sets.Count > 1)
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


        if (sets.Count > 1)
        {
            tris.Add(end);
            tris.Add(end - 1);
            tris.Add(end - 2);

            tris.Add(end - 3);
            tris.Add(end - 2);
            tris.Add(end - 1);
        }

        primaryTendrilMesh.SetVertices(vertices);
        primaryTendrilMesh.uv = uvs.ToArray();
        primaryTendrilMesh.normals = normals.ToArray();
        primaryTendrilMesh.triangles = tris.ToArray();

        primaryTendrilMesh.RecalculateNormals();
        primaryTendrilMesh.RecalculateBounds();

        secondaryTendrilMesh.SetVertices(vertices);
        secondaryTendrilMesh.uv = uvs.ToArray();
        secondaryTendrilMesh.normals = normals.ToArray();
        secondaryTendrilMesh.triangles = tris.ToArray();

        secondaryTendrilMesh.RecalculateNormals();
        secondaryTendrilMesh.RecalculateBounds();

        primaryMeshFilter.mesh = primaryTendrilMesh;
        //secondaryMeshFilter.mesh = secondaryTendrilMesh;
    }
    public void AnimateDestroy(GameObject target = null)
    {
        StartCoroutine(DestructionAnimation(target));
    }

    IEnumerator DestructionAnimation(GameObject target)
    {
        float start = Time.time;

        while (Time.time - start < 0.4f)
        {
            float progress = (Time.time - start) / 0.4f;

            for (int i = 0; i < vertices.Count; i += 2)
            {
                Vector3 average = Vector3.Lerp(vertices[i], vertices[i + 1], 0.5f);
                vertices[i] = Vector3.Lerp(vertices[i], average, progress);
                vertices[i + 1] = Vector3.Lerp(vertices[i + 1], average, progress);

                primaryTendrilMesh.SetVertices(vertices);
                //secondaryTendrilMesh.SetVertices(vertices);
            }

            yield return null;
        }

        if (target != null)
        {
            Destroy(target);
        }
    }
}
