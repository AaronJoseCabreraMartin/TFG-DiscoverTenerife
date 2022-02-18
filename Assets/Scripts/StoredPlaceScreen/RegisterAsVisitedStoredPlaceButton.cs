using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the button that tries to register
  * as visited the chosen stored place.
  */
public class RegisterAsVisitedStoredPlaceButton : MonoBehaviour
{
    /**
      * @brief GameObject that contains the toast message that will be shown
      * to give information to the user.
      */
    [SerializeField] private GameObject toastMessageObject_;

    /**
      * @brief GameObject that contains the panel that shows if the current user
      * has visited the stored panel or not.
      */
    [SerializeField] private GameObject visitedPanelObject_;

    /**
      * @brief This method should be called when the current user press
      * the button of register as visited a stored place.
      * - If the user is not close enough, it will show a toast with the following message: 
      * "You too far away to register this place as visited".
      * - If the user is close enough but he didnt waited the enough time, it will show a
      * toast with the following message: "You have to wait to register this place as visited again".
      * - If the user is close enough and the cooldown time has finished, it counts the new
      * visit to the current stored chosen place and make a toast wiht the following
      * message: "You register this place as visited successfully". It also calls the 
      * CheckNewState method of VisiteStoredPlaceController class.
      */
    public void tryToRegisterAsVisited(){
        toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
        if(gpsController.gpsControllerInstance_.CalculateDistanceToUser(StoredPlacesController.choosenStoredPlace_.getLatitude(), 
                                                                        StoredPlacesController.choosenStoredPlace_.getLongitude()) < gameRules.getMaxDistance()){
            if(firebaseHandler.firebaseHandlerInstance_.cooldownVisitingStoredPlaceFinished(StoredPlacesController.choosenStoredPlace_)){
                //firebaseHandler.firebaseHandlerInstance_.userVisitedPlaceByName(StoredPlacesController.choosenStoredPlace_.getName());
                StoredPlacesController.choosenStoredPlace_.oneMoreVisit();
                StoredPlace.saveStoredPlace(StoredPlacesController.choosenStoredPlace_);
                toastMessageInstance.makeAnimation("You register this place as visited successfully", new Color32(76,175,80,255), 5);
                visitedPanelObject_.GetComponent<VisiteStoredPlaceController>().CheckNewState();
            }else{
                toastMessageInstance.makeAnimation("You have to wait to register this place as visited again", new Color32(255,145,15,255), 5);
            }
        }else{
            toastMessageInstance.makeAnimation("You too far away to register this place as visited", new Color32(255,0,0,255), 5);
        }
    }
}
