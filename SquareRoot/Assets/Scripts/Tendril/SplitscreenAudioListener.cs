using UnityEngine;
using System.Collections.Generic;

public class SplitscreenAudioListener : MonoBehaviour {
    /*
     * Adjusts Volume of AudioSources to scale linearly relative to their distance from nearest Player Camera
     * AudioSources that require this should register themselves with this object through RegisterAudioSource()
     */
    List<Camera> PlayerCameras = new List<Camera>();
    AudioListener listener;
    public float maxAudibleDistance = 1000.0f;
    public List<AudioSource> mAudioSources = new List<AudioSource>();

	// Use this for initialization
	void Start () {
	   foreach(PlayerObject player in GameObject.FindObjectsOfType<PlayerObject>()){
           PlayerCameras.Add(player.camera); 
       }
	}
	
	// Update is called once per frame
	void Update () {
        //AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();
        foreach(AudioSource source in mAudioSources){
            float dist = DistanceToClosestCamera(source.transform.position);
            float volume = 1 - dist/maxAudibleDistance;
            volume = Mathf.Clamp01(volume);
            source.volume = volume;
        }
	}

    float DistanceToClosestCamera(Vector3 position){
        float closestDist = float.MaxValue;
        foreach (Camera cam in PlayerCameras){
            float distToCam = Vector3.Distance(position, cam.transform.position);
            closestDist = closestDist <  distToCam ? closestDist : distToCam;
        }
        return closestDist;
    }

    public void RegisterAudioSource(AudioSource newSource)
    {
        mAudioSources.Add(newSource);
    }

    public void UnregisterAudioSource(AudioSource source)
    {
        mAudioSources.Remove(source);
    }
    
}
