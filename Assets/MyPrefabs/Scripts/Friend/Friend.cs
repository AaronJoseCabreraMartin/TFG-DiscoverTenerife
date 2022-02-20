using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief class that controls the panel element that represent one 
  * of the current user friends.
  */
public class Friend : MonoBehaviour
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
      * @brief Reference to a FriendData object that contains all the information
      * of the shown friend.
      */
    private FriendData friendData_;

    /**
      * @brief True if the user was notified about he is erasing a friend, false
      * if you have to notify the user before deleting this friend.
      */
    private bool userWasNotified_;

    /**
      * @param the FriendData object that contains all the information
      * of the represented user.
      * @brief The constructor of the class. It sets the userWasNotified_ as false,
      * it sets the shown text as the display name of the user.
      */
    public void setData(FriendData friendData){
        userWasNotified_ = false;
        friendData_ = friendData;
        displayName_.GetComponent<Text>().text = friendData_.getDisplayName();
    }
    
    /**
      * @param GameObject that contains the panel that this object is attached.
      * @brief Setter of the panel_ property.
      */
    public void setPanel(GameObject panel){
        panel_ = panel;
    }

    /**
      * @brief TODO
      */
    public void chanllegeFriend(){
        if(firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            //mostrar buscador de lugares
            //una vez seleccionado:
            // toast amigo retado!
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You have chanllege your friend successfully", new Color32(76,175,80,255), 5);
            // desactivar interactibilidad del boton hasta que el reto caduque (1 semana)
        }else{
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You don't have internet connection, try it again later.", new Color32(255,0,0,255), 5);
        }
    }

    /**
      * @brief this method deletes the friendship relationship if there is
      * internet connection. Before deleting the friendship relationship
      * it notify the user to make him aware of what he is doing.
      */
    public void deleteFriend(){
        if(firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            if(userWasNotified_){
                //eliminar amigo
                // avisar a user data para que actualice sus listas
                firebaseHandler.firebaseHandlerInstance_.currentUser_.deleteFriend(friendData_.getUid());
                friendData_.addDeletedFriend(firebaseHandler.firebaseHandlerInstance_.auth.CurrentUser.UserId);
                // avisar a firebase para que suba los cambios
                firebaseHandler.firebaseHandlerInstance_.updateUserDeleteAFriend(friendData_.getUid(),friendData_.getStringConversionOfDeletedFriends());
                firebaseHandler.firebaseHandlerInstance_.writeUserData();
                //toast de amigo eliminado!
                toastMessageObject_.GetComponent<toastMessage>().makeAnimation(friendData_.getDisplayName() + " is no longer a friend", new Color32(255,0,0,255), 5);
                //destruye este objeto y actualiza el tamaño del panel
                destroyAndAdvice();
            }else{
                toastMessageObject_.GetComponent<toastMessage>().makeAnimation("Are you sure that you want to delete "+friendData_.getDisplayName() + " as a friend?", new Color32(255,145,15,255), 5);
                userWasNotified_ = true;
            }
        }else{
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You don't have internet connection, try it again later.", new Color32(255,0,0,255), 5);
        }
    }

    /**
      * @brief This method destroy this GameObject and notify the
      * friendsPanel object at it is attached to make it adjust
      * his height.
      */
    private void destroyAndAdvice(){
        panel_.GetComponent<friendsPanel>().friendDeleted(this.gameObject);
        Destroy(this.gameObject);
        Destroy(this);
    }
    
    /**
      * @return string that contains the display name of the user.
      * @brief getter of the display name of the represented user.
      */
    public string getName(){
        return friendData_.getDisplayName();
    }

    /**
      * @return string that contains the user id of the user.
      * @brief getter of the user id of the represented user.
      */
    public string getUid(){
      return friendData_.getUid();
    }
}
