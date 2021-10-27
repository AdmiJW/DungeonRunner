using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance = null;

    public AudioSource musicPlayer { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicPlayer = GetComponent<AudioSource>();
        }
        else Destroy(gameObject);
    }


    public void SetVolume(float volume)
    {
        musicPlayer.volume = volume;
    }
}
