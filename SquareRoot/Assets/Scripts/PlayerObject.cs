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
    // order in the list of players
    PlayerNum playerNum;
    public PlayerNum number
    {
        get
        {
            return playerNum;
        }
    }

    // is this player still active in the game? (have they lost yet?)
    public bool alive
    {
        get
        {
            return true;
        }
    }

    // all present tendrils, alive or dead (until they decompose)
    List<TendrilRoot> roots;


    void Update()
    {
    }

    // initialization functions called by GameController at scene start
    public void SetPlayerNumber(PlayerNum num, int numPlayers)
    {
        // store order
        playerNum = num;

        // set splitscreen
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
