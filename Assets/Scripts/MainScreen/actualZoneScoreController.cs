using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class actualZoneScoreController : MonoBehaviour
{
    [SerializeField] private GameObject textObject_;
    private gpsController gpsController_;
    private firebaseHandler firebaseHandlerObject_;
    private string lastZone_;
    void Awake(){
        lastZone_ = "";
        gpsController_ = GameObject.FindGameObjectsWithTag("gpsController")[0].GetComponent<gpsController>();
        firebaseHandlerObject_ = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        if(firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            updateZoneData();
        }
    }

    // Update is called once per frame
    void Update(){
        string userZone = gpsController_.getActualZoneOfUser();
        if(lastZone_ != userZone && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            updateZoneData();
        }
    }

    private void updateZoneData(){
        string userZone = gpsController_.getActualZoneOfUser();
        if(userZone == "North" || userZone == "West" || userZone == "Center"
                || userZone == "East" || userZone == "South"){
            float countOfVisitedPlacesOfActualZone = firebaseHandlerObject_.actualUser_.countOfVisitedPlacesOfZone(userZone);
            float totalPlacesOfActualZone = firebaseHandlerObject_.totalOfPlacesOfZone(userZone);
            float fractionOfVisited = countOfVisitedPlacesOfActualZone / totalPlacesOfActualZone;
            gameObject.GetComponent<Image>().fillAmount = fractionOfVisited ;
            textObject_.GetComponent<Text>().text = Math.Round(fractionOfVisited*100,2).ToString()+"%";
            lastZone_ = userZone; 
        }
    }
}