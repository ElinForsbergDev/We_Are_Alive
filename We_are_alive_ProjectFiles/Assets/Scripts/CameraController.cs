using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera topDownCamera;

    private bool camSwitch = true;
    public bool canSwitchCam = false;

    public FirstPersonController movementScript;
    public TutorialScreen tutorialManager;

    public GameObject joint;
    public GameObject headJoint;

    public GameObject head;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canSwitchCam){
            if(Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Camera"))){
                camSwitch = !camSwitch;
                firstPersonCamera.gameObject.SetActive(camSwitch);
                topDownCamera.gameObject.SetActive(!camSwitch);
                if(!camSwitch){
                    movementScript.rb.velocity = Vector3.zero;
                    movementScript.playerCanMove = false;
                    topDownCamera.transform.position = new Vector3(this.transform.position.x,topDownCamera.transform.position.y,this.transform.position.z);
                    head.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                else{
                    movementScript.playerCanMove = true;
                    head.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
        }
    
    }

    public void AnimationCompleted(){
        firstPersonCamera.transform.parent = joint.transform;
        firstPersonCamera.transform.localPosition = new Vector3(0,0,0);
        movementScript.cameraCanMove = true;
        movementScript.playerCanMove = true;
        movementScript.enableJump = true;
        movementScript.enableCrouch = true;
        movementScript.enableZoom = true;
        movementScript.enableSprint = true;
        tutorialManager.showTutorial();

    }
}
