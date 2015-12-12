using UnityEngine;
using System.Collections.Generic;
using InControl;

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
    private List<PlayerDeviceCoupling> players;

    public JoinPanel player1Pane;
    public JoinPanel player2Pane;
    public JoinPanel player3Pane;
    public JoinPanel player4Pane;

    private int currentPlayerCount;

    void Start()
    {
        player1Pane.Appear();
        player2Pane.Disappear();
        player3Pane.Disappear();
        player4Pane.Disappear();

        players = new List<PlayerDeviceCoupling>();

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        InputDevice inputDevice = InputManager.ActiveDevice;

        if (JoinButtonWasPressedOnDevice(inputDevice))
        {
            if (ThereIsNoPlayerUsingDevice(inputDevice))
            {
                AddDevice(inputDevice);
            }
        }
        if (LeaveButtonWasPressedOnDevice(inputDevice))
        {
            PlayerDeviceCoupling player = FindPlayerUsingDevice(inputDevice);
            if (player != null)
            {
                RemovePlayer(player);
            }
        }
    }

    void OnDeviceDetached(InputDevice inputDevice)
    {
        PlayerDeviceCoupling player = FindPlayerUsingDevice(inputDevice);
        if (player != null)
        {
            RemovePlayer(player);
        }
    }

    void RemovePlayer(PlayerDeviceCoupling player)
    {
        players.Remove(player);
        RemoveDevice();
    }

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

    bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
    {
        return inputDevice.Action1.WasPressed || inputDevice.Command.WasPressed;
    }

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
