using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyleController : MonoBehaviour
{
    
    private Animator anim;
    private FirstPersonController controller;
    private Camera cam;

    public GameObject exitMenu;
    public GameObject lostMenu;
    public GameObject settingsMenu;
    public GameObject keyBindsMenu;


    public bool canExit = true;
    public bool canCrouch = true;
    public bool canSprint = true;
    private bool menuUp;

    private bool sprinting = false;

    private Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        
        controller = GetComponentInParent<FirstPersonController>();
        cam = GameObject.Find("PlayerCamera").GetComponent<Camera>();

	originalScale = transform.localScale;
        //cam = GameObject.Find("PlayerCamera");
    }

    // Update is called once per frame
    void Update()
    {
    
        float inputDirection = Input.GetAxis("Horizontal");
        float direction = Mathf.Min(Mathf.Abs(inputDirection), 0.5f);
        float inputSpeed = Input.GetAxis("Vertical");
        float speed = Mathf.Min(Mathf.Abs(inputSpeed), 0.5f);   
        if (canSprint && Input.GetKey((KeyCode)PlayerPrefs.GetInt("Sprint")))
        {
            speed = Mathf.Min(Mathf.Abs(inputSpeed),1);
            direction = Mathf.Min(Mathf.Abs(inputDirection),1);  
        }
        if (canCrouch && controller.GetIsCrouching()) //Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("Crouch")))
        {
            anim.SetBool("Crouch", true);
            direction = Mathf.Min(Mathf.Abs(inputDirection), 0.3f);
            speed = Mathf.Min(Mathf.Abs(inputSpeed), 0.3f);
            transform.localScale = new Vector3(1, 3f, 1); // + (transform.localScale.y - transform.lossyScale.y)
        }
        if (canCrouch && !controller.GetIsCrouching()) //Input.GetKeyUp((KeyCode)PlayerPrefs.GetInt("Crouch")))
        {
            anim.SetBool("Crouch", false);
            transform.localScale = originalScale;
        }
        anim.SetFloat("Speed", Mathf.Sign(inputSpeed) * speed);
        anim.SetFloat("Direction", Mathf.Sign(inputDirection) * direction);  
        anim.SetBool("Jump", !controller.GetIsGrounded());
        if(speed > 0.5f){
            setSprinting();
        }
        else{
            if(sprinting){
                sprinting = false;
                this.transform.localPosition += new Vector3(0,0,0.3f);
            }
        }

        if(canExit){
                // Exit menu    
            if(Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("ESC"))){
                if(menuUp){
                    exitMenu.SetActive(false);
                    settingsMenu.SetActive(false);
                    keyBindsMenu.SetActive(false);
                    canCrouch = true;
                    canSprint = true;
                    controller.enableCrouch = true;
                    Time.timeScale = 1;
                    menuUp = false;
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                    controller.cameraCanMove = true;
                }
                else{
                    exitMenu.SetActive(true);
                    canCrouch = false;
                    canSprint = false;
                    controller.enableCrouch = true;
                    Time.timeScale = 0;
                    menuUp = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    controller.cameraCanMove = false;

                }
        
            }
        }
    
    }

    private void setSprinting(){
        if(!sprinting){
            this.transform.localPosition += new Vector3(0,0,-0.3f);
            sprinting = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Enemy"){
            Time.timeScale = 0;
            lostMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
           
            controller.cameraCanMove = false;

        }
    }
}