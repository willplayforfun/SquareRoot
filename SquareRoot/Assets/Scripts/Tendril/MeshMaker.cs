using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MeshMaker : MonoBehaviour
{
    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> tris;
    private Mesh tendrilMesh;
    private MeshFilter meshfilter;

    public float uvScale = 0.5f;

    void Awake()
    {
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        tris = new List<int>();
        tendrilMesh = new Mesh();
        meshfilter = GetComponent<MeshFilter>();
    }

    void Start()
    {
        if (meshfilter != null)
        {
            if (this.GetType() == typeof(TendrilRoot))
            {
                UpdateMesh(transform.position, transform.up);
            }
            else
            {
                UpdateMesh(transform.position, -transform.up);

            }
        }
    }

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

                tendrilMesh.SetVertices(vertices);
            }

            yield return null;
        }

        if (target != null)
        {
            Destroy(target);
        }
    }
}
