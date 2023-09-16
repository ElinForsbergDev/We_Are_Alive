using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEggScript : MonoBehaviour
{
    private Camera animationCamera;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject playerKyle;
    [SerializeField] private GameObject animationKyle;
    [SerializeField] private AudioSource audio;
    [SerializeField] private float animationTime;
    [SerializeField] private Light spotLight;
    private Animator kyleAnimator;
    private float animationTimer = 0;


    // Start is called before the first frame update
    void Start()
    {
        animationCamera = transform.GetChild(2).GetComponent<Camera>();
        animationKyle = transform.GetChild(1).gameObject;
        kyleAnimator = animationKyle.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() { }

    // Activates the animation
    public void activate() {
        StartCoroutine(playAnimation());
    }

    // Plays the animation
    private IEnumerator playAnimation() {
        kyleAnimator.SetTrigger("Active"); // Set animation

        // Change camera
        playerCamera.gameObject.SetActive(false);
        animationCamera.gameObject.SetActive(true);

        // Swap kyle model
        animationKyle.transform.position = playerKyle.transform.position - new Vector3(0, 1, 0); // The minus vector is to not have the animation kyle be in the ground I think
        spotLight.transform.position = playerKyle.transform.position + new Vector3(0, 3, 0);
        animationKyle.transform.LookAt(animationCamera.transform);              // This could do with a bit of work as the kyle faces upwards towards the camera in some cases
        playerKyle.transform.position = new Vector3(930, 4, 160);

        audio.PlayOneShot(audio.clip); // Play song

        while (animationTimer < 9.6f) {
            animationTimer += Time.deltaTime;
            yield return null;
        }

        // Change things back again
        playerCamera.gameObject.SetActive(true);
        animationCamera.gameObject.SetActive(false);
        playerKyle.transform.position = animationKyle.transform.position + new Vector3(0, 1, 0);
        spotLight.transform.position = new Vector3(0, 0, 0);
        gameObject.SetActive(false);
    }
}
