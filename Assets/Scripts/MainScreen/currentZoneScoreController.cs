using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that shows the current zone current user's score
  */
public class currentZoneScoreController : MonoBehaviour
{
    /**
      * @brief GameObject that contains the text where the score will be showed.
      */
    [SerializeField] private GameObject textObject_;

    /**
      * @brief store a reference to the gpsController.
      */
    private gpsController gpsController_;//WTF no necesita guardarlo, ademas no comprueba si es nulo.
    
    /**
      * @brief store a reference to the firebaseHandler instance.
      */
    private firebaseHandler firebaseHandlerObject_;

    /**
      * @brief stores the last zone that the user was, for optimization reasons.
      */
    private string lastZone_;

    /**
      * @brief This method is called on the first frame, it initializes each property and 
      * if all FirebaseDependenciesAreResolved, userDataIsReady and placesAreReady methods
      * of firebaseHandler class returned true, it calls the updateZoneData method.
      */
    void Awake(){
        lastZone_ = "";
        gpsController_ = gpsController.gpsControllerInstance_;
        firebaseHandlerObject_ = firebaseHandler.firebaseHandlerInstance_;
        if(firebaseHandlerObject_.FirebaseDependenciesAreResolved() && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            updateZoneData();
        }
    }

    /**
      * This method is called on each frame, it checks the following conditions:
      * - If the user has changed the zone
      * - If FirebaseDependenciesAreResolved method of firebaseHandler return true
      * - If placesAreReady method of firebaseHandler return true
      * - If userDataIsReady method of firebaseHandler return true
      *
      * If all the conditions are true, it calls the updateZoneData method.
      */
    void Update(){
        string userZone = gpsController_.getcurrentZoneOfUser();
        if(lastZone_ != userZone &&  firebaseHandlerObject_.FirebaseDependenciesAreResolved() && 
            firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
                updateZoneData();
        }
    }

    /**
      * If the current zone is valid, it calls countOfVisitedPlacesOfZone method of UserData class with
      * the current zone and it calls the totalOfPlacesOfZone method of firebaseHandler class with the
      * current zone. Then, calculates the porportion of visited and update the text and the fillAmount
      * properties for show the score of the current zone.
      */
    private void updateZoneData(){
        string userZone = gpsController_.getcurrentZoneOfUser();
        //WTF no deberia comprobar que la zona está en el mapRulesHandler???
        if(userZone == "North" || userZone == "West" || userZone == "Center"
                || userZone == "East" || userZone == "South"){
            float countOfVisitedPlacesOfcurrentZone = firebaseHandlerObject_.currentUser_.countOfVisitedPlacesOfZone(userZone);
            float totalPlacesOfcurrentZone = firebaseHandlerObject_.totalOfPlacesOfZone(userZone);
            float fractionOfVisited = countOfVisitedPlacesOfcurrentZone / totalPlacesOfcurrentZone;
            gameObject.GetComponent<Image>().fillAmount = fractionOfVisited ;
            textObject_.GetComponent<Text>().text = Math.Round(fractionOfVisited*100,2).ToString()+"%";
            lastZone_ = userZone; 
        }
    }
}