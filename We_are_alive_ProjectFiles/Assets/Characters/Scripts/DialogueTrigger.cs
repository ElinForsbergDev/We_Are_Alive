using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue dialogue;
	public StoryManager storyManager;

	public void TriggerDialogue ()
	{
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}

	public void TellStory(){
		if(storyManager != null){
			storyManager.showStory(dialogue);
		}
	}

}
