using UnityEngine;
using System.Collections.Generic;

public class SplitscreenAudioListener : MonoBehaviour {
    /*
     * Adjusts Volume of AudioSources to scale linearly relative to their distance from nearest Player Camera
     * AudioSources that require this should register themselves with this object through RegisterAudioSource()
     */
    public float maxAudibleDistance = 1000.0f;

    private List<Transform> playerCameras;

    void Awake()
    {
        playerCameras = new List<Transform>();
    }

    void Start()
    {
        foreach (PlayerObject player in GameObject.FindObjectsOfType<PlayerObject>())
        {
            playerCameras.Add(player.camera.transform);
        }
    }

    public float GetAudioSourceVolume(Transform source)
    {
        float dist = DistanceToClosestCamera(source.transform.position);
        float volume = 1 - dist / maxAudibleDistance;
        volume = Mathf.Clamp01(volume);
        return volume;
    }

    private float DistanceToClosestCamera(Vector3 position){
        float closestDist = float.MaxValue;
        foreach (Transform cam in playerCameras)
        {
            float distToCam = Vector3.Distance(position, cam.transform.position);
            closestDist = closestDist <  distToCam ? closestDist : distToCam;
        }
        return closestDist;
    }   
}
