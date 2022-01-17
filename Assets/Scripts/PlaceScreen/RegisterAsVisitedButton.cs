using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterAsVisitedButton : MonoBehaviour
{
    [SerializeField] private GameObject toastMessageObject_;
    [SerializeField] private GameObject visitedPanelObject_;

    public void tryToRegisterAsVisited(){
        toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
        if(!firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            toastMessageInstance.makeAnimation("You don't have internet conection, try to store the place and visit it on the offline mode", new Color32(255,0,0,255), 5);
        }else{
            if(gpsController.gpsControllerInstance_.CalculateDistanceToUser(PlaceHandler.choosenPlace_.getLatitude(), PlaceHandler.choosenPlace_.getLongitude()) < gameRules.getMaxDistance()){
                if(firebaseHandler.firebaseHandlerInstance_.cooldownVisitingPlaceByNameFinished(PlaceHandler.choosenPlace_.getName())){
                    firebaseHandler.firebaseHandlerInstance_.userVisitedPlaceByName(PlaceHandler.choosenPlace_.getName());
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
}
