using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class LightFlicker : MonoBehaviour
{
    public new Light light;
    public GameObject emissionObj;

    float defaultIntensity;
    Renderer rend;
    Color defaultColor;
    Color offColor = new Color(29f / 255f, 29f / 255f, 29f / 255f);

    void Start()
    {
        // External or internal light
        if (light == null)
        {
            light = GetComponent<Light>();
        }
        defaultIntensity = light.intensity;

        rend = emissionObj.GetComponent<Renderer>();
        defaultColor = rend.material.GetColor("_EmissionColor");

        StartCoroutine(UpdateLight());
    }

    IEnumerator UpdateLight()
    {
        while(true)
        {
            
            yield return new WaitForSeconds(Random.Range(6f,12f));

            //for (int i = 0; i <= Random.Range(1,2); i++)
            //{
                light.intensity = 0;
                rend.material.SetColor("_EmissionColor", offColor);

                yield return new WaitForSeconds(0.08f);

                light.intensity = defaultIntensity;
                rend.material.SetColor("_EmissionColor", defaultColor);

                yield return new WaitForSeconds(0.05f);
            //}
            

            yield return null;
        }
        

    }

}
