using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Quaternion rotation
    {
        get
        {
            return transform.rotation;
        }
    }
    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    private TendrilRoot mRoot;

    public bool IsInUse()
    {
        if (mRoot == null)
        {
            return false;
        }
        return true;
    }

    public void AttachRoot(TendrilRoot root)
    {
        mRoot = root;
    }
}
