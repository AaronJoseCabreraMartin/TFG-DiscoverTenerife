using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This class controls the prefab that shows the information of one
  * of the current user's challenge.
  */
public class challengePrefabController : MonoBehaviour
{
    /**
      * @brief GameObject that shows the name of the challenge's place.
      */
    [SerializeField] private GameObject placeName_;

    /**
      * @brief GameObject that shows the name of the challenge's place.
      */
    [SerializeField] private GameObject placeZone_;

    /**
      * @brief GameObject that shows the challenge's expire date.
      */
    [SerializeField] private GameObject expireDate_;

    /**
      * @brief GameObject that shows the player that sended this challenge.
      */
    [SerializeField] private GameObject challenger_;

    /**
      * @brief GameObject that contains the toastMessage that will shown
      * the error messages to the user.
      */
    [SerializeField] private GameObject toastMessage_;

    /**
      * @brief string that contains the default text of the text that will
      * shown the place's name.
      */
    [SerializeField] private string placeNameDefaultText_ = "";

    /**
      * @brief string that contains the default text of the text that will
      * shown the place's zone.
      */
    [SerializeField] private string placeZoneDefaultText_ = "Zone:";

    /**
      * @brief string that contains the default text of the text that will
      * shown the challenge's expire date.
      */
    [SerializeField] private string expireDateDefaultText_ = "Expire Date:\n";

    /**
      * @brief string that contains the default text of the text that will
      * shown the name of the user that sended this challenge.
      */
    [SerializeField] private string challengerDefaultText_ = "Challenger:";

    /**
      * @brief true if the user was warned before deleting the challenge.
      */
    private bool userWasWarned_;

    /**
      * @brief true if the prefab is showing the challenge's information.
      */
    private bool filled_;

    /**
      * @brief challengeData object that contains all the information of the
      * challenge that will be shown.
      */
    private challengeData challenge_;
    
    /**
      * @brief GameObject that references the panel that this 
      * GameObject is attached.
      */
    private GameObject panel_;

    /**
      * @brief This method is called before the first frame. It initialices the:
      * userWasWarned_, filled_ and challenge_ properties, and it sets the placeName_,
      * placeZone_, expireDate_ and challenger_ text as the correspondent default ones.
      */
    void Awake(){
        userWasWarned_ = false;
        filled_ = false;
        challenge_ = null;
        placeName_.GetComponent<Text>().text = placeNameDefaultText_;
        placeZone_.GetComponent<Text>().text = placeZoneDefaultText_;
        expireDate_.GetComponent<Text>().text = expireDateDefaultText_;
        challenger_.GetComponent<Text>().text = challengerDefaultText_;
    }

    /**
      * @brief This method is called each frame. It only checks if the filled_ property
      * is false, it that is the case it calls the fillFields method. In other case this
      * method dont do nothing.
      */
    void Update(){
        if(!filled_){
            fillFields();
        }
    }

    /**
      * @param challengeData object that contains all the information of the represented
      * challenge.
      * @brief This method is the setter of the challenge_ property. It also puts
      * the filled_ property as false and call the fillFields method.
      */
    public void setChallengeData(challengeData challenge){
        challenge_ = challenge;
        filled_ = false;
        fillFields();
    }

    /**
      * @brief This method checks if the friendDataIsComplete method of UserData
      * is true and if the challenge_ is not null, it sets all the texts of the prefab and
      * also sets the filled_ property as true. If both contitions are not true, this method
      * dont do nothing.
      */
    private void fillFields(){
        if(firebaseHandler.firebaseHandlerInstance_.currentUser_.friendDataIsComplete() && challenge_ != null){//si no estas esperando a descargar nada, rellena los datos
            FriendData challengerData = firebaseHandler.firebaseHandlerInstance_.currentUser_.getFriendDataByUID(challenge_.getChallengerId());
            challenger_.GetComponent<Text>().text = challengerDefaultText_ + challengerData.getDisplayName();
            string expireDate = (new DateTime(challenge_.getStartTimestamp() + gameRules.getExpiryTimeForChallenges())).ToString();
            expireDate_.GetComponent<Text>().text = expireDateDefaultText_ + expireDate;
            Place placeInfo = firebaseHandler.firebaseHandlerInstance_.requestHandler_.getPlaceByTypeAndId(challenge_.getPlaceType(),challenge_.getPlaceId().ToString());
            placeName_.GetComponent<Text>().text = placeNameDefaultText_ + placeInfo.getName();
            placeZone_.GetComponent<Text>().text = placeZoneDefaultText_ + placeInfo.getZone();
            filled_ = true;
        }
    }

    /**
      * @param GameObject that contains the panel that this object is attached.
      * @brief Setter of the panel_ property.
      */
    public void setPanel(GameObject panel){
        panel_ = panel;
    }

    /**
      * @brief This method delete this challenge and disattached it from the panel that is
      * attached. The first time its called it makes a toastMessage warning the user that
      * it will remove this challenge, if the user confirms the removing touching again
      * on the button, this method destroy this game object and calls challengeDeleted method
      * of challengesPanelController, destroyChallengeByChallengerId of UserData and 
      * writeUserData from firebaseHandler.
      */
    public void deleteThisChallenge(){
        if(userWasWarned_){
            panel_.GetComponent<challengesPanelController>().challengeDeleted(this.gameObject);
            firebaseHandler.firebaseHandlerInstance_.currentUser_.destroyChallengeByChallengerId(challenge_.getChallengerId());
            firebaseHandler.firebaseHandlerInstance_.writeUserData();
            Destroy(this);
            Destroy(this.gameObject);
        }else{
            userWasWarned_ = true;
            toastMessage_.GetComponent<toastMessage>().makeAnimation("Are you sure to delete this challenge?", new Color32(255,145,15,255), 5);
        }
    }
}
