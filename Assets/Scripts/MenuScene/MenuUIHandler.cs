using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIHandler : MonoBehaviour
{
    private Slider volumeSlider;

    private void Start()
    {
        volumeSlider = gameObject.transform.Find("Volume").GetComponent<Slider>();
        volumeSlider.value = MusicPlayer.Instance.musicPlayer.volume;
    }


    public void OnStartGameButtonClick()
    {
        SceneManager.LoadScene(1);
    }


    public void OnVolumeChange()
    {
        MusicPlayer.Instance.SetVolume(volumeSlider.value);
    }
}
