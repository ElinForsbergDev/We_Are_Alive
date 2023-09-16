using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFreeAnim : MonoBehaviour {

	private Vector3 rot = Vector3.zero;
	private float rotSpeed = 40f;
	private Animator anim;
	private UnityEngine.AI.NavMeshAgent agent;
	public Transform player;

	private GameObject triggeringRobot;
    private bool triggering;

    public GameObject button;
    private Animator animator;

	private bool activated = false;

	// Use this for initialization
	void Awake() 
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		anim = gameObject.GetComponent<Animator>();
		
		float stopDistance = Random.Range(3f,6f);
		agent.stoppingDistance = stopDistance;
	}

	// Update is called once per frame
	void Update()
	{
		//CheckKey();
		//gameObject.transform.eulerAngles = rot;

		if(activated){
			anim.SetBool("Open_Anim", true);
		
			if (player != null)
                agent.SetDestination(player.position);

			if(agent.remainingDistance > 8f){

				anim.SetBool("InstaRoll_Anim", true);
				anim.SetBool("Roll_Anim", true);
				agent.speed = 8f;
				
			}
            else if (agent.remainingDistance > agent.stoppingDistance){
				anim.SetBool("InstaRoll_Anim", false);
				anim.SetBool("Roll_Anim", false);
				anim.SetBool("Walk_Anim", true);
				agent.speed = 2f;
	
			}
	
            else{
				anim.SetBool("Walk_Anim", false);
				anim.SetBool("InstaRoll_anim", false);
			}
				
		}
		else{
			checkIfInteracted();
		}

		
	}

	void checkIfInteracted(){
		RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5f)){
            if(hit.collider.tag == "MiniRobot"){
                button.SetActive(true);
				float dist = Vector3.Distance(Camera.main.transform.position, transform.position);
                if(Input.GetKeyDown(KeyCode.E) && dist < 5f){
					activated = true; 
                }
				button.SetActive(false);
				
            }
            else{
                button.SetActive(false);
                
            }
        }
        else{
            button.SetActive(false);
        }
	}


	/*
	void CheckKey()
	{
		// Walk
		if (Input.GetKey(KeyCode.W))
		{
			anim.SetBool("Walk_Anim", true);
		}
		else if (Input.GetKeyUp(KeyCode.W))
		{
			anim.SetBool("Walk_Anim", false);
		}

		// Rotate Left
		if (Input.GetKey(KeyCode.A))
		{
			rot[1] -= rotSpeed * Time.fixedDeltaTime;
		}

		// Rotate Right
		if (Input.GetKey(KeyCode.D))
		{
			rot[1] += rotSpeed * Time.fixedDeltaTime;
		}

		// Roll
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (anim.GetBool("Roll_Anim"))
			{
				anim.SetBool("Roll_Anim", false);
			}
			else
			{
				anim.SetBool("Roll_Anim", true);
			}
		}

		// Close
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			if (!anim.GetBool("Open_Anim"))
			{
				anim.SetBool("Open_Anim", true);
			}
			else
			{
				anim.SetBool("Open_Anim", false);
			}
		}
	}
	*/

}
