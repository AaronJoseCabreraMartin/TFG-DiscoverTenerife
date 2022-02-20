using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controls the panel that shows the information of a searched player.
  */
public class SearchedPlayer : MonoBehaviour
{
    /**
      * @brief GameObject that contains the text that shows the display name of the user.
      */
    [SerializeField] private GameObject text_;

    /**
      * @brief Reference to the panel that this object is being showed.
      */
    private SearchedFriendsPanel panel_;

    /**
      * @brief String conversion of the data of the searched player.
      */
    private Dictionary<string,string> searchedPlayerData_;
    
    /**
      * @brief true if the process of sending an invitation is currently working.
      * False in other case.
      */
    private bool onProcess_;

    /**
      * @brief This method is called before the first frame. It sets the onProcess_ property to false.
      */
    void Awake(){
        onProcess_ = false;
    }

    /**
      * @param Dictionary<string,string> string conversion of the data of the searched player.
      * @brief This method sets the searchedPlayerData_ property to the value of the given dictionary.
      * It expects a dictionary with the entries: name, uid, rank (on future).
      * It also sets the text component of the text_ game object as the name entry of the given dictionary.
      */
    public void setSearchedPlayerData(Dictionary<string,string> searchedPlayerData){
        searchedPlayerData_ = searchedPlayerData;
        text_.GetComponent<Text>().text = searchedPlayerData_["name"];
    }

    /**
      * @brief if the onProcess_ property is true, this method dont do nothing. This is a way to control
      * that there isnt several parallel calls to the send invitation process. If onProcess_ property is false
      * but there isnt internet connection, it make a toast animation showing an error message because you
      * have to have internet connection to complete the send invitation process. And if you have internet
      * connection and the onProcess_ property is false, it sets the onProcess_ property to true and then 
      * call the sendFriendshipInvitation method of firebaseHandler class.
      */
    public void sendInvitation(){
        if(onProcess_){
            return;
        }
        if(searchedPlayerData_["uid"] == firebaseHandler.firebaseHandlerInstance_.currentUser_.getUid()){
            panel_.makeToast("You can't send a friendship invitation to yourself.",new Color32(255,0,0,255), 5);
        }else if(firebaseHandler.firebaseHandlerInstance_.internetConnection()){
            onProcess_ = true;
            //avisar a firebase para que envie la peticion de mistad
            firebaseHandler.firebaseHandlerInstance_.sendFriendshipInvitation(searchedPlayerData_["uid"],this);
        }else{
            //si no hay internet toast Error
            panel_.makeToast("You don't have access to internet, try it again later",new Color32(255,0,0,255), 5);
        }
    }

    /**
      * @param string with the result, the string should be one of the defined states.
      * @brief This method should be called by the firebaseHandler instance when it 
      * finish the sendFriendshipInvitation method. This method shows a toast
      * showing to the user the result of the sending invitation process, if it 
      * makes it right it also calls the clearSearchedFriendsPanel method of the 
      * SearchedFriendsPanel class.
      * The defined states for the result are failed, repeated and sended. If the
      * given string has a different state, it will show an error on the console.
      */
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

    /**
      * @param reference to a SearchedFriendsPanel to copy it on the panel_
      * property.
      * @brief setter of the panel_ property.
      */
    public void SetPanel(SearchedFriendsPanel panel){
        panel_ = panel;
    }
}
