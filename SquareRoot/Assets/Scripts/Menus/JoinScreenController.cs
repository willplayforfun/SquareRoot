using UnityEngine;
using System.Collections.Generic;
using InControl;

// container object to connect a player number to a device
public class PlayerDeviceCoupling
{
    public PlayerNum number;
    public InputDevice device;

    public PlayerDeviceCoupling(PlayerNum p_num, InputDevice p_device)
    {
        number = p_num;
        device = p_device;
    }
}

public class JoinScreenController : MonoBehaviour
{
    private bool loaded;

    private List<PlayerDeviceCoupling> players;
    public List<PlayerDeviceCoupling> playerList
    {
        get
        {
            return players;
        }
    }


    public JoinPanel player1Pane;
    public JoinPanel player2Pane;
    public JoinPanel player3Pane;
    public JoinPanel player4Pane;

    private int currentPlayerCount;

    void LoadGame()
    {
		InputManager.OnDeviceDetached -= OnDeviceDetached;
        loaded = true;
        Application.LoadLevel(Levels.LevelSelection);
    }

    void Start()
    {
		InputManager.OnDeviceDetached += OnDeviceDetached;

        player1Pane.Appear();
        player2Pane.Disappear();
        player3Pane.Disappear();
        player4Pane.Disappear();

        players = new List<PlayerDeviceCoupling>();

        // this object acts as a bridge between scenes
        DontDestroyOnLoad(gameObject);
        gameObject.tag = Tags.DeviceBag;
    }

    void OnLevelWasLoaded(int index)
    {
        if(index != Levels.JoinScreen)
        {
            if (index < Levels.LevelSelection)
            {
                Destroy(gameObject);
                return;
            }

            if (players != null)
            {
                foreach (PlayerDeviceCoupling player in players)
                {
                    Debug.LogFormat("{0} player device... known: {1}, attached {2}... named {3}", player.number.ToString(), player.device.IsKnown, player.device.IsAttached, player.device.Name);
                }
            }
        }
    }

    void Update()
    {
        if (!loaded)
        {
            InputDevice inputDevice = InputManager.ActiveDevice;

            // when A or start is pressed
            if (JoinButtonWasPressedOnDevice(inputDevice))
            {
                Debug.LogFormat("Join pressed on device {0}", inputDevice.Name);

                // if device is new, add a player
                if (ThereIsNoPlayerUsingDevice(inputDevice))
                {
                    Debug.LogFormat("Adding device {0}", inputDevice.Name);

                    AddDevice(inputDevice);
                }
                else
                {
                    Debug.LogFormat("Starting game with {0} players", currentPlayerCount);

                    // start game if the controller is already registered
                    LoadGame();
                }
            }

            // allow players to leave
            if (LeaveButtonWasPressedOnDevice(inputDevice))
            {
                PlayerDeviceCoupling player = FindPlayerUsingDevice(inputDevice);
                if (player != null)
                {
                    RemovePlayer(player);
                }
                else //if(players.Count == 0)
                {
                    Debug.Log("Going to main menu");
		            InputManager.OnDeviceDetached -= OnDeviceDetached;
                    Destroy(gameObject);
                    Application.LoadLevel(Levels.MainMenu);
                }
            }
        }
    }

    void OnDeviceDetached(InputDevice inputDevice)
    {
        // disconnect player when controller is disconnected
        PlayerDeviceCoupling player = FindPlayerUsingDevice(inputDevice);
        if (player != null)
        {
            RemovePlayer(player);
        }
    }

    // removes a player and updates UI
    void RemovePlayer(PlayerDeviceCoupling player)
    {
        players.Remove(player);
        RemoveDevice();
    }

    // adds a player and updates UI
    void AddDevice(InputDevice newDevice)
    {
        if (currentPlayerCount < 4)
        {
            players.Add(new PlayerDeviceCoupling((PlayerNum)currentPlayerCount, newDevice));

            currentPlayerCount++;

            switch (currentPlayerCount)
            {
                case 1:
                    player1Pane.Join();
                    player2Pane.Appear();
                    break;
                case 2:
                    player2Pane.Join();
                    player3Pane.Appear();
                    break;
                case 3:
                    player3Pane.Join();
                    player4Pane.Appear();
                    break;
                case 4:
                    player4Pane.Join();
                    break;
            }
        }
    }

    // updates UI
    void RemoveDevice()
    {
        if (currentPlayerCount > 0)
        {
            currentPlayerCount--;

            switch (currentPlayerCount)
            {
                case 0:
                    player1Pane.Unjoin();
                    player2Pane.Disappear();
                    break;
                case 1:
                    player2Pane.Unjoin();
                    player3Pane.Disappear();
                    break;
                case 2:
                    player3Pane.Unjoin();
                    player4Pane.Disappear();
                    break;
                case 3:
                    player4Pane.Unjoin();
                    break;
            }
        }
    }

    // A or start
    bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
    {
        return inputDevice.Action1.WasPressed || inputDevice.Command.WasPressed;
    }

    // B
    bool LeaveButtonWasPressedOnDevice(InputDevice inputDevice)
    {
        return inputDevice.Action2.WasPressed;
    }
    
    PlayerDeviceCoupling FindPlayerUsingDevice(InputDevice inputDevice)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].device == inputDevice)
            {
                return players[i];
            }
        }

        return null;
    }

    bool ThereIsNoPlayerUsingDevice(InputDevice inputDevice)
    {
        return FindPlayerUsingDevice(inputDevice) == null;
    }
}
