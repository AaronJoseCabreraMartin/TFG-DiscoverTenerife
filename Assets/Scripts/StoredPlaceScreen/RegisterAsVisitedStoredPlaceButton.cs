using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterAsVisitedStoredPlaceButton : MonoBehaviour
{
    [SerializeField] private GameObject toastMessageObject_;
    [SerializeField] private GameObject visitedPanelObject_;

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
