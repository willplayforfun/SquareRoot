using UnityEngine;
using System.Collections;
using InControl;

public class JoinScreenController : MonoBehaviour
{
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

        //InputManager.OnDeviceAttached += AddDevice;
        //InputManager.OnDeviceDetached += AddDevice;
    }

    void Update()
    {
        //if()

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddDevice();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveDevice();
        }
    }

    void AddDevice()
    {
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
            default:
                Debug.LogWarningFormat("Weird stuff inside JoinScreenController AddDevice() ({0} devices)", currentPlayerCount);
                break;
        }
    }

    void RemoveDevice()
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
            default:
                Debug.LogWarningFormat("Weird stuff inside JoinScreenController RemoveDevice() ({0} devices)", currentPlayerCount);
                break;
        }
    }
}
