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

    public float cameraZoomSpeed = 2f;
    public float maxCameraSize = 10;
    public float minCameraSize = 3;

    private float timeActive = 0f;

    // how often new tendrils are created
    public float tendrilSpawnPeriod = 12.0f;
    private float lastTendrilSpawn;

    SpawnPoint[] mSpawnPoints;

    // InControl device used for input
    InputDevice inputDevice;

    // GameController used to get required resource count and for pausing
    GameController gameController;

    // Player camera for zooming and splitscreening
    Camera playerCamera;

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

    // spawned when a new tendril is created
    public TendrilRoot tendrilPrefab;

    // all present tendrils, alive or dead (until they decompose)
    TendrilRoot[] roots;
    // index in roots list of current player-controlled root
    int activeRootIndex;
    private TendrilRoot activeRoot
    {
        get
        {
            return roots[activeRootIndex];
        }
    }

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        gameController = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameController>();

        activeRootIndex = -1;

        alive_ = true;
    }

    void Start()
    {
        foreach (SpawnZone spawn in FindObjectsOfType<SpawnZone>())
        {
            if (spawn.playerNumber == number)
            {
                mSpawnPoints = spawn.spawnPoints;
            }
        }

        roots = new TendrilRoot[mSpawnPoints.Length];

        if (mSpawnPoints != null && mSpawnPoints.Length > 0)
        {
            SpawnTendril();
        }
        else
        {
            Debug.Log("No Spawnpoints available to " + number );
        }
    }

    public void SpawnTendril()
    {
        lastTendrilSpawn = Time.time;

        int index = 0;
        foreach (SpawnPoint sp in mSpawnPoints)
        {
            if (!sp.IsInUse())
            {
                TendrilRoot newRoot = Instantiate(tendrilPrefab);
                newRoot.transform.position = sp.position;
                newRoot.transform.rotation = sp.rotation;
                sp.AttachRoot(newRoot);
                roots[index] = newRoot;
                if (activeRootIndex < 0)
                {
                    activeRootIndex = index;
                    playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(newRoot.activeTip.transform, maintainOffset:false);
                }
                break;
            }
            index += 1;
        }
    }

    private void GoToTendril(int delta)
    {
        activeRootIndex = Mathf.FloorToInt(Mathf.Repeat(activeRootIndex + delta, roots.Length - 1));
        playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(roots[activeRootIndex].activeTip.transform, maintainOffset: false);
    }

    void Update()
    {
        timeActive += Time.deltaTime;
        // resource test
        resourceCount += 2f * Time.deltaTime;

        // spawn new tendril 
        if (Time.time - lastTendrilSpawn > tendrilSpawnPeriod)
        {
            Debug.Log("Spawning New Tendril");
            SpawnTendril();
        }
        // lose check

        if (resourceCount < gameController.requiredResources)
        {
            alive_ = false;
        }

        // handle input

        if (alive && inputDevice != null)
        {
            if (activeRootIndex >= 0)
            {
                // branching
                if (inputDevice.Action1.WasPressed)
                {
                    Debug.Log(number.ToString() + " player pressed Action 1 (branch).");
                    activeRoot.StartBranch();
                }
                if (inputDevice.Action1.WasReleased)
                {
                    Debug.Log(number.ToString() + " player released Action 1 (branch).");
                    activeRoot.EndBranch();
                }
                activeRoot.BranchAim(inputDevice.LeftStick.Vector);

                // cutting
                if (inputDevice.Action2.WasPressed)
                {
                    Debug.Log(number.ToString() + " player pressed Action 2 (cut tendril).");
                    playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(activeRoot.transform, maintainOffset: false);
                    activeRoot.CutTendril();
                }

                // accelerate growth
                if (inputDevice.LeftBumper.IsPressed || inputDevice.RightBumper.IsPressed || inputDevice.LeftTrigger.IsPressed || inputDevice.RightTrigger.IsPressed)
                {
                    Debug.Log(number.ToString() + " player accelerating growth.");
                    activeRoot.AccelerateGrowth();
                }
            }

            float deltaX = inputDevice.LeftStick.X;
            if(deltaX > 0.2f)
            {
                // go to right tendril
                GoToTendril(1);
            }
            if (deltaX < -0.2f)
            {
                // go to left tendril
                GoToTendril(-1);
            }

            // pause
            if (inputDevice.MenuWasPressed || !inputDevice.IsAttached)
            {
                Debug.Log(number.ToString() + " player pressed pause.");
                gameController.TogglePause();
            }

            // camera control
            float deltaY = inputDevice.RightStick.Y;
            float originalSize = playerCamera.orthographicSize;
            playerCamera.orthographicSize = Mathf.Clamp(originalSize - deltaY * cameraZoomSpeed, minCameraSize, maxCameraSize);
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
        GetComponentInChildren<PlayerUI>().SetHUDPosition(num, numPlayers);

        // store order
        playerNum = num;

        // set splitscreen
        switch(num)
        {
            case PlayerNum.First:
                if (numPlayers == 1)
                {
                    playerCamera.rect = new Rect(0, 0, 1, 1);
                }
                else if (numPlayers < 4)
                {
                    playerCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
                }
                else
                {
                    playerCamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Second:
                if (numPlayers == 2)
                {
                    playerCamera.rect = new Rect(0, 0, 1, 0.5f);
                }
                else if (numPlayers == 3)
                {
                    playerCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                }
                else if (numPlayers == 4)
                {
                    playerCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Third:
                if (numPlayers == 4)
                {
                    playerCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                }
                else
                {
                    playerCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Fourth:
                playerCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }
    }
}
