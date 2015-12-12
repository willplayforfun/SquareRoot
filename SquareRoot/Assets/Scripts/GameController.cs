using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    // reference to GlobalUI in scene (for victory text, scoreboard, pause menu, etc.)
    public GlobalUI ui;

    private bool matchOngoing;

    private bool paused;
    public void TogglePause()
    {
        if(paused)
        {
            paused = false;

            Debug.Log("Game unpaused.");

            //unpause
            Time.timeScale = 1;
            ui.HidePauseMenu();
        }
        else
        {
            paused = true;

            Debug.Log("Game paused.");

            //pause
            Time.timeScale = 0;
            ui.ShowPauseMenu();
        }
    }

    public float resourceRate = 1f;
    float matchStart;
    public int requiredResources
    {
        get
        {
            return Mathf.FloorToInt(resourceRate * (Time.time - matchStart));
        }
    }

    [Space(12)]
    
    public PlayerObject playerPrefab;
    List<PlayerObject> players;

    void Awake()
    {
        gameObject.tag = Tags.GameController;
    }

    void Start()
    {
        if (GameObject.FindGameObjectWithTag(Tags.DeviceBag) == null || GameObject.FindGameObjectWithTag(Tags.DeviceBag).GetComponent<JoinScreenController>() == null)
        {
            Debug.LogError("NO DEVICE/PLAYER LIST!");
            Application.LoadLevel(Levels.JoinScreen);
            return;
        }

        JoinScreenController jsc = GameObject.FindGameObjectWithTag(Tags.DeviceBag).GetComponent<JoinScreenController>();
        int numPlayers = jsc.playerList.Count;

        // spawn requisite number of players and have them configure for split screen
        players = new List<PlayerObject>();
        for(int i = 0; i < numPlayers; i++)
        {
            PlayerObject player = Instantiate(playerPrefab);
            player.SetInputDevice(jsc.playerList[i].device);
            player.SetPlayerNumber(jsc.playerList[i].number, numPlayers);
            players.Add(player);
        }

        paused = false;
        matchStart = Time.time;
        matchOngoing = true;
    }

    void Update()
    {
        if (matchOngoing)
        {
            // win condition check
            PlayerObject alivePlayer = null;
            foreach (PlayerObject player in players)
            {
                if (player.alive)
                {
                    if (alivePlayer == null)
                    {
                        alivePlayer = player;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // DANGER:
            // execution will not reach here if multiple players are alive

            if (alivePlayer != null)
            {
                // win!
                EndMatch(winner: alivePlayer);
            }
        }
    }

    public void EndMatch(PlayerObject winner = null)
    {
        matchOngoing = false;

        if(winner != null)
        {
            Debug.LogFormat("Match concluded, {0} player was winner", winner.number.ToString());
            ui.SetVictoryText(winner.number);
        }
        else
        {
            Debug.Log("Match concluded as a draw");
            ui.SetDrawText();
        }

        // TODO scoreboard
    }
}
