using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterAsVisitedButton : MonoBehaviour
{
    [SerializeField] private GameObject toastMessageObject_;
    [SerializeField] private GameObject visitedPanelObject_;

    public void tryToRegisterAsVisited(){
        gpsController gps = gpsController.gpsControllerInstance_;
        toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
        if(gps.CalculateDistanceToUser(PlaceHandler.choosenPlace_.getLatitude(), PlaceHandler.choosenPlace_.getLongitude()) < 0.05){//si esta a menos de 50m
            firebaseHandler firebaseHandlerObject = firebaseHandler.firebaseHandlerInstance_;
            if(firebaseHandlerObject.cooldownVisitingPlaceByNameFinished(PlaceHandler.choosenPlace_.getName())){
                firebaseHandlerObject.userVisitedPlaceByName(PlaceHandler.choosenPlace_.getName());
                toastMessageInstance.makeAnimation("You register this place as visited successfully", new Color32(76,175,80,255), 5);
                visitedPanelObject_.GetComponent<VisitedPanelController>().CheckNewState();
            }else{
                toastMessageInstance.makeAnimation("You have to wait to register this place as visited again", new Color32(255,145,15,255), 5);
            }
        }else{
            toastMessageInstance.makeAnimation("You too far away to register this place as visited", new Color32(255,0,0,255), 5);
        }
    }
}
