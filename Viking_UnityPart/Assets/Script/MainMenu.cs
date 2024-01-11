using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject HowToPlay;
    // Update is called once per frame
    public void PlayBtn()
    {
        SceneManager.LoadScene("VikingGame");
    }
    public void HelpBtn()
    {
        HowToPlay.SetActive(!HowToPlay.activeSelf);
    }
    public void ExitBtn()
    {
        Application.Quit();
    }
}
