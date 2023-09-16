using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWalkScript : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        anim.SetFloat("Direction", h);
        float s = Input.GetAxis("Vertical");
        anim.SetFloat("Speed", s);

        
        anim.SetBool("Crouch", Input.GetKey(KeyCode.LeftControl));
        
    }
}
