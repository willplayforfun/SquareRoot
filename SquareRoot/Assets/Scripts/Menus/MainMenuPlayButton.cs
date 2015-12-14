using UnityEngine;
using System.Collections;

public class MainMenuPlayButton : MonoBehaviour
{
    public void Action()
    {
        GameObject obj = GameObject.FindGameObjectWithTag(Tags.DeviceBag);
        if ( obj != null)
        {
            Destroy(obj);
        }
        Application.LoadLevel(Levels.JoinScreen);
    }
}
