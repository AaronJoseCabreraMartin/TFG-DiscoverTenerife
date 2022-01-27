using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Friend : MonoBehaviour
{
    [SerializeField] private GameObject toastMessageObject_;
    [SerializeField] private GameObject displayName_;
    
    private GameObject panel_;
    private FriendData friendData_;
    private bool usuarioAvisado_;

    public void setData(FriendData friendData){
        usuarioAvisado_ = false;
        friendData_ = friendData;
        displayName_.GetComponent<Text>().text = friendData_.getDisplayName();
    }

    public void setPanel(GameObject panel){
        panel_ = panel;
    }

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

    public void deleteFriend(){
        if(firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            if(usuarioAvisado_){
                //eliminar amigo
                // avisar a user data para que actualice sus listas
                firebaseHandler.firebaseHandlerInstance_.actualUser_.deleteFriend(friendData_.getUid());
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
                usuarioAvisado_ = true;
            }
        }else{
            toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You don't have internet connection, try it again later.", new Color32(255,0,0,255), 5);
        }
    }

    private void destroyAndAdvice(){
        panel_.GetComponent<friendsPanel>().friendDeleted(this.gameObject);
        Destroy(this.gameObject);
        Destroy(this);
    }
    

    public string getName(){
        return friendData_.getDisplayName();
    }
}
