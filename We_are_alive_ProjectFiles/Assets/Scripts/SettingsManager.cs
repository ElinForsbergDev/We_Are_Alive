using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;
    public Camera playerCamera;                   // Remove?

    public Dictionary<string,KeyCode> keyBinds;
    public GameObject[] keyInputs;
    private string buttonToRebindName = null;
    private Button buttonToRebind;

    public KyleController kyleCon;

    public Slider brightnessSlider;
    public TextMeshProUGUI brightnessText; 
    public Light skyLight;
    public Color darkSky = new Color(70f / 255, 93f / 255, 128f / 255);
    public Color brightSky = new Color(222f/255, 222f / 255, 222f / 255);

    public Toggle fpsToggle;
    public Text fpsText;

    void Start(){
        
        keyBinds = new Dictionary<string, KeyCode>();
        keyBinds.Add( "FlashLight", (KeyCode)PlayerPrefs.GetInt("FlashLight", (int)KeyCode.F));
        keyBinds.Add( "Camera", (KeyCode)PlayerPrefs.GetInt("Camera", (int)KeyCode.X));
        keyBinds.Add( "Jump", (KeyCode)PlayerPrefs.GetInt("Jump", (int)KeyCode.Space));
        keyBinds.Add( "ESC", (KeyCode)PlayerPrefs.GetInt("ESC", (int)KeyCode.Escape));
        keyBinds.Add( "Interact", (KeyCode)PlayerPrefs.GetInt("Interact", (int)KeyCode.E));
        keyBinds.Add( "Sprint", (KeyCode)PlayerPrefs.GetInt("Sprint", (int)KeyCode.LeftShift));
        keyBinds.Add( "Crouch", (KeyCode)PlayerPrefs.GetInt("Crouch", (int)KeyCode.C));
        SaveControlKeybinds();

        if (PlayerPrefs.GetFloat("volume", -1) != -1) {
            ChangeVolume(PlayerPrefs.GetFloat("volume"));
            volumeSlider.value = PlayerPrefs.GetFloat("volume");
        } else {
            volumeSlider.value = AudioListener.volume;
        }

        if (PlayerPrefs.GetFloat("brightness", -1) != -1) {
            ChangeBrightness(PlayerPrefs.GetFloat("brightness"));
            brightnessSlider.value = PlayerPrefs.GetFloat("brightness");
        } else {
            ChangeBrightness(0.5f);
        }
    }

    void Update () {
	
        if(buttonToRebindName != null)
        {
            kyleCon.canExit = false;
            if(Input.anyKeyDown)
            {
                // Loop through all possible keys and see if it was pressed down
                foreach(KeyCode kc in System.Enum.GetValues( typeof(KeyCode) ) )
                {
                    if(Input.GetKeyDown(kc))
                    {
                        keyBinds[buttonToRebindName] = kc;
                        buttonToRebind.GetComponentInChildren<TextMeshProUGUI>().text = kc.ToString();
                        buttonToRebindName = null;
                        kyleCon.canExit = true;
                        break;
                    }
                }

            }
        }

	}

    public void StartRebindFor(Button button)
    {
        buttonToRebindName = button.name;
        buttonToRebind = button;
    }
    
    public void ChangeVolume(float newValue){
        float newVol = AudioListener.volume;
        newVol = newValue;
        AudioListener.volume = newVol;
        volumeText.text = "Volume: " + Mathf.Round(newValue * 100) + "%";
        PlayerPrefs.SetFloat("volume", newValue);

    } 

    public void updateKeyBindsInput(){
        int i = 0;
    
        foreach(string key in keyBinds.Keys ) {
            keyInputs[i].GetComponentInChildren<TextMeshProUGUI>().text = ((KeyCode)PlayerPrefs.GetInt(key)).ToString();
            i++;
        }

    }

    public void SaveControlKeybinds () {
        foreach(string key in keyBinds.Keys ) {
 
         //cast the enum to an int
         int intRepresentation = (int) keyBinds[key];
         
         //Save the keybind
         PlayerPrefs.SetInt(key, intRepresentation);
        }
     
        //Write the changes to disk
        PlayerPrefs.Save();
    }   

    public void ShowFPSCounter() {
        if (fpsToggle.isOn) {
            fpsText.gameObject.SetActive(true);
        } else {
            fpsText.gameObject.SetActive(false);
        }
        
    }

    public void ChangeBrightness(float newValue) {
        skyLight.color = Color.Lerp(darkSky, brightSky, newValue);
        brightnessText.text = "Brightness: " + Mathf.Round(newValue * 100) + "%";
        PlayerPrefs.SetFloat("brightness", newValue);
    }
}

