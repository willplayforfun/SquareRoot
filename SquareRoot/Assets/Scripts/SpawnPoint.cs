using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
    public Quaternion rotation
    {
        get{
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
    bool inUse = false;
    TendrilRoot mRoot;

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


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
