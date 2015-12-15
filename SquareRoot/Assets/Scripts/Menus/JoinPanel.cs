using UnityEngine;
using System.Collections;

public class JoinPanel : MonoBehaviour
{
    public GameObject[] unjoinedElements;
    public GameObject[] joinedElements;

    void Start()
    {
        Unjoin();
    }

    public void Join()
    {
        GetComponent<AudioSource>().Play();
        foreach (GameObject element in unjoinedElements)
        {
            element.SetActive(false);
        }
        foreach (GameObject element in joinedElements)
        {
            element.SetActive(true);
        }
    }

    public void Unjoin()
    {
        GetComponent<AudioSource>().Play();
        foreach (GameObject element in unjoinedElements)
        {
            element.SetActive(true);
        }
        foreach (GameObject element in joinedElements)
        {
            element.SetActive(false);
        }
    }

    public void Appear()
    {
        gameObject.SetActive(true);
    }

    public void Disappear()
    {
        gameObject.SetActive(false);
    }
}
