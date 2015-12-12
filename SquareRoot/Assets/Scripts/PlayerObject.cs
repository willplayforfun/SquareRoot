using UnityEngine;
using System.Collections.Generic;
using InControl;

public enum PlayerNum
{
    First,
    Second,
    Third,
    Fourth
}

public class PlayerObject : MonoBehaviour
{
    public static int MaxResources = 100;

    // InControl device used for input
    InputDevice inputDevice;

    // GameController used to get required resource count and for pausing
    GameController gameController;

    // order in the list of players
    PlayerNum playerNum;
    public PlayerNum number
    {
        get
        {
            return playerNum;
        }
    }

    // current resources
    private float resourceCount;
    public float resources
    {
        get
        {
            return resourceCount;
        }
    }

    public void AddResources(float amount)
    {
        resourceCount += amount;
    }

    // returns actual amount removed (down to zero)
    public float TakeResources(float amount)
    {
        float oldResourceCount = resourceCount;
        resourceCount = Mathf.Max(0, resourceCount - amount);
        return oldResourceCount - resourceCount;
    }

    // is this player still active in the game? (have they lost yet?)
    private bool alive_;
    public bool alive
    {
        get
        {
            return alive_;
        }
    }

    // all present tendrils, alive or dead (until they decompose)
    List<TendrilRoot> roots;
    // index in roots list of current player-controlled root
    int activeRoot;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameController>();

        roots = new List<TendrilRoot>();
        activeRoot = -1;

        alive_ = true;
    }

    void Update()
    {
        // resource test
        resourceCount += 2f * Time.deltaTime;

        // lose check

        if (resourceCount < gameController.requiredResources)
        {
            alive_ = false;
        }

        // handle input

        if (alive)
        {
            if (activeRoot >= 0)
            {
                // branching
                if (inputDevice.Action1.WasPressed)
                {
                    Debug.Log(number.ToString() + " player pressed Action 1 (branch).");
                    roots[activeRoot].StartBranch();
                }
                if (inputDevice.Action1.WasReleased)
                {
                    Debug.Log(number.ToString() + " player released Action 1 (branch).");
                    roots[activeRoot].EndBranch();
                }
                roots[activeRoot].BranchAim(inputDevice.LeftStick.Vector);

                // cutting
                if (inputDevice.Action2.WasPressed)
                {
                    Debug.Log(number.ToString() + " player pressed Action 2 (cut tendril).");
                    roots[activeRoot].CutTendril();
                }

                // accelerate growth
                if (inputDevice.Action2.WasPressed)
                {
                    roots[activeRoot].CutTendril();
                }
            }

            // pause
            if (inputDevice.MenuWasPressed)
            {
                Debug.Log(number.ToString() + " player pressed pause.");
                gameController.TogglePause();
            }

            // camera control
            float deltaY = inputDevice.RightStick.Y;
        }
    }

    // initialization function called by GameController at scene start
    public void SetInputDevice(InputDevice device)
    {
        inputDevice = device;
        Debug.LogFormat("Player set to use {0}", device.Name);
    }

    // initialization function called by GameController at scene start
    public void SetPlayerNumber(PlayerNum num, int numPlayers)
    {
        // store order
        playerNum = num;

        // set splitscreen
        Camera camera = GetComponentInChildren<Camera>();
        switch(num)
        {
            case PlayerNum.First:
                if (numPlayers == 1)
                {
                    camera.rect = new Rect(0, 0, 1, 1);
                }
                else if (numPlayers < 4)
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
