using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void playGame(){
        Time.timeScale = 1;
        ResetPlayerPrefs();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void exitGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void reloadGame(){
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.SetString("parts_collected", "");
        PlayerPrefs.SetString("robots_collected", "");
        PlayerPrefs.SetString("wires_collected", "");
        PlayerPrefs.SetString("triggers_activated", "");
        PlayerPrefs.GetFloat("level_time", Time.time);
    }
}
