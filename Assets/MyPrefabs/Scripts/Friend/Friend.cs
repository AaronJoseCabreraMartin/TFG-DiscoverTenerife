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

    public void setData(FriendData friendData){
        friendData_ = friendData;
        displayName_.GetComponent<Text>().text = friendData_.getDisplayName();
    }

    public void setPanel(GameObject panel){
        panel_ = panel;
    }

    public void chanllegeFriend(){
        //mostrar buscador de lugares
        //una vez seleccionado:
        // toast amigo retado!
        toastMessageObject_.GetComponent<toastMessage>().makeAnimation("You have chanllege your friend successfully", new Color32(76,175,80,255), 5);
        // desactivar interactibilidad del boton hasta que el reto caduque (1 semana)
    }

    public void deleteFriend(){
        //rechazar amigo:
        // avisar a user data para que actualice sus listas
        // avisar a firebase para que suba los cambios 
        //toast de amigo eliminado!
        toastMessageObject_.GetComponent<toastMessage>().makeAnimation(friendData_.getDisplayName() + " is no longer a friend", new Color32(255,0,0,255), 5);
        destroyAndAdvice();
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
