using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScreenScript : MonoBehaviour
{
    public TextMeshProUGUI endingTypeText;

    public int tooFastTime = 60;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nrEndingsText;
    public TextMeshProUGUI nextText;

    // Endings
    //  Parts
    [TextArea(3,10)]
    public string[] endNothing;
    public string[] endSomeParts;
    public string[] endAllParts;
    public string[] endAllPartsAllRobots;

    //  Time
    [TextArea(3, 10)]
    public string[] endTooFast;

    // Parts and robots
    [TextArea(3, 10)]
    public string[] endNoPartsSomeRobots;
    public string[] endNoPartsAllRobots;



    private int TOTAL_ENDINGS = 6;

    private Queue<string> sentences;

    private bool switchToGameWorld;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        int partsCollected = PlayerPrefs.GetString("parts_collected").Length;
        int robotsCollected = PlayerPrefs.GetString("robots_collected").Length;
        float completionTime = PlayerPrefs.GetFloat("level_time");


        string endingTitle;

        // Ending too fast 
        if (Time.time - completionTime < tooFastTime)
        {
            endingTitle = "Too fast";
            sentences = new Queue<string>(endTooFast);
        }
        // Ending no parts
        else if (partsCollected == 0)
        {
            // Ending no parts all robots
            if (robotsCollected == 10)
            {
                endingTitle = "Sacrifice";
                sentences = new Queue<string>(endNoPartsAllRobots);

            // Ending no parts some robots
            } else if (robotsCollected > 0) {
                endingTitle = "All for nothing";
                sentences = new Queue<string>(endNoPartsSomeRobots);

            // Ending no parts no robots
            } else
            {
                endingTitle = "Destroyed";
                sentences = new Queue<string>(endNothing);
            }
        }
        // Ending Some parts collected
        else if (1 <= partsCollected && partsCollected <= 4)
        {
            endingTitle = "Barely alive";
            sentences = new Queue<string>(endSomeParts);
        }
        // Ending all parts collected
        else if (partsCollected == 5 && robotsCollected == 10) {
            endingTitle = "The best for us";
            sentences = new Queue<string>(endAllPartsAllRobots);
        }
        // Ending all parts collected
        else if (partsCollected == 5)
        {
            endingTitle = "All alone";
            sentences = new Queue<string>(endAllParts);
        }
        else
        {
            endingTitle = "No ending";
            sentences = new Queue<string>();
            sentences.Enqueue("No valid ending");
        }


        // Get all completed endings
        List<string> endings = new List<string>(PlayerPrefs.GetString("endings_completed").Split(","));

        // If player has already completed ending, change title and don't add to endings list.
        if (endings.Contains(endingTitle))
        {
            endingTypeText.text = "You have already recieved the ending:\n" + endingTitle;
        } else
        {
            endings.Add(endingTitle);
            endingTypeText.text = "You recieved ending: " + endingTitle;
        }


        Debug.Log("Parts: " + partsCollected + " Robots: " + robotsCollected + " Endings: " + string.Join(",", endings));

        nrEndingsText.text = "Endings completed: \n" + (endings.Count - 1) + " / " + TOTAL_ENDINGS;

        PlayerPrefs.SetString("endings_completed", string.Join(",", endings).Trim());
        PlayerPrefs.Save();
        
        ResetPlayerPrefs();

        nextText.enabled = true;
        DisplayNextSentence();

        // Scene 
        ResetPlayerPrefs();
        StartCoroutine(LoadAsynchronously("GameWorld"));
    }
    private void Update()
    {
        //Continue with return and space key
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }
    }

    void DisplayNextSentence()
    {
        if (sentences.Count == 0) return;
        
        string sentence = sentences.Dequeue();
        if (sentences.Count == 0) nextText.enabled = false;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    //Typewriter effect
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            yield return new WaitForSeconds(0.05f);
            dialogueText.text += letter;
            yield return null;
        }
    }

    IEnumerator LoadAsynchronously(string name)
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        operation.allowSceneActivation = false;

        while (!switchToGameWorld)
        {
            yield return null;
        }
        
        operation.allowSceneActivation = true;
    }

    public void mainMenu()
    {
        ResetPlayerPrefs();
        SceneManager.LoadScene("menu");
    }

    public void tryAgain()
    {
        ResetPlayerPrefs();
        switchToGameWorld = true;
    }

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.SetString("parts_collected", "");
        PlayerPrefs.SetString("robots_collected", "");
        PlayerPrefs.GetFloat("level_time", Time.time);
        PlayerPrefs.Save();
    }
}
