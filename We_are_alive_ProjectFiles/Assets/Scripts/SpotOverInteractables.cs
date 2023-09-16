using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotOverInteractables : MonoBehaviour
{
    public GameObject player;
    private Light light;
    public float cutOfDistance = 30;

    float distToPlayer;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // 25 to 0
        if (distToPlayer < cutOfDistance)
        {
            light.enabled = true;
            light.intensity = (cutOfDistance - distToPlayer) / 10f;
            
        } else
        {
            light.enabled = false;
        }

        
    }
}
