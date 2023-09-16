using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public TextMeshProUGUI progressText;
    public GameObject loadingImage;
    public GameObject LoadingScreen;
    private bool loadingComplete = false;
    private AsyncOperation operation;
    private int index;

    private RectTransform rectComponent;
    private Image imageComp;
    private bool load = false;
    private bool starting = false;

    public float rotateSpeed = 200f;

    void Start () {
        rectComponent = loadingImage.GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();

    }

    public void LoadLevel(int sceneIndex){
       Time.timeScale = 1;
       index = sceneIndex;
       load = true;
       resetPlayerPrefs();
       
    }

    public void QuitGame() {
        PlayerPrefs.Save();
        Application.Quit();
    }

   public IEnumerator LoadAsynchronously(int sceneIndex){
        operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;


        
        while(!operation.isDone){

            if(!starting){
                progressText.text = "Loading... " + Mathf.Round((operation.progress * 100)) + "%";
            
                float currentSpeed = rotateSpeed * Time.deltaTime;
                rectComponent.Rotate(0f, 0f, currentSpeed);
            }
            

            if(operation.progress >= 0.9f){
                if(!starting){
                    progressText.text = "Press space to start/skip";
                    loadingImage.SetActive(false);
                    if(Input.GetKeyDown(KeyCode.Space)){
                        progressText.text = "Starting...";
                        operation.allowSceneActivation = true;
                        starting = true;
                    }
                }
                
            }

            yield return null;
        }
        loadingComplete = true;
        
   }

    
    private void resetPlayerPrefs()
    {
        PlayerPrefs.SetString("parts_collected", "");
        PlayerPrefs.SetString("robots_collected", "");
        PlayerPrefs.SetString("wires_collected", "");
        PlayerPrefs.SetString("triggers_activated", "");
        PlayerPrefs.GetFloat("level_time", Time.time);
    }
}
