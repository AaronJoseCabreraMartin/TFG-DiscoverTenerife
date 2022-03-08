using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that the panel element that shows a new friendship invitation.
  */
public class newFriendInvitation : MonoBehaviour
{
    /**
      * @brief GameObject that references the toastMessage object
      * that will shown the information to the user.
      */
    [SerializeField] private GameObject toastMessageObject_;

    /**
      * @brief GameObject that has the text where the displayName
      * will be shown.
      */
    [SerializeField] private GameObject displayName_;
    
    /**
      * @brief GameObject that references the panel that this 
      * GameObject is attached.
      */
    private GameObject panel_;

    /**
      * @brief Reference to a newFriendData object that contains all the information
      * of the shown friendship invitation.
      */
    private newFriendData newFriendData_;

    /**
      * @param the newFriendData object that contains all the information
      * of the represented frienship invitation.
      * @brief The constructor of the class. It stores the given newFriendData
      * object, it sets the shown text as the display name of the user.
      */
    public void setData(newFriendData friendData){
        newFriendData_ = friendData;
        displayName_.GetComponent<Text>().text = newFriendData_.getDisplayName();
    }

    /**
      * @param GameObject that contains the panel that this object is attached.
      * @brief Setter of the panel_ property.
      */
    public void setPanel(GameObject panel){
        panel_ = panel;
    }

    /**
      * @brief This method should be called when the current user accepts
      * the friendship invitation of the current user. If there is internet connection
      * it calls the acceptFriend method of UserData class and it calls both
      * downloadFriendData and writeUserData firebaseHandler class. If there is
      * no internet connection it just shows an error message on a toastMessage.
      */
    public void acceptFriend(){
        if(firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            //aceptar amigo:
            // avisar a user data para que actualice sus listas
            firebaseHandler.firebaseHandlerInstance_.currentUser_.acceptFriend(newFriendData_.getUid());
            // avisar a firebase para que suba los cambios
            firebaseHandler.firebaseHandlerInstance_.downloadFriendData(newFriendData_.getUid());
            firebaseHandler.firebaseHandlerInstance_.writeUserData();
            //toast de amigo aceptado!
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You have accepted "+newFriendData_.getDisplayName()+" as your friend successfully", new Color32(76,175,80,255), 5);
            destroyAndAdvice();
        }else{
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You dont have internet connection, try it again later.", new Color32(255,0,0,255), 5);
        }
    }

    /**
      * @brief This method should be called when the current user denies
      * the friendship invitation of the current user. If there is internet connection
      * it calls the deleteInvitationByName method of UserData class and it calls
      * writeUserData of firebaseHandler class. If there is no internet connection it 
      * just shows an error message on a toastMessage.
      */
    public void refuseFriend(){
        if(firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            //rechazar amigo:
            // avisar a user data para que actualice sus listas
            firebaseHandler.firebaseHandlerInstance_.currentUser_.deleteInvitationByName(newFriendData_.getUid());
            // avisar a firebase para que suba los cambios 
            firebaseHandler.firebaseHandlerInstance_.writeUserData();
            //toast de amigo rechazado!
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You have rejected the "+newFriendData_.getDisplayName()+"'s invitation to be your friend", new Color32(255,145,15,255), 5);
            destroyAndAdvice();
        }else{
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You dont have internet connection, try it again later.", new Color32(255,0,0,255), 5);
        }
    }

    /**
      * @brief This method destroy this GameObject and it also calls the
      * elementDeleted method of the newFriendInvitationPanel class.
      */
    private void destroyAndAdvice(){
      panel_.GetComponent<newFriendInvitationPanel>().elementDeleted(this.gameObject);
      Destroy(this.gameObject);
      Destroy(this);
    }

    /**
      * @param string that contains the user ID of the represented user.
      * @brief Getter of the user id property.
      */
    public string getUid(){
        return newFriendData_.getUid();
    }
}
