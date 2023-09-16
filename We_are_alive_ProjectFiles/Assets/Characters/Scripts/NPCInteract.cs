using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public DialogueTrigger script;
    public GameObject dialogueBox;


    public void activate(){
        dialogueBox.SetActive(true);
        script.TriggerDialogue();  
    }



}
