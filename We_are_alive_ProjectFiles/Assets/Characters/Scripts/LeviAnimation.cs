using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeviAnimation : MonoBehaviour
{
    private Animator anim;
    public Transform guardHand;
    private bool follow = false;

    void Start(){
        anim = GetComponent<Animator>();
    } 

    public void getHit(){
        anim.SetBool("Hit", true);
    }

    public void followHand(){
        transform.Rotate(-90.0f,0.0f,0.0f);
        follow = true;
    }

    void Update() {
        if(follow){
            transform.position = new Vector3(guardHand.position.x, guardHand.position.y - 3.0f, guardHand.position.z);
            
        }
    }
}
