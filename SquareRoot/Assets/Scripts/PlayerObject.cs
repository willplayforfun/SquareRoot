using UnityEngine;
using System.Collections.Generic;

public enum PlayerNum
{
    First,
    Second,
    Third,
    Fourth
}

public class PlayerObject : MonoBehaviour
{
    List<TendrilRoot> roots;

    public void SetPlayerNumber(PlayerNum num, int numPlayers)
    {
        Camera camera = GetComponentInChildren<Camera>();
        switch(num)
        {
            case PlayerNum.First:
                if(numPlayers < 4)
                {
                    camera.rect = new Rect(0, 0.5f, 1, 0.5f);
                }
                else
                {
                    camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Second:
                if (numPlayers == 2)
                {
                    camera.rect = new Rect(0, 0, 1, 0.5f);
                }
                else if (numPlayers == 3)
                {
                    camera.rect = new Rect(0, 0, 0.5f, 0.5f);
                }
                else if (numPlayers == 4)
                {
                    camera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Third:
                if (numPlayers == 4)
                {
                    camera.rect = new Rect(0, 0, 0.5f, 0.5f);
                }
                else
                {
                    camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Fourth:
                camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }
    }
}
