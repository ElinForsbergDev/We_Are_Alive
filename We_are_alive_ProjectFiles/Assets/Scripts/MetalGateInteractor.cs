using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalGateInteractor : MonoBehaviour
{
    private bool activated = false;
    [SerializeField] private Vector3 moveVector;
    [SerializeField] private GameObject moveDownGroup1;
    [SerializeField] private GameObject moveDownGroup2;
    [SerializeField] private BoxCollider collider1;
    [SerializeField] private BoxCollider collider2;
    [SerializeField] private GameObject spark1;
    [SerializeField] private GameObject spark2;
    [SerializeField] private GameObject spark3;
    [SerializeField] private GameObject spark4;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 objectPos = moveDownGroup1.transform.position;

        if (activated && objectPos.y > -1.5) {
            moveDownGroup1.transform.Translate(moveVector * Time.deltaTime);
            moveDownGroup2.transform.Translate(moveVector * Time.deltaTime);
        }    
    }

    public void activate() {

        if (!activated && canOpen()) { 
            activated = true;
            gameObject.tag = "Untagged";
            collider1.enabled = false;
            collider2.enabled = false;
        }
        
    }

    private bool canOpen() {
        string wiresCollected = PlayerPrefs.GetString("wires_collected");

        if (!wiresCollected.Contains('r')) {
            emitParticles(spark1);
        }
        if (!wiresCollected.Contains('g')) {
            emitParticles(spark2);
        }
        if (!wiresCollected.Contains('b')) {
            emitParticles(spark3);
        }
        if (!wiresCollected.Contains('y')) {
            emitParticles(spark4);
        }

        bool canOpen = wiresCollected.Contains('r') && wiresCollected.Contains('g') && wiresCollected.Contains('b') && wiresCollected.Contains('y');
        if (!canOpen)
        {
            audioSource.Play();
        }
        return canOpen;
    }

    private void emitParticles(GameObject spark) {
        ParticleSystem[] sparkParticles = spark.GetComponentsInChildren<ParticleSystem>();
        
        foreach (ParticleSystem particle in sparkParticles) {
            particle.Emit(30);
        }
    }

}
