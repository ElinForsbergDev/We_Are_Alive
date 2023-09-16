using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsInteractor : MonoBehaviour
{
    public PartsHandeler handeler;
    public char ID;

    private void Start()
    {
       if (PlayerPrefs.GetString("parts_collected").Contains(ID))
        {
            this.gameObject.GetComponent<MapMarker>().isActive = false;
            transform.position += new Vector3(0, -10, 0);
            handeler.increase(this.name, false);
        }
    }
    public void activate(){
        PlayerPrefs.SetString("parts_collected", PlayerPrefs.GetString("parts_collected") + ID);
        this.gameObject.GetComponent<MapMarker>().isActive = false;
        handeler.increase(this.name, true);
        transform.position += new Vector3(0,-10,0);
        
    }
}
