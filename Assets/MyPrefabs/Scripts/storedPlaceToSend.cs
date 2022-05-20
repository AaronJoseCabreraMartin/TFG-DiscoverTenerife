using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controls the prefab that represent a stored place
  * on the choosing place for make a challenge screen.
  */
public class storedPlaceToSend : MonoBehaviour
{
    /**
      * @brief GameObject that contains the text where the name of the place will be shown.
      */
    [SerializeField] private GameObject name_;
    
    /**
      * @brief GameObject that contains the text where the zone of the place will be shown.
      */
    [SerializeField] private GameObject zone_;

    /**
      * @brief GameObject that contains the text where the type of the place will be shown.
      */
    [SerializeField] private GameObject type_;
    
    /**
      * @brief GameObject that contains the toastMessage where the errors and the warings
      * to the user will be shown.
      */
    [SerializeField] private GameObject toastMessageObject_;

    /**
      * @brief GameObject that references the panel that this 
      * GameObject is attached.
      */
    private GameObject panel_;

    /**
      * @brief A reference to the stored place that is represented with this prefab.
      */
    private StoredPlace representedPlace_;

    /**
      * @brief boolean that controls if the user can select a place to send a challenge
      * to not allow the user send several challenges to the same user meanwhile the 
      * destruction animation is being played.
      */
    public static bool userCanSelect_ = true;
    
    /**
      * @param GameObject that contains the panel that this object is attached.
      * @brief Setter of the panel_ property.
      */
    public void setPanel(GameObject panel){
        panel_ = panel;
    }

    /**
      * @param StoredPlace with the information of the place that is represented.
      * @brief This method sets the representedPlace_ property as the given
      * StoredPlace it also sets the text of the name_, zone_ and type_ gameObjects.
      */
    public void setData(StoredPlace placeInformation){
        representedPlace_ = placeInformation;
        name_.GetComponent<Text>().text = placeInformation.getName();
        zone_.GetComponent<Text>().text = placeInformation.getZone();
        type_.GetComponent<Text>().text = firebaseHandler.firebaseHandlerInstance_.findPlaceByName(placeInformation.getName())["type"];
    }

    /**
      * @brief This method checks if there is internet connection, if there inst, it shows an error
      * on the toastMessageObject_ gameobject. If there is, it creates a new challenge with 
      * the current user as challenger, the represented stored place as place to visit on the challenge
      * and it also calls the uploadFriendChallengesOf method of firebaseHandler class. Then, it shows 
      * a toastMessage and then start the destroyAfterSeconds coroutine.
      */
    public void chooseThisPlaceAndUpload(){
        if(storedPlaceToSend.userCanSelect_){
          if(firebaseHandler.firebaseHandlerInstance_.internetConnection()){
              storedPlaceToSend.userCanSelect_ = false;
              Dictionary<string,string> placeKeys = firebaseHandler.firebaseHandlerInstance_.findPlaceByName(representedPlace_.getName());
              FriendData.chosenFriend_.createNewChallenge(placeKeys["id"], placeKeys["type"],firebaseHandler.firebaseHandlerInstance_.currentUser_.getUid());
              firebaseHandler.firebaseHandlerInstance_.uploadFriendChallengesOf(FriendData.chosenFriend_);
              toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You have chanllege " + FriendData.chosenFriend_.getDisplayName() + " successfully", new Color32(76,175,80,255), 5);
              StartCoroutine(destroyAfterSeconds(5));
          }else{
              toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You don't have internet connection, try it again later.", new Color32(255,0,0,255), 5);
          }
        }
    }

    /**
      * @param int number of seconds that it has to wait.
      * @brief This method wait for the given number of seconds and then
      * it calls the destroyAndAdvice method.
      */
    IEnumerator destroyAfterSeconds(int seconds){
        yield return new WaitForSeconds(seconds);
        destroyAndAdvice();
    }

    /**
      * @brief This method destroy this GameObject and notify the
      * friendsPanel object at it is attached to make it adjust
      * his height.
      */
    private void destroyAndAdvice(){
        if(panel_!= null){
          panel_.GetComponent<chooseAPlaceToSendAsAChallengePanel>().elementDeleted(this.gameObject);
        }
        Destroy(this.gameObject);
        Destroy(this);
        storedPlaceToSend.userCanSelect_ = true;
    }
}
