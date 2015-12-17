using UnityEngine;
using System.Collections;

public class AudioClipManager : MonoBehaviour
{
    public static AudioClipManager instance;

    public AudioClip OnFireSound;
    public AudioClip HitRockSound;
    public AudioClip NewBranchSound;
    public AudioClip SplitSound;
    public AudioClip LeechingSound;
    public AudioClip SelectSound;
    public AudioClip BackSound;
    public AudioClip BackgroundMusic;
    public AudioClip DrawSound;
    public AudioClip ElimSound;
    public AudioClip VictorySound;
    public AudioClip CutSound;
    public AudioClip HitTendrilSound;

    void Awake()
    {
        instance = this;
    }
}
