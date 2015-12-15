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
    public void Lose()
    {
        alive_ = false;
        playerCamera.GetComponent<FollowingCamera>().ApplyShock(8);
        FocusOnMap();
        playerCamera.orthographicSize = zoomTarget;
    }

    public float startingResourceCount = 5;
    public static int MaxResources = 100;

    public float cameraZoomSpeed = 2f;
    public float maxCameraSize = 10;
    public float minCameraSize = 3;

    public float mapZoomLevel = 50;
    public float playerZoomLevel = 30;

    private float previousZoom;
    private float zoomTarget = 5;

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
    public Camera camera
    {
        get
        {
            return playerCamera;
        }
    }
    //minimap camera
    Camera minimapCamera;

    public PlayerUI ui;

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

        ui.vignetteColor = Color.clear;

        alive_ = true;
        resourceCount = startingResourceCount;
    }

    void Start()
    {
        foreach (SpawnZone spawn in FindObjectsOfType<SpawnZone>())
        {
            if (spawn.playerNumber == number)
            {
                currentTendrilIndicator.rotation = spawn.transform.rotation;
                transform.position = spawn.transform.position;
                playerCamera.transform.rotation = spawn.transform.rotation;
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

        int attempts = 0;
        int index = Random.Range(0, mSpawnPoints.Length);
        SpawnPoint sp;
        while((sp = mSpawnPoints[index]).IsInUse() && attempts < 10)
        {
            index = Random.Range(0, mSpawnPoints.Length);
            attempts++;
        }

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
            else
            {
                ui.ShowTendrilNotification();
                ui.vignetteColor = Color.green;
                ui.FadeVignetteToColor(new Color(0, 255, 0, 0), 1);
            }
        }
    }

    public Transform currentTendrilIndicator;

    private enum FocusState
    {
        Tip,
        Root,
        Player,
        Map
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
            zoomTarget = previousZoom;
        }
    }

    private void FocusOnRoot()
    {
        if (roots[activeRootIndex] != null)
        {
            if (currentFocus == FocusState.Tip)
            {
                previousZoom = zoomTarget;
            }

            currentFocus = FocusState.Root;
            playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(roots[activeRootIndex].transform, maintainOffset: false);
            zoomTarget = playerZoomLevel;
        }
        else
        {
            FocusOnPlayer();
        }
    }

    private void FocusOnPlayer()
    {
        if (currentFocus == FocusState.Tip)
        {
            previousZoom = zoomTarget;
        }

        currentFocus = FocusState.Player;
        playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(this.transform, maintainOffset: false);

        currentTendrilIndicator.gameObject.SetActive(false);
        zoomTarget = playerZoomLevel;
    }

    private void FocusOnMap()
    {
        if (currentFocus == FocusState.Tip)
        {
            previousZoom = zoomTarget;
        }

        currentFocus = FocusState.Map;

        playerCamera.GetComponent<FollowingCamera>().SetTrackingTarget(null);
        playerCamera.transform.position = new Vector3(0, 0, playerCamera.transform.position.z);
        zoomTarget = mapZoomLevel;
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
                currentTendrilIndicator.position = roots[i].transform.position - 3f * roots[i].transform.up;

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
                currentTendrilIndicator.position = roots[i].transform.position - 3f * roots[i].transform.up;

                activeRootIndex = i;
                FocusOnRoot();
                return true;
            }
        }
        return false;
    }
    
    void Update()
    {
        if (alive_)
        {
            timeActive += Time.deltaTime;
            // resource test
            //resourceCount += 0.5f * Time.deltaTime;

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
                    if (inputDevice.Action1.WasPressed || inputDevice.RightTrigger.WasPressed)
                    {
                        Debug.Log(number.ToString() + " player pressed Action 1 (branch).");
                        activeRoot.EndBranch();
                        activeRoot.StartBranch();
                    }
                    if (inputDevice.Action1.WasReleased || inputDevice.RightTrigger.WasReleased)
                    {
                        Debug.Log(number.ToString() + " player released Action 1 (branch).");
                        //activeRoot.EndBranch();
                    }
                    activeRoot.BranchAim(playerCamera.transform.TransformDirection(inputDevice.LeftStick.Vector));

                    // cutting
                    if (inputDevice.Action2.WasPressed)
                    {
                        Debug.Log(number.ToString() + " player pressed Action 2 (cut tendril).");
                        activeRoot.CutTendril();

                        GoToAnyTendril();
                        ui.HideFireNotification();
                        ui.HideTendrilNotification();
                    }

                    // accelerate growth
                    if (false)//(inputDevice.LeftBumper.IsPressed || inputDevice.RightBumper.IsPressed || inputDevice.LeftTrigger.IsPressed || inputDevice.RightTrigger.IsPressed)
                    {
                        Debug.Log(number.ToString() + " player accelerating growth.");
                        activeRoot.AccelerateGrowth();
                    }
                }

                if (inputDevice.LeftBumper.WasPressed)
                {
                    GoToNextLeftTendril();
                    FocusOnTip();
                    ui.HideFireNotification();
                    ui.HideTendrilNotification();
                }

                if (inputDevice.RightBumper.WasPressed)
                {
                    GoToNextRightTendril();
                    FocusOnTip();
                    ui.HideFireNotification();
                    ui.HideTendrilNotification();
                }   

                if ((inputDevice.Action3.WasPressed || inputDevice.LeftTrigger.WasPressed) && currentFocus == FocusState.Tip)
                {
                    // focus on root
                    FocusOnRoot();
                    ui.HideFireNotification();
                    ui.HideTendrilNotification();
                }
                if (inputDevice.Action3.IsPressed || inputDevice.LeftTrigger.IsPressed)
                {
                    //bool goRight = false;
                    //bool goLeft = false;
                    bool goLeft = inputDevice.LeftStickLeft.WasPressed;
                    bool goRight = inputDevice.LeftStickRight.WasPressed;
                    /*
                    switch(number)
                    {
                        case PlayerNum.First:
                            goLeft = inputDevice.LeftStickLeft.WasPressed || inputDevice.LeftTrigger.WasPressed;
                            goRight = inputDevice.LeftStickRight.WasPressed || inputDevice.RightTrigger.WasPressed;
                            break;
                        case PlayerNum.Second:
                            goLeft = inputDevice.LeftStickUp.WasPressed;
                            goRight = inputDevice.LeftStickDown.WasPressed;
                            break;
                        case PlayerNum.Third:
                            goLeft = inputDevice.LeftStickRight.WasPressed;
                            goRight = inputDevice.LeftStickLeft.WasPressed;
                            break;
                        case PlayerNum.Fourth:
                            goLeft = inputDevice.LeftStickDown.WasPressed;
                            goRight = inputDevice.LeftStickUp.WasPressed;
                            break;
                    }
                    */

                    if (goRight)
                    {
                        // go to next right tendril
                        GoToNextRightTendril();
                    }
                    if (goLeft)
                    {
                        // go to next left tendril
                        GoToNextLeftTendril();
                    }
                }
                if ((inputDevice.Action3.WasReleased || inputDevice.LeftTrigger.WasReleased) && currentFocus == FocusState.Root)
                {
                    // focus on tip
                    FocusOnTip();
                }

                // pause
                if (inputDevice.MenuWasPressed)// || !inputDevice.IsAttached)
                {
                    Debug.Log(number.ToString() + " player pressed pause.");
                    gameController.TogglePause();
                }

                // camera control
                if (currentFocus == FocusState.Tip)
                {
                    float deltaY = inputDevice.RightStick.Y;
                    float originalSize = zoomTarget;
                    zoomTarget = Mathf.Clamp(originalSize - deltaY * cameraZoomSpeed, minCameraSize, maxCameraSize);
                }
            }

            playerCamera.orthographicSize = Mathf.Lerp(playerCamera.orthographicSize, zoomTarget, Time.deltaTime / 0.3f);

            //Personal Minimap code:
            //GameObject minimapTrackingTransformHolder = Instantiate<GameObject> 
            //float minimapX = (minX + maxX) / 2;
            //float minimapY = (minY + maxY) / 2;
            //float minimapZ = playerCamera.GetComponent<FollowingCamera>().tra
            //minimapTransform.position = minX + maxX / 2;
            //minimapCamera.GetComponent<FollowingCamera>().SetTrackingTarget()
        }
    }
    
    public void TendrilCaughtOnFire()
    {
        ui.vignetteColor = Color.red;
        ui.FadeVignetteToColor(new Color(255, 0, 0, 0), 1);
        ui.ShowFireNotification();
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
                    minCameraSize *= 0.25f;
                }
                else if (numPlayers < 4)
                {
                    playerCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
                    minCameraSize *= 0.5f;
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
                    minCameraSize *= 0.5f;
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
