using UnityEngine;
using System.Collections;

public class WorldAudioSource : MonoBehaviour
{
    private SplitscreenAudioListener listener;
    private AudioSource source;

	void Start ()
    {
        listener = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<SplitscreenAudioListener>();
        source = GetComponent<AudioSource>();
    }
	
	void Update ()
    {
	    if (listener != null && source != null)
        {
            source.volume = listener.GetAudioSourceVolume(transform);
        }
	}
}
