using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuEnablerButton : MonoBehaviour
{
    public GameObject subject;

	void Update ()
    {
	    if(EventSystem.current.currentSelectedGameObject == gameObject)
        {
            subject.SetActive(true);
        }
        else
        {
            subject.SetActive(false);
        }
	}
}
