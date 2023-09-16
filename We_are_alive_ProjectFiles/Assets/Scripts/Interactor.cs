using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Interactor : MonoBehaviour
{
    public GameObject buttonImage;
    public GameObject button;
    [SerializeField] private Light headLight;
    private Vector3 lightDistance = new Vector3(0, 1);
    public TMPro.TextMeshProUGUI textPromt;
    public Dialogue talk;
    public KyleTalking talkScript;
    private bool activatedOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage.SetActive(false);
    }
     
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5f)){
            Vector3 test = hit.collider.transform.position + lightDistance;
            button.GetComponentInChildren<TextMeshProUGUI>().text = ((KeyCode)PlayerPrefs.GetInt("Interact")).ToString();

            
            if(hit.collider.tag == "Interactable"){
                buttonImage.SetActive(true);
                textPromt.text = "Press to save robot";
                if(Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Interact"))){
					hit.collider.GetComponent<interactable>().activate();
                    if(!activatedOnce){
                        talkScript.StartTalk(talk);
                        activatedOnce = true;
                    }
                }
            }
            else if(hit.collider.tag == "NPC"){
                buttonImage.SetActive(true);
                textPromt.text = "Press to talk";
                if(Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Interact"))){
					hit.collider.GetComponent<NPCInteract>().activate();
                    hit.collider.tag = "Untagged";
                }
            }

            else if(hit.collider.tag == "Part"){
                buttonImage.SetActive(true);
                textPromt.text = "Press to collect";
                if(Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Interact"))){
					hit.collider.GetComponent<PartsInteractor>().activate();
                }

            } else if (hit.collider.tag == "MetalGate") {
                buttonImage.SetActive(true);
                textPromt.text = "Press to activate";
                if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Interact"))) {
                    hit.collider.GetComponent<MetalGateInteractor>().activate();
                }
            } else if (hit.collider.tag == "Wire") {
                buttonImage.SetActive(true);
                textPromt.text = "Press to collect";
                if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Interact"))) {
                    hit.collider.GetComponent<WireHandler>().activate();
                }
            } else if (hit.collider.tag == "EasterEgg") {
                buttonImage.SetActive(true);
                textPromt.text = "Press to activate";
                if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Interact"))) {
                    hit.collider.GetComponent<EasterEggScript>().activate();
                    buttonImage.SetActive(false);
                }
            } else {
                buttonImage.SetActive(false);
            }
        }
        else{
            buttonImage.SetActive(false);
        }

        if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("FlashLight"))) {
        headLight.enabled = !headLight.enabled;
        }
    
    }
}
