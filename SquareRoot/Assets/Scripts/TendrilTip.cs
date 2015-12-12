using UnityEngine;
using System.Collections;

public class TendrilTip : TendrilNode
{
    Vector3 direction;
    static double nodeDropRate = 1; // distance between nodes
    float growthRate = 2.0f; //speed of the tip
    float timeSinceNodeDropped = 0.0f;
    

    void Start()
    {
        base.Start();
        //TEST
        direction = Vector3.left;
    }


    void Update()
    {
        timeSinceNodeDropped += Time.deltaTime;
        base.Update();
        transform.position += growthRate * Time.deltaTime * direction;
        //Debug.Log("tip active for " + timeActive);
        if (nodeDropRate - timeSinceNodeDropped*growthRate <= 0) // if reached nodeDropRate distance since last node, make a new node
        {
            timeSinceNodeDropped = 0;
            CreateNewNode();
        }
    }
    void CreateNewBranch(Vector3 direction)
    {
        /*TODO*/
        
    }
    void CreateNewNode()
    {
        /*TODO*/
        //TendrilNode node = new TendrilNode();
        GameObject newNode = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newNode.transform.localScale *= 0.5f;
        newNode.transform.position = transform.position;
        newNode.AddComponent<TendrilNode>();
        newNode.GetComponent<TendrilNode>().AddChild(this); //new node children set
        newNode.GetComponent<TendrilNode>().SetParent(parent); //new node parent set
        if (parent != null)
        {
            parent.RemoveChild(this);
            parent.AddChild(newNode.GetComponent<TendrilNode>()); //old parent child set
        }

        parent = newNode.GetComponent<TendrilNode>(); //my parent set

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "fire" && typeof(OnFire) != mState.GetType() ){
            Debug.Log("hit fire");
            mState = new OnFire(this);
        }
    }
    
}
