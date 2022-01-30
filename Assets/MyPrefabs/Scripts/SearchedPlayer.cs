using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchedPlayer : MonoBehaviour
{
    [SerializeField] private GameObject text_;

    private SearchedFriendsPanel panel_;
    private Dictionary<string,string> searchedPlayerData_;
    private bool onProcess_;

    void Awake(){
        onProcess_ = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setSearchedPlayerData(Dictionary<string,string> searchedPlayerData){
        searchedPlayerData_ = searchedPlayerData;
        text_.GetComponent<Text>().text = searchedPlayerData_["name"];
    }

    public void sendInvitation(){
        if(onProcess_){
            return;
        }
        if(firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            onProcess_ = true;
            //avisar a firebase para que envie la peticion de mistad
            firebaseHandler.firebaseHandlerInstance_.sendFriendshipInvitation(searchedPlayerData_["uid"],this);
        }else{
            //si no hay internet toast Error
            panel_.makeToast("You don't have access to internet, try it again later",new Color32(255,0,0,255), 5);
        }
    }

    public void resultOfTheSending(string result){
        Debug.Log($"On resultOfTheSending with = {result}");
        onProcess_ = false;
        if(result == "failed"){
            //error, no se pudo enviar la peticion
            panel_.makeToast("Error, the petition couldn't be sended, please try it again",new Color32(255,0,0,255), 5);
        }else if(result == "repeated"){
            //error, {usuario} ya tiene una peticion de amistad tuya pendiente 
            panel_.makeToast("Error, the user "+searchedPlayerData_["name"]+" already have a friendship invitation of you",new Color32(255,145,15,255), 5);
        }else if(result == "sended"){
            //toast diciendo OK!
            panel_.makeToast("You have sended a friendship invitation to"+searchedPlayerData_["name"]+" successfully", new Color32(76,175,80,255), 5);
            //avisar al panel para que se limpie
            //panel_.clearSearchedFriendsPanel();
        }else{
            Debug.Log($"resultado de resultOfTheSending inesperado: {result}");
        }
    }

    public void SetPanel(SearchedFriendsPanel panel){
        panel_ = panel;
    }
}
