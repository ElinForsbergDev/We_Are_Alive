using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
 
    public Text nameText;
    public Text dialogueText;
    public Image renderer;

    public GameObject dialogueBox;
    public LeviInteraction leviAgent;

    public GameObject[] parts;
    public GameObject[] characters;
 
    private Queue<string> sentences;

    private void Awake() {
        sentences = new Queue<string>();
    }
 
    public void StartDialogue(Dialogue dialogue)
    {
    
        nameText.text = dialogue.name;
        renderer.sprite = dialogue.image;
        sentences.Clear();
 
        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
 
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
 
    //Typewriter effect
    public IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }
 
    void EndDialogue()
    {
        dialogueBox.SetActive(false);

        switch(nameText.text){
            case "Levi":
                leviAgent.setGoodToGo();
                break;
            case "Edi":
                if(!parts[2].GetComponent<MapMarker>().beenActivated){
                    parts[2].GetComponent<MapMarker>().isActive = true;
                }
                characters[3].tag = "NPC";
                break;
            case "Robot":
                if(!parts[1].GetComponent<MapMarker>().beenActivated){
                    parts[1].GetComponent<MapMarker>().isActive = true;
                }
                characters[2].tag = "NPC";
                break;
            case "Model_247":
                if(!parts[0].GetComponent<MapMarker>().beenActivated){
                    parts[0].GetComponent<MapMarker>().isActive = true;
                }
                characters[0].tag = "NPC";
                break;
            case "Maid":
                if(!parts[3].GetComponent<MapMarker>().beenActivated){
                    parts[3].GetComponent<MapMarker>().isActive = true;
                }
                characters[1].tag = "NPC";
                break;
            default:
                break;
        }
    
    }
 
    void Update()
    {
        //Continue with return and space key
        if(Input.anyKeyDown && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.D))
        {
            DisplayNextSentence();
        }

    }
}
