using UnityEngine;
using System.Collections;

public class RestartLevelButton : MonoBehaviour {
    public void Action()
    {
        Application.LoadLevel(Application.loadedLevel);
    } 
}
