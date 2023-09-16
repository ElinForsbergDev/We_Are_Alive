using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniRobot : MonoBehaviour
{
    private Animator anim;
	private UnityEngine.AI.NavMeshAgent agent;
	public Transform player;

    private Animator animator;
    private bool activated = false;

    void Awake() 
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		anim = gameObject.GetComponent<Animator>();
		
		float stopDistance = Random.Range(2f,5f);
		agent.stoppingDistance = stopDistance;
	}

    // Update is called once per frame
    void Update()
    {
        if(activated){
			anim.SetBool("Open_Anim", true);
		
			if (player != null)
                agent.SetDestination(player.position);

			if(agent.remainingDistance > 8f){

				anim.SetBool("InstaRoll_Anim", true);
				anim.SetBool("Roll_Anim", true);
				agent.speed = 6f;
				
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
    }

    public void activate(){
        activated = true;
        gameObject.tag = "MiniRobot";
    }
}
