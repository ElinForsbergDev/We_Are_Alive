using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour
{
    private GameObject triggeringRobot;
    private bool triggering;

    public GameObject button;
    private Animator animator;

    void Awake() {
        animator = triggeringRobot.GetComponent<Animator>();
    }

    void Update(){
        /*
        if(triggering){
            button.SetActive(true);
            if(Input.GetKeyDown(KeyCode.E)){
                print("Interacting!");
            }
        }
        else{
            button.SetActive(false);
        }
        */

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5f)){
            if(hit.collider.tag == "MiniRobot"){
                button.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E)){
                    print("Interacting with " + hit.collider.gameObject);
                    interaction();
                }
            }
            else{
                button.SetActive(false);
                
            }
        }
        else{
            button.SetActive(false);
        }
    }

   private void interaction(){
      print("Activating");
      animator.enabled = true;
      //triggeringRobot.GetComponent<UnityEngine.AI.NavMeshAgent>().SetActive(true);
      //triggeringRobot.GetComponent<RobotFreeAnim>().SetActive(true);
    
   }
}
