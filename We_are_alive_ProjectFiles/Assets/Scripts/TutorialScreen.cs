using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialScreen : MonoBehaviour
{
    public GameObject tutorialMenu;
    private bool startedGame;
    public TMPro.TextMeshProUGUI[] keybindsText;

    // Start is called before the first frame update
    public void showTutorial()
    {
        Time.timeScale = 0;
        foreach(TextMeshProUGUI keyText in keybindsText){
            keyText.text = ((KeyCode)PlayerPrefs.GetInt(keyText.name)).ToString();
        }
        tutorialMenu.SetActive(true);
        startedGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!startedGame){
            if(Input.anyKey){
                Time.timeScale = 1;
                tutorialMenu.SetActive(false);
                startedGame = true;
            }
        }
        
    }
}
