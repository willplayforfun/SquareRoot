using UnityEngine;
using System.Collections;

public class PauseMenuEndMatchButton : MonoBehaviour
{
    public void Action()
    {
        GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameController>().TogglePause();
        GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameController>().EndMatch();
    }
}
