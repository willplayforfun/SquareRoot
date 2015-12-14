using UnityEngine;
using System;
using System.Collections;

public class ResourcePool : MonoBehaviour
{
    public float maxAmount;
    private float amount;
    public float feedRate;
    
    public event Action<float> Feed;

    void Start()
    {
        amount = maxAmount;
    }

    void Update()
    {
        if(Feed != null)
        {
            int feeders = Feed.GetInvocationList().Length;

            float desiredAmount = feeders * feedRate * Time.deltaTime;
            float actualAmount = Mathf.Min(amount, desiredAmount);

            amount -= actualAmount;
            GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.black, Color.green, amount / maxAmount);

            Feed.Invoke(actualAmount / feeders);
        }
    }
}
