using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public GlobalUI ui;

    [Space(12)]

    [Range(2,4)]
    public int numPlayers;
    public PlayerObject playerPrefab;

    List<PlayerObject> players;

    void Start()
    {
        // spawn requisite number of players and have them configure for split screen
        players = new List<PlayerObject>();
        for(int i = 0; i < numPlayers; i++)
        {
            PlayerObject player = Instantiate(playerPrefab);
            player.SetPlayerNumber((PlayerNum)i, numPlayers);
            players.Add(player);
        }
    }

    void Update()
    {
        // global input
        

        // win condition check
        PlayerObject alivePlayer = null;
        foreach (PlayerObject player in players)
        {
            if(player.alive)
            {
                if(alivePlayer == null)
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

        if(alivePlayer != null)
        {
            // win!
            ui.SetVictoryText(alivePlayer.number);
        }
    }
}
