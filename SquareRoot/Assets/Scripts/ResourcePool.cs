using UnityEngine;
using System;
using System.Collections;

public class ResourcePool : MonoBehaviour
{
    public float amount;
    public float feedRate;
    
    public event Action<float> Feed;

    void Update()
    {
        if(Feed != null)
        {
            int feeders = Feed.GetInvocationList().Length;

            float desiredAmount = feeders * feedRate * Time.deltaTime;
            float actualAmount = Mathf.Min(amount, desiredAmount);

            amount -= actualAmount;

            Feed.Invoke(actualAmount / feeders);
        }
    }
}
