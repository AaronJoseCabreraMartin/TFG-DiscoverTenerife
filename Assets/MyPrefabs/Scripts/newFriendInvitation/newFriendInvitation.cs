using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class newFriendInvitation : MonoBehaviour
{
    [SerializeField] private GameObject displayName_;
    
    private GameObject panel_;
    private string name_;

    public void setDisplayName(string newText){
        name_ = newText;
        displayName_.GetComponent<Text>().text = newText;
    }

    public void setPanel(GameObject panel){
        panel_ = panel;
    }

    public void acceptFriend(){
        //aceptar amigo:
        // avisar a user data para que actualice sus listas
        // avisar a firebase para que suba los cambios 
        //toast de amigo aceptado!
        destroyAndAdvice();
    }

    public void refuseFriend(){
        //rechazar amigo:
        // avisar a user data para que actualice sus listas
        // avisar a firebase para que suba los cambios 
        //toast de amigo rechazado!
        destroyAndAdvice();
    }

    private void destroyAndAdvice(){
        panel_.GetComponent<newFriendInvitationPanel>().invitationDeleted(this.gameObject);
        Destroy(this.gameObject);
        Destroy(this);
    }

    public string getName(){
        return name_;
    }
}
