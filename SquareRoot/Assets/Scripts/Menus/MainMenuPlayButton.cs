using UnityEngine;
using System.Collections;

public class MainMenuPlayButton : MonoBehaviour
{
    public void Action()
    {
        Application.LoadLevel(Levels.JoinScreen);
    }
}
