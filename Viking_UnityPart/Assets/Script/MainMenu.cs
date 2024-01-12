using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject HowToPlay;
    [SerializeField] GameObject AudioPlayer;
    [SerializeField] Slider VoiceBar;
    public float voice;

    // Update is called once per frame
    private void Update()
    {
        AudioPlayer.GetComponent<AudioSource>().volume = VoiceBar.value;
    }
    public void PlayBtn()
    {
        voice = VoiceBar.value;
        PlayerPrefs.SetFloat("voiceParameter", voice);
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
