using UnityEngine;
using System.Collections;

public class PauseMenuResumeButton : MonoBehaviour
{
    public void Action()
    {
        GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameController>().TogglePause();
    }
}
