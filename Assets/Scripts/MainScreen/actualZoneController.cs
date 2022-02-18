using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that shows the zone that the current user is.
  */
public class actualZoneController : MonoBehaviour
{
    /**
      * @brief text where the current zone will be shown.
      */
    [SerializeField] private GameObject textObject_;
    
    /**
      * @brief stores a reference to the gpsController.
      */
    private gpsController gpsController_ = null;

    /**
      * @brief stores the last zone that the user was, for optimization reasons.
      */
    private string lastZone_ = "";
    
    /**
      * @brief this method is called before the first frame, it initializes the gpsController_ property. 
      */
    void Awake(){
        //WTF no deberia tener una propiedad para tener la referencia al gpsController, además
        //no estoy comprobando si este es null.
        gpsController_ = gpsController.gpsControllerInstance_; 
    }

    /**
      * @brief this method is called each frame, it checks if the user has changed his zone. If that is
      * the case, it update the textObject_ to show the new current zone.
      */
    void Update(){
        string currentZone = gpsController_.getActualZoneOfUser();
        if(lastZone_ != currentZone){
            lastZone_ = currentZone;
            textObject_.GetComponent<Text>().text = currentZone;
        }
    }
}
