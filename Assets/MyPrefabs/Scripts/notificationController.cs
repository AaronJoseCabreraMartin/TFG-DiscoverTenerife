using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the notification light that powers
  * ON when the current user has a new friendship invitation or a
  * chanllege.
  */
public class notificationController : MonoBehaviour
{
    /**
      * @brief GameObject that stores the image of the yellow circle that alerts
      * the user.
      */
    [SerializeField] GameObject image_;

    /**
      * @brief string that determinates which property of the current user
      * this class should watch. The allowed values are:
      * - friendshipsInvitations: It will check if the user has any new friendship invitation,
      * if that is the case, it will shown the image_ GameObject, in other case it will hide it.
      * - chanlleges: It will check if the user has any new chanllege to do, if that is the case,
      * it will show the image_ GameObject, in other case it will hide it.
      * - all: It will check each of the options already mentioned.
      */
    [SerializeField] string toObserve_;
    
    /**
      * @brief This method is called on the first frame, it hides or shows the image
      * depending of the result of calling shouldBeShown method.
      */
    void Start(){
        image_.SetActive(shouldBeShown());
    }

    /**
      * @brief This method is called on each frame, it hides or shows the image
      * depending of the result of calling shouldBeShown method.
      */
    void Update(){
        image_.SetActive(shouldBeShown());
    }

    /**
      * @return true if the image_ GameObject should be shown, false in other case.
      * @brief This method checks the current user methods that the toObserve_ 
      * property points. If either the toObserve_ property has an unvalid string or
      * the user data isnt ready it will return false.
      */
    private bool shouldBeShown(){
        if(!firebaseHandler.firebaseHandlerInstance_.userDataIsReady()){
            return false;
        }
        // true si hay que mostrarlo
        if(toObserve_ == "friendshipsInvitations"){
            return firebaseHandler.firebaseHandlerInstance_.currentUser_.countOfNewFriends() != 0; 
        }

        if(toObserve_ == "chanlleges"){
            return firebaseHandler.firebaseHandlerInstance_.currentUser_.getQuantityOfChallenges() != 0; 
        }

        if(toObserve_ == "all"){
            return firebaseHandler.firebaseHandlerInstance_.currentUser_.countOfNewFriends() != 0
                    || firebaseHandler.firebaseHandlerInstance_.currentUser_.getQuantityOfChallenges() != 0;
        }
        return false;
    }
}
