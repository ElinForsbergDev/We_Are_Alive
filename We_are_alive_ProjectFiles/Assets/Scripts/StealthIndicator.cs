using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StealthIndicator : MonoBehaviour
{
    private Slider slider;
    private RectTransform innerIndicatorPosition;
    private RectTransform outerindicatorPosition;
    private RectTransform holderPosition;
    private bool playingFoundAnimation = false;
    private float animationTimer = 0f;
    private Image colorBar;
    private Image arrow;
    [SerializeField] private Color filledColor = new Color(255, 0, 0, 145);
    [SerializeField] private Color emptyColor = new Color(255, 255, 255, 145);
    [SerializeField] private Color arrowColor = new Color(255, 255, 255, 120);
    [SerializeField] private AudioSource audio;

    // Start is called before the first frame update
    void Awake()
    {
        // Setup all components
        slider = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>();
        colorBar = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        innerIndicatorPosition = transform.GetChild(0).GetComponent<RectTransform>();
        //innerIndicatorPosition.localPosition = Vector3.zero;

        outerindicatorPosition = transform.GetChild(1).GetComponent<RectTransform>();
        arrow = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        //outerindicatorPosition.localPosition = Vector3.zero;

        holderPosition = transform.GetComponent<RectTransform>();
        holderPosition.localPosition = Vector3.zero;

        slider.maxValue = 2f; 
        slider.minValue = 0f;
    }


    // Sets the slider value 
    public void SetSliderValue(float value) {
        transform.gameObject.SetActive(true);

        float sliderValue = Mathf.Clamp(value, 0, 1);
        slider.value = sliderValue / 4;
        //Debug.Log(Color.Lerp(emptyColor, filledColor, sliderValue));
        colorBar.color = Color.Lerp(emptyColor, filledColor, sliderValue);
        if (value >= 1f) {
            arrow.color = new Color(1, 170f / 255f, 170f / 255f, 80f / 255f);
        } else {
            arrow.color = arrowColor;
        }
    }

    // Sets the rotation so it is rotated towards a guard
    public void SetRoationToGuard(Quaternion rotation) {
        transform.gameObject.SetActive(true);
        innerIndicatorPosition.localRotation = rotation;
        innerIndicatorPosition.Rotate(new Vector3(0, 0, (90 * slider.value) - 90));

        outerindicatorPosition.localRotation = rotation;
        outerindicatorPosition.Rotate(new Vector3(0, 0, 22.5f));
    }

    // Removes the indicator from the screen
    public void removeIndicator() {
        if (!playingFoundAnimation) {
            transform.gameObject.SetActive(false);
            arrow.color = arrowColor;
        }
    }

    // When a guard sees the player, the stealth bar will animate a bit to show that the player was found
    public void playFoundAnimation() {
        transform.gameObject.SetActive(true);
        if (!playingFoundAnimation) {
            StartCoroutine(foundAnimation());
        }
    }

    // Playes the animation
    private IEnumerator foundAnimation() {
        playingFoundAnimation = true;
        audio.PlayOneShot(audio.clip);
        while (animationTimer < 0.5f) {
            float size = (Mathf.Sin(animationTimer * 32 - (Mathf.PI / 2)) * 0.015f) + 1.015f;
            outerindicatorPosition.localScale = new Vector3(size, size, size);
            animationTimer += Time.deltaTime;
            yield return null;
        }

        arrow.color = new Color(1, 170f / 255f, 170f / 255f, 80f / 255f);
        outerindicatorPosition.localScale = Vector3.one;
        animationTimer = 0;
        playingFoundAnimation = false;
    }
}
