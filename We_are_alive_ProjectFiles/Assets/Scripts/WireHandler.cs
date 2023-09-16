using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireHandler : MonoBehaviour
{

    [SerializeField] private char wireColor;
    [SerializeField] private Vector3 wireStandardPosition;
    [SerializeField] private Vector3 wireStandartRotation;


    // Start is called before the first frame update
    void Start()
    {
       
        // Set position of wire based on if it has been picked up
        if (PlayerPrefs.GetString("wires_collected").Contains(wireColor)) {
            moveWire();
        }

    }

    // Update is called once per frame
    void Update()
    {}

    public void activate() {

        PlayerPrefs.SetString("wires_collected", wireColor + PlayerPrefs.GetString("wires_collected"));
        moveWire();

        // Disables light glow
        GetComponent<SpotOverInteractables>().enabled = false;
        GetComponentInChildren<Light>().enabled = false;
        

    }

    private void moveWire() {
        gameObject.transform.position = wireStandardPosition;
        gameObject.transform.rotation = Quaternion.Euler(wireStandartRotation);
        gameObject.tag = "Untagged";
    }

}
