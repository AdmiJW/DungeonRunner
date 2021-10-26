using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIHandler : MonoBehaviour
{
    public void OnStartGameButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}
