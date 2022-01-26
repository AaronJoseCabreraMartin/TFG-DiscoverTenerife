using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*

Falta 
    - mensaje de que no tienes amigos
    - hacer que se pueda eliminar un amigo! y se suban los datos

*/
public class newFriendInvitation : MonoBehaviour
{
    [SerializeField] private GameObject toastMessageObject_;
    [SerializeField] private GameObject displayName_;
    
    private GameObject panel_;
    private newFriendData newFriendData_;

    public void setData(newFriendData friendData){
        newFriendData_ = friendData;
        displayName_.GetComponent<Text>().text = newFriendData_.getDisplayName();
    }

    public void setPanel(GameObject panel){
        panel_ = panel;
    }

    public void acceptFriend(){
        //aceptar amigo:
        // avisar a user data para que actualice sus listas
        firebaseHandler.firebaseHandlerInstance_.actualUser_.acceptFriend(newFriendData_.getUid());
        // avisar a firebase para que suba los cambios
        firebaseHandler.firebaseHandlerInstance_.addFriendDataToDownload(newFriendData_.getUid());
        firebaseHandler.firebaseHandlerInstance_.writeUserData();
        //toast de amigo aceptado!
        toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You have accepted "+newFriendData_.getDisplayName()+" as your friend successfully", new Color32(76,175,80,255), 5);
        destroyAndAdvice();
    }

    public void refuseFriend(){
        //rechazar amigo:
        // avisar a user data para que actualice sus listas
        firebaseHandler.firebaseHandlerInstance_.actualUser_.deleteInvitationByName(newFriendData_.getUid());
        // avisar a firebase para que suba los cambios 
        firebaseHandler.firebaseHandlerInstance_.writeUserData();
        //toast de amigo rechazado!
        toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You have rejected the "+newFriendData_.getDisplayName()+"'s invitation to be your friend", new Color32(255,145,15,255), 5);
        destroyAndAdvice();
    }

    private void destroyAndAdvice(){
        panel_.GetComponent<newFriendInvitationPanel>().invitationDeleted(this.gameObject);
        Destroy(this.gameObject);
        Destroy(this);
    }

    public string getUid(){
        return newFriendData_.getUid();
    }
}
