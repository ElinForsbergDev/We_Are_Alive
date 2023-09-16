using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyleTalkTrigger : MonoBehaviour
{
    public Dialogue talk;
    public char id;
    public KyleTalking talkScript;
    private bool activatedOnce = false;
    
    void Start() {
        if(PlayerPrefs.GetString("triggers_activated").Contains(id)){
            activatedOnce = true;
        }
        //update
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && !activatedOnce){
            talkScript.StartTalk(talk);
            PlayerPrefs.SetString("triggers_activated", PlayerPrefs.GetString("triggers_activated") + id);
            activatedOnce = true;
        }
    }
}
