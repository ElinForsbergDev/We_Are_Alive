using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryManager : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public LevelLoader loader;
    public GameObject textHolder;
    public GameObject LoadingScreen;
    public GameObject loadingHolder;
    public GameObject[] images;
    private Queue<string> lines;
    private bool showNextline = false;
    private bool activateLoader;
    
    // Start is called before the first frame update
    void Start()
    {
        lines = new Queue<string>();
        activateLoader = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(LoadingScreen.active){
            if(activateLoader){
                activateLoader = false;
                StartCoroutine(loader.LoadAsynchronously(1));
                
            }

            if(Input.GetKeyDown(KeyCode.E)){
                DisplayNextLine();
            }
        }
        

    }

    public void showStory(Dialogue story){
        
        lines.Clear();
 
        foreach(string line in story.sentences)
        {
            lines.Enqueue(line);
        }
        DisplayNextLine();
        
      
        
    }

    public void DisplayNextLine()
    {
        int index;
        if (lines.Count == 0)
        {
            textHolder.SetActive(false);
            loadingHolder.transform.localPosition = new Vector3(260f,4f,0f);
            return;
        }
        index = 7 - lines.Count;
        images[index].SetActive(true);
        string line = lines.Dequeue();
        storyText.text = line;
        
    }
}
