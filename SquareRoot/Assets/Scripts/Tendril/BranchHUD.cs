using UnityEngine;
using System.Collections;

public class BranchHUD : MonoBehaviour
{
    public float minAngle = 10;
    public float maxAngle = 170;

    public float radius = 2;

    public LineRenderer dirRenderer;
    public LineRenderer[] extentsRenderers;

    public void Show()
    {
        foreach (LineRenderer renderer in extentsRenderers)
        {
            renderer.enabled = true;
        }
        dirRenderer.enabled = true;

        extentsRenderers[0].SetVertexCount(2);
        extentsRenderers[0].SetPosition(0, Vector3.zero);
        extentsRenderers[0].SetPosition(1, Quaternion.AngleAxis(minAngle, Vector3.forward) * Vector3.up * radius);

        extentsRenderers[1].SetVertexCount(2);
        extentsRenderers[1].SetPosition(0, Vector3.zero);
        extentsRenderers[1].SetPosition(1, Quaternion.AngleAxis(-minAngle, Vector3.forward) * Vector3.up * radius);

        extentsRenderers[2].SetVertexCount(2);
        extentsRenderers[2].SetPosition(0, Vector3.zero);
        extentsRenderers[2].SetPosition(1, Quaternion.AngleAxis(maxAngle, Vector3.forward) * Vector3.up * radius);

        extentsRenderers[3].SetVertexCount(2);
        extentsRenderers[3].SetPosition(0, Vector3.zero);
        extentsRenderers[3].SetPosition(1, Quaternion.AngleAxis(-maxAngle, Vector3.forward) * Vector3.up * radius);
    }
    public void Hide()
    {
        foreach(LineRenderer renderer in extentsRenderers)
        {
            renderer.enabled = false;
        }
        dirRenderer.enabled = false;
    }
    public void SetAngle(Vector2 dir)
    {
        dirRenderer.SetVertexCount(2);
        dirRenderer.SetPosition(0, Vector3.zero);
        dirRenderer.SetPosition(1, transform.InverseTransformDirection(dir) * radius);

        //dirRenderer.material.mainTextureScale = new Vector2(dir.magnitude * radius, 1);
    }
}
