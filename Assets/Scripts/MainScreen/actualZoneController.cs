using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class actualZoneController : MonoBehaviour
{
    [SerializeField] private GameObject textObject_;
    
    private gpsController gpsController_ = null;
    private string lastZone_ = "";
    
    void Awake(){
        gpsController_ = GameObject.FindGameObjectsWithTag("gpsController")[0].GetComponent<gpsController>(); 
    }

    // Update is called once per frame
    void Update(){
        string actualZone = gpsController_.getActualZoneOfUser();
        if(lastZone_ != actualZone){
            lastZone_ = actualZone;
            textObject_.GetComponent<Text>().text = actualZone;
        }
    }
}
