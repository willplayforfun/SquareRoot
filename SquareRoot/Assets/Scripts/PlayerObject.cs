﻿using UnityEngine;
using System.Collections.Generic;
using InControl;
using TapRoot.Tendril;

public enum PlayerNum
{
    First,
    Second,
    Third,
    Fourth
}

public class PlayerObject : MonoBehaviour
{
    public bool debug;

    public bool gameDisabled; // true during start and end times

    public void Lose()
    {
        alive_ = false;
        playerCamera_.GetComponent<FollowingCamera>().ApplyShock(8);
        //FocusOnMap();
        //playerCamera_.orthographicSize = zoomTarget;
        GetComponent<AudioSource>().PlayOneShot(AudioClipManager.instance.ElimSound);

        ui.Lose();

        foreach(TendrilRoot root in roots)
        {
            if(root != null && root.IsAlive())
            {
                root.CutTendril();
            }
        }
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
    public float tendrilSpawnPeriodMin = 10.0f;
    public float tendrilSpawnPeriodMax = 14.0f;
    public float noTendrilsSpawnPeriod = 2f;
    private float nextScheduledTendril;
    private float lastTendrilSpawn;

    SpawnPoint[] mSpawnPoints;

    // InControl device used for input
    InputDevice inputDevice;

    // GameController used to get required resource count and for pausing
    GameController gameController;

    // Player camera for zooming and splitscreening
    Camera playerCamera_;
    public Camera playerCamera
    {
        get
        {
            return playerCamera_;
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
    int activeRootIndex_;
    int activeRootIndex
    {
        get
        {
            return activeRootIndex_;
        }
        set
        {
            if (value >= 0 && roots[value] != null)
            {
                roots[value].activeTip.selected = true;
            }
            if(activeRootIndex_ >= 0 && roots[activeRootIndex_] != null)
            { 
                roots[activeRootIndex_].activeTip.selected = false;
            }
            activeRootIndex_ = value;
        }
    }
    int newestRootIndex;
    private TendrilRoot activeRoot
    {
        get
        {
            return roots[activeRootIndex];
        }
    }

    void Awake()
    {
        playerCamera_ = GetComponentInChildren<Camera>();
        gameController = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameController>();

        FocusOnPlayer();

        ui.vignetteColor = Color.clear;

        alive_ = true;
        resourceCount = startingResourceCount;

        activeRootIndex_ = -1;
    }

    public void StartGame()
    {
        foreach (SpawnZone spawn in FindObjectsOfType<SpawnZone>())
        {
            if (spawn.playerNumber == number)
            {
                currentTendrilIndicator.rotation = spawn.transform.rotation;
                transform.position = spawn.transform.position;
                playerCamera_.transform.rotation = spawn.transform.rotation;
                mSpawnPoints = spawn.spawnPoints;
            }
        }

        roots = new TendrilRoot[mSpawnPoints.Length];

        if (mSpawnPoints != null && mSpawnPoints.Length > 0)
        {
            SpawnTendril();
            mSpawnPoints[newestRootIndex].EnableActiveTip(true);
        }
        else
        {
            Debug.Log("No Spawnpoints available to " + number );
        }
    }

    public void SpawnTendril()
    {
        lastTendrilSpawn = Time.time;
        nextScheduledTendril = Time.time + Random.Range(tendrilSpawnPeriodMin, tendrilSpawnPeriodMax);

        int attempts = 0;
        int index = Random.Range(0, mSpawnPoints.Length);
        SpawnPoint sp = mSpawnPoints[index];

        //old way of determining index;
        /*while((sp = mSpawnPoints[index]).IsInUse() && attempts < 10)
        {
            index = Random.Range(0, mSpawnPoints.Length);
            attempts++;
        }*/

        // Better way of finding a spawn point
        List<SpawnPoint> unusedSpawnPoints = new List<SpawnPoint>();
        // add all unused spawnpoints to list
        foreach (SpawnPoint spawn in mSpawnPoints)
        {
            if (!spawn.IsInUse())
            {
                unusedSpawnPoints.Add(spawn);
            }
        }
        // pick one at random
        int unusedIndex = Random.Range(0, unusedSpawnPoints.Count);
        for (int i = 0; i < mSpawnPoints.Length; i++)
        {
            if (mSpawnPoints[i] == unusedSpawnPoints[unusedIndex])
            {
                sp = mSpawnPoints[i];
                index = i;
                break;
            }
        }
        // now that sp is determined and we have and index for a root:
        // Better way code ends here

        if (!sp.IsInUse())
        {
            TendrilRoot newRoot = Instantiate(tendrilPrefab);
            newRoot.SetPlayer(this);
            newRoot.transform.position = sp.position;
            newRoot.transform.rotation = sp.rotation;
            //Greatest hack-fix of all time pls?
            //newRoot.activeTip.EndBranch();
            //newRoot.activeTip.enabled = false;
            //pls?
            sp.AttachRoot(newRoot);
            roots[index] = newRoot;
            newestRootIndex = index;
            

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
                ui.vignetteColor = new Color(0, 255, 0, 0.5f);
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
            playerCamera_.GetComponent<FollowingCamera>().SetTrackingTarget(roots[activeRootIndex].activeTip.transform, maintainOffset: false);
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
            playerCamera_.GetComponent<FollowingCamera>().SetTrackingTarget(roots[activeRootIndex].transform, maintainOffset: false);
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
        playerCamera_.GetComponent<FollowingCamera>().SetTrackingTarget(this.transform, maintainOffset: false);

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

        playerCamera_.GetComponent<FollowingCamera>().SetTrackingTarget(null);
        playerCamera_.transform.position = new Vector3(0, 0, playerCamera_.transform.position.z);
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

    private bool GoToNewestTendril()
    {
        if(roots[newestRootIndex] != null && roots[newestRootIndex].IsAlive())
        {
            currentTendrilIndicator.gameObject.SetActive(true);
            currentTendrilIndicator.position = roots[newestRootIndex].transform.position - 3f * roots[newestRootIndex].transform.up;

            activeRootIndex = newestRootIndex;
            FocusOnRoot();
            return true;
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

            // notify player of new tendril 
            if (!gameDisabled && (Time.time > nextScheduledTendril || 
                                  (currentFocus == FocusState.Player && Time.time - lastTendrilSpawn > noTendrilsSpawnPeriod)))
            {
                Debug.Log("New Tendril Available");
                //ui.ShowTendrilNotification();
                //ui.vignetteColor = new Color(0, 255, 0, 0.5f);
                //ui.FadeVignetteToColor(new Color(0, 255, 0, 0), 1);
                SpawnTendril();
                mSpawnPoints[newestRootIndex].EnableActiveTip(false);


            }

            // lose check
            if (resourceCount < gameController.requiredResources)
            {
                Lose();
            }

            // handle input

            if (alive && inputDevice != null && !gameDisabled)
            {
                if (activeRootIndex >= 0 && activeRoot != null)
                {
                    // branching
                    //if (inputDevice.Action1.WasPressed && (Time.time > nextScheduledTendril ||
                    //              (currentFocus == FocusState.Player && Time.time - lastTendrilSpawn > noTendrilsSpawnPeriod)))
                    //{
                    //    SpawnTendril();
                    //    GoToNewestTendril();
                    //    FocusOnTip();
                    //    mSpawnPoints[newestRootIndex].EnableActiveTip(false);
                    //    ui.HideTendrilNotification();
                    //}
                    if (inputDevice.Action1.WasPressed || inputDevice.RightTrigger.WasPressed || (debug && Input.GetKeyDown(KeyCode.F)))
                    {
                        Debug.Log(number.ToString() + " player pressed Action 1 (branch).");
                        activeRoot.EndBranch();
                        activeRoot.StartBranch();
                        mSpawnPoints[activeRootIndex_].EnableActiveTip(true);
                    }
                    if (inputDevice.Action1.WasReleased || inputDevice.RightTrigger.WasReleased)
                    {
                        Debug.Log(number.ToString() + " player released Action 1 (branch).");
                        //activeRoot.EndBranch();
                    }
                    activeRoot.BranchAim(playerCamera_.transform.TransformDirection(inputDevice.LeftStick.Vector));

                    // cutting
                    if (inputDevice.Action2.WasPressed || (debug && Input.GetKeyDown(KeyCode.C)))
                    {
                        Debug.Log(number.ToString() + " player pressed Action 2 (cut tendril).");
                        activeRoot.CutTendril();

                        GoToAnyTendril();
                        ui.HideFireNotification();
                        ui.HideTendrilNotification();
                    }

                    // accelerate growth
                    if (inputDevice.Action4.IsPressed)
                    {
                        Debug.Log(number.ToString() + " player accelerating growth.");
                        activeRoot.AccelerateGrowth();
                    }
                }

                if (inputDevice.LeftBumper.WasPressed || (debug && Input.GetKeyDown(KeyCode.Q)))
                {
                    GoToNextLeftTendril();
                    FocusOnTip();
                    ui.HideFireNotification();
                    ui.HideTendrilNotification();
                }

                if (inputDevice.RightBumper.WasPressed || (debug && Input.GetKeyDown(KeyCode.E)))
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
                if ((inputDevice.Action3.WasReleased || inputDevice.LeftTrigger.WasReleased) && currentFocus == FocusState.Root)
                {
                    // focus on tip
                    FocusOnTip();
                }

                // pause
                if (!gameDisabled && (inputDevice.MenuWasPressed || (debug && Input.GetKeyDown(KeyCode.Escape))))// || !inputDevice.IsAttached)
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

            playerCamera_.orthographicSize = Mathf.Lerp(playerCamera_.orthographicSize, zoomTarget, Time.deltaTime / 0.3f);

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
        ui.vignetteColor = new Color(255, 0, 0, 0.8f);
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
                    playerCamera_.rect = new Rect(0, 0, 1, 1);
                    minCameraSize *= 0.25f;
                }
                else if (numPlayers == 2)
                {
                    playerCamera_.rect = new Rect(0, 0.5f, 1, 0.5f);
                    minCameraSize *= 0.5f;
                }
                else if(numPlayers == 3)
                {
                    playerCamera_.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                }
                else
                {
                    playerCamera_.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Second:
                if (numPlayers == 2)
                {
                    playerCamera_.rect = new Rect(0, 0, 1, 0.5f);
                    minCameraSize *= 0.5f;
                }
                else if (numPlayers == 3)
                {
                    //playerCamera_.rect = new Rect(0, 0, 0.5f, 0.5f);
                    playerCamera_.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                }
                else if (numPlayers == 4)
                {
                    playerCamera_.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Third:
                if (numPlayers == 4)
                {
                    playerCamera_.rect = new Rect(0, 0, 0.5f, 0.5f);
                }
                else
                {
                    playerCamera_.rect = new Rect(0, 0, 0.5f, 0.5f);
                    //playerCamera_.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                }
                break;
            case PlayerNum.Fourth:
                playerCamera_.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }
    }
}
