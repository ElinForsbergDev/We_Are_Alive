using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LeviInteraction : MonoBehaviour
{

    private NavMeshAgent agent;
    public LeviAnimation leviScript;
    public Transform[] positions;
    public GameObject levi;
    private Animator anim;
    private bool playedOnce = false;
    private bool stop = false;
    private bool finalDestination = false;
    private bool goodToGo = false;
    private bool going = false;

    
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.autoBraking = false;
        agent.Stop();
    }
    
    void Update () {
        
        if (goodToGo && !going) {
            Vector3 newPos = new Vector3(38.2f, 0.056f, 100.56f);
            agent.Warp(newPos);
            agent.Resume();
            goToLevi();
            going = true;
        }
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            if (!stop) {
                agent.destination = positions[1].position;
                stop = true;
            } else {
                if (!finalDestination) {
                    agent.Stop();
                    if (!playedOnce) {
                        anim.SetBool("attack", true);
                        playedOnce = true;
                    }
                } else {
                    levi.SetActive(false);
                    gameObject.SetActive(false);
                }

            }
        }
        
    }

    public void disableAttack() {
        anim.SetBool("attack", false);
        agent.Resume();
        agent.destination = positions[2].position; 
    }

    public void ResumeWalk() {
        agent.Resume();
        agent.destination = positions[3].position;
        finalDestination = true;
    }

    public void notifyLeviHit() {
        leviScript.getHit();
    }

    public void notifyLeviFollow() {
        leviScript.followHand();
    }

    public void setGoodToGo() {
        goodToGo = true;
        
    }
    
    private void goToLevi() {
        agent.destination = positions[0].position;
        
    }

}
