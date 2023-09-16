using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartsHandeler : MonoBehaviour
{
    public GameObject partsTextBox;
    public GameObject partsPopUp;
    public GameObject miniMapPopUp;
    public GameObject cameraPopUp;
    public MapMarker exitMarker;

    public CameraController cameraController;
    public Text partsText;
    public float sec = 3f;

    public GameObject MiniMap;

    // Start is called before the first frame update
    void Awake()
    {
        partsTextBox.SetActive(false);
        partsPopUp.SetActive(false);
    }

    public void increase(string partName, bool displayPopUp){
        int partsAmount = PlayerPrefs.GetString("parts_collected").Length;
        if(partsAmount >= 1){
            partsTextBox.SetActive(true);
        }
        partsText.text = partsAmount + "/5 Parts Collected"; 

        if(partsAmount == 5){
            exitMarker.isActive = true;
        }
        
        checkIfSpecialPart(partName, displayPopUp);
        if (displayPopUp) StartCoroutine(popUp(sec,partsPopUp));
    }

    public void checkIfSpecialPart(string partName, bool displayPopUp){
        switch (partName){
            case "Antena":
                MiniMap.transform.position += new Vector3(300,0,0);
                if (displayPopUp) StartCoroutine(popUp(sec + 5f,miniMapPopUp));
                break;
            case "Eye":
                cameraController.canSwitchCam = true;
                if (displayPopUp) StartCoroutine(popUp(sec + 5f,cameraPopUp));
                break;
            default:
                break;
        }
    }

    IEnumerator popUp(float seconds, GameObject popUp){
        popUp.SetActive(true);

        yield return new WaitForSeconds(seconds);

        popUp.SetActive(false);
    }
}
