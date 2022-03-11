using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the button that allows the user visit a place.
  */
public class RegisterAsVisitedButton : MonoBehaviour
{

    /**
      * @brief Reference to the GameObject that has the toast message that shows information
      * to the user.
      */
    [SerializeField] private GameObject toastMessageObject_;

    /**
      * @brief Reference to the GameObject that shows if the user has visited or not the chosen place.
      */
    [SerializeField] private GameObject visitedPanelObject_;

    /**
      * @brief This method should be called only when the register as visited button is
      * pressed. 
      * - If you dont have internet connection it will show an error on a toast message.
      * - If the user is close enough it calls the userVisitedPlaceByName
      * method of firebaseHandler, the CheckNewState of the VisitedPanelController class and 
      * shows some toast messages giving information to the user.
      * - If the user is close enough but it cant visit again that place it shows a toast
      * message telling to the user that he has to wait.
      * - If the user is not close enough it shows a toast message with the error, telling the user 
      * that he has to be more more close to mark it as visited.
      * 
      * It also checks if the user has level up his range, if that is the case it will show a
      * toast message showing his new range.
      *
      * How many time the user has to wait between visiting the same place twice is defined on
      * the gameRules class, as well as how close the user has to be to register a place as visited.
      */
    public void tryToRegisterAsVisited(){
        toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
        if(!firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            toastMessageInstance.makeAnimation("You don't have internet conection, try to store the place and visit it on the offline mode", new Color32(255,0,0,255), 5);
        }else{
            if(gpsController.gpsControllerInstance_.CalculateDistanceToUser(PlaceHandler.chosenPlace_.getLatitude(), PlaceHandler.chosenPlace_.getLongitude()) < gameRules.getMaxDistance()){
                if(firebaseHandler.firebaseHandlerInstance_.cooldownVisitingPlaceByNameFinished(PlaceHandler.chosenPlace_.getName())){
                    string initialRange = gameRules.calculateRange(firebaseHandler.firebaseHandlerInstance_.currentUser_.getScore());
                    firebaseHandler.firebaseHandlerInstance_.userVisitedPlaceByName(PlaceHandler.chosenPlace_.getName());

                    toastMessageInstance.makeAnimation("You register this place as visited successfully!", new Color32(76,175,80,255), 2);
                    toastMessageInstance.makeAnimation(
                              "You have earned " + firebaseHandler.firebaseHandlerInstance_.currentUser_.lastVisitScore().ToString() + " points!", 
                              new Color32(76,175,80,255), 3);
                    
                    int challengeScore = firebaseHandler.firebaseHandlerInstance_.currentUser_.lastChallengeScore();
                    if(challengeScore != 0){
                      toastMessageInstance.makeAnimation(
                            "You also have completed a challenge! You have earned " + challengeScore.ToString() + " extra points!", 
                            new Color32(76,175,80,255), 3);
                    }
                    string postRange = gameRules.calculateRange(firebaseHandler.firebaseHandlerInstance_.currentUser_.getScore());
                    if(initialRange != postRange){
                      toastMessageInstance.makeAnimation( "You have reached the " + postRange + " range!", new Color32(76,175,80,255), 3);
                    }
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
