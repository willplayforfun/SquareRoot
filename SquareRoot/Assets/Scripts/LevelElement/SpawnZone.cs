using UnityEngine;
using System.Collections.Generic;

public class SpawnZone : MonoBehaviour {
    /*
     * Dictates spawning area of a player's root nodes
     * SpawnPoints are assigned in Editor, via objects that have SpawnPoint as a component
     * Nodes are spawned at the spawnpoints in this object
     * In Start(), SpawnZone finds its corresponding player and sets that player's list of SpawnPoints to that of the SpawnZone
     */
    public SpawnPoint[] spawnPoints;
    public PlayerNum playerNumber;
}
