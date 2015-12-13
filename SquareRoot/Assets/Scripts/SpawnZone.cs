using UnityEngine;
using System.Collections.Generic;

public class SpawnZone : MonoBehaviour {
    /*
     * Dictates spawning area of a player's root nodes
     * SpawnPoints are assigned in Editor, via objects that have SpawnPoint as a component
     * Nodes are spawned at the spawnpoints in this object
     * In Start(), SpawnZone finds its corresponding player and sets that player's list of SpawnPoints to that of the SpawnZone
     */
    public List<GameObject> mSpawnPointMarkers;
    List<SpawnPoint> mSpawnPoints = new List<SpawnPoint>();
    public PlayerNum mPlayerNum;
    PlayerObject[] players;

    void Awake()
    {
        foreach (GameObject obj in mSpawnPointMarkers)
        {
            if (obj.GetComponent<SpawnPoint>())
            {
                mSpawnPoints.Add(obj.GetComponent<SpawnPoint>());
            }
             obj.GetComponent<Renderer>().enabled = false; // stop rendering spawn markers during play
        }
    }
	// Use this for initialization
	void Start () {
        players = GameObject.FindObjectsOfType<PlayerObject>();
        foreach (PlayerObject p in players)
        {
            if (p.number == mPlayerNum)
            {
                p.SetSpawnPoints(mSpawnPoints);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
