using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameController : MonoBehaviour
{
    public float startBuffer = 10;
    public float resourceRate = 1f;
    public float GameStartDelay = 5.0f;
    public float GameEndDelay = 2.0f; // Time Between Game Ending and Game Over screen showing

    [Space(12)]

    // reference to GlobalUI in scene (for victory text, scoreboard, pause menu, etc.)
    public GlobalUI ui;

    public PlayerObject playerPrefab;
    List<PlayerObject> players;

    private bool paused;
    public void TogglePause(bool showMenu = true)
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
            if(showMenu) ui.ShowPauseMenu();
        }
    }

    private bool matchOngoing;
    private bool matchStarted;
    private float matchStart;
    public int requiredResources
    {
        get
        {
            return Mathf.FloorToInt(resourceRate * (Time.time - matchStart - startBuffer - GameStartDelay));
        }
    }

    void Awake()
    {
        gameObject.tag = Tags.GameController;
        Time.timeScale = 1.0f;
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

        ui.SetMinimap(numPlayers);

        // spawn requisite number of players and have them configure for split screen
        players = new List<PlayerObject>();
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerObject player = Instantiate(playerPrefab);
            player.SetInputDevice(jsc.playerList[i].device);
            player.SetPlayerNumber(jsc.playerList[i].number, numPlayers);
            players.Add(player);

            player.gameDisabled = true;

            if (numPlayers < 2)
            {
                player.debug = true;
            }
        }

        paused = false;
        matchStart = Time.time;
        matchOngoing = false;
        matchStarted = false;

        foreach (PlayerObject player in players)
        {
            player.ui.StartCountdown(GameStartDelay);
        }
    }

    void Update()
    {
        if(!matchStarted)
        {
            if(Time.time - matchStart > GameStartDelay)
            {
                matchStarted = true;
                matchOngoing = true;

                foreach (PlayerObject player in players)
                {
                    player.gameDisabled = false;
                    player.StartGame();
                }

                // start music
                GetComponent<AudioSource>().Play();
            }
        }

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

            if (alivePlayer != null && players.Count > 1)
            {
                // win!
                EndMatch(winner: alivePlayer);
            }
            else if(alivePlayer == null)
            {
                EndMatch();
            }
        }
    }

    public void EndMatch(PlayerObject winner = null)
    {
        matchOngoing = false;

        // stop music
        GetComponent<AudioSource>().Stop();

        if(winner != null)
        {
            Debug.LogFormat("Match concluded, {0} player was winner", winner.number.ToString());
            ui.SetVictoryText(winner.number);
            TogglePause(showMenu: false);
            GetComponent<AudioSource>().PlayOneShot(AudioClipManager.instance.VictorySound);
        }
        else
        {
            Debug.Log("Match concluded as a draw");
            ui.SetDrawText();
            TogglePause(showMenu: false);
            GetComponent<AudioSource>().PlayOneShot(AudioClipManager.instance.DrawSound);
        }

        StartCoroutine("DisplayGameOverMenu");
        // TODO scoreboard
        
    }

    IEnumerator DisplayGameOverMenu()
    {   
        float mStartTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - mStartTime < GameEndDelay)
        {
            yield return null;
        }
        ui.ShowGameOverScreen();
    }
}
