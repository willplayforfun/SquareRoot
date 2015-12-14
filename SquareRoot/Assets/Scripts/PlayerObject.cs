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

        FocusOnPlayer();

        alive_ = true;
    }

    void Start()
    {
        foreach (SpawnZone spawn in FindObjectsOfType<SpawnZone>())
        {
            if (spawn.playerNumber == number)
            {
                currentTendrilIndicator.rotation = spawn.transform.rotation;
                transform.position = spawn.transform.position;
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
                newRoot.player = this;
                newRoot.transform.position = sp.position;
                newRoot.transform.rotation = sp.rotation;
                sp.AttachRoot(newRoot);
                roots[index] = newRoot;

                // if focused on player
                if (currentFocus == FocusState.Player)
                {
                    // focus on new tendril
                    activeRootIndex = -1;
                    GoToNextRightTendril();
                    FocusOnTip();
                }
                break;
            }
            index += 1;
        }
    }

    public Transform currentTendrilIndicator;

    private enum FocusState
    {
        Tip,
        Root,
        Player
    }
    private FocusState currentFocus;

    private void FocusOnTip()
    {
        bool goAhead = true;

        if(!roots[activeRootIndex].activeTip.IsAlive())
        {
            if(roots[activeRootIndex].IsAlive())
            {
                goAhead = false;
                FocusOnRoot();
            }
            else
            {
                goAhead = GoToAnyTendril();
            }
        }

        if(goAhead)
        {
            currentFocus = FocusState.Tip;
            playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(roots[activeRootIndex].activeTip.transform, maintainOffset: false);
        }
    }

    private void FocusOnRoot()
    {
        currentFocus = FocusState.Root;
        playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(roots[activeRootIndex].transform, maintainOffset: false);
    }

    private void FocusOnPlayer()
    {
        currentFocus = FocusState.Player;
        playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(this.transform, maintainOffset: false);

        currentTendrilIndicator.gameObject.SetActive(false);
    }

    private bool GoToAnyTendril()
    {
        if (!GoToNextRightTendril())
        {
            if (!GoToNextLeftTendril())
            {
                FocusOnPlayer();
                return false;
            }
            else
            {
                FocusOnTip();
                return true;
            }
        }
        else
        {
            FocusOnTip();
            return true;
        }
    }

    private bool GoToNextRightTendril()
    {
        for (int i = Mathf.Clamp(activeRootIndex + 1, 0, roots.Length - 1); i < roots.Length; i++)
        {
            if (roots[i] != null && roots[i].IsAlive())
            {
                currentTendrilIndicator.gameObject.SetActive(true);
                currentTendrilIndicator.position = roots[i].transform.position - roots[i].transform.up;

                activeRootIndex = i;
                FocusOnRoot();
                return true;
            }
        }
        return false;
    }
    private bool GoToNextLeftTendril()
    {
        for (int i = Mathf.Clamp(activeRootIndex - 1, 0, roots.Length - 1); i >= 0; i--)
        {
            if (roots[i] != null && roots[i].IsAlive())
            {
                currentTendrilIndicator.gameObject.SetActive(true);
                currentTendrilIndicator.position = roots[i].transform.position - roots[i].transform.up;

                activeRootIndex = i;
                FocusOnRoot();
                return true;
            }
        }
        return false;
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
            if (activeRootIndex >= 0 && activeRoot != null)
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
                    activeRoot.CutTendril();

                    GoToAnyTendril();
                }

                // accelerate growth
                if (inputDevice.LeftBumper.IsPressed || inputDevice.RightBumper.IsPressed || inputDevice.LeftTrigger.IsPressed || inputDevice.RightTrigger.IsPressed)
                {
                    Debug.Log(number.ToString() + " player accelerating growth.");
                    activeRoot.AccelerateGrowth();
                }
            }

            if (inputDevice.Action3.WasPressed)
            {
                // focus on root
                FocusOnRoot();
            }
            if (inputDevice.Action3.IsPressed)
            {
                if (inputDevice.LeftStickRight.WasPressed)
                {
                    // go to next right tendril
                    GoToNextRightTendril();
                }
                if (inputDevice.LeftStickLeft.WasPressed)
                {
                    // go to next left tendril
                    GoToNextLeftTendril();
                }
            }
            if (inputDevice.Action3.WasReleased)
            {
                // focus on tip
                FocusOnTip();
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
