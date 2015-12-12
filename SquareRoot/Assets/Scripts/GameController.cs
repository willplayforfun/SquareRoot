using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [Range(2,4)]
    public int numPlayers;
    public PlayerObject playerPrefab;

    List<PlayerObject> players;

    void Start()
    {
        players = new List<PlayerObject>();
        for(int i = 0; i < numPlayers; i++)
        {
            PlayerObject player = Instantiate(playerPrefab);
            player.SetPlayerNumber((PlayerNum)i, numPlayers);
            players.Add(player);
        }
    }
}
