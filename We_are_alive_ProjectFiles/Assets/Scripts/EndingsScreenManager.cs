using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingsScreenManager : MonoBehaviour
{
    public TextMeshProUGUI endingSummary;
    public TextMeshProUGUI notCompletedText;
    public TextMeshProUGUI completedText;

    private string[] allEndings = new string[] {"Too fast", "Sacrifice", "All for nothing", "Destroyed", "Barely alive", "All alone", "The best for us" };
    void OnEnable()
    {
        // Reset text fields
        notCompletedText.text = "";
        completedText.text = "";

        int nrCompletedEndings = 0;

        // Get all completed endings
        List<string> completedEndings = new List<string>(PlayerPrefs.GetString("endings_completed").Split(","));

        foreach (string end in allEndings)
        {
            if (completedEndings.Contains(end))
            {
                completedText.text += "•" + end + "\n";
                nrCompletedEndings++;
            } else
            {
                notCompletedText.text += "•" + end + "\n";
            }
        }

        endingSummary.text = "You have received " + nrCompletedEndings + " out of " + allEndings.Length + " possible endings ";
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.SetString("parts_collected", "");
        PlayerPrefs.SetString("robots_collected", "");
        PlayerPrefs.SetFloat("level_time", Time.time);
        PlayerPrefs.SetString("endings_completed", "");
        PlayerPrefs.Save();
        OnEnable();
    }

}
