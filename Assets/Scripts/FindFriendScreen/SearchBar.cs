using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchBar : MonoBehaviour
{

    [SerializeField] private GameObject text_;
    [SerializeField] private GameObject panel_;

    private string state_;

    void Awake(){
        state_ = "waiting";
    }

    public void OnWriting(string whatIsWriting){
        state_ = "waiting";
    }

    public void OnEndWriting(string searchedName){
        
        Debug.Log("DEBERIA COMPROBAR QUE NO ESTÁ EN TU LISTA DE AMIGOS YA!!!");
        
        //si no hay internet o es tu propio uid ni lo intenta buscar
        if( !firebaseHandler.firebaseHandlerInstance_.internetConnection() || 
            text_.GetComponent<Text>().text == firebaseHandler.firebaseHandlerInstance_.auth.CurrentUser.UserId){
                state_ = "notFound";
        }else if(firebaseHandler.firebaseHandlerInstance_.internetConnection() && text_.GetComponent<Text>().text.Length != 0){
            Debug.Log("Searching "+text_.GetComponent<Text>().text);
            state_ = "searching";
            firebaseHandler.firebaseHandlerInstance_.SearchOtherUserByName(text_.GetComponent<Text>().text,"usersThatAllowFriendshipInvitations",this);  
        }
    }

    // el resultado de la busqueda tendra 
    //    uid  -> uid del usuario
    //    name -> displayName del usuario
    public void resultsOfTheSearch(Dictionary<string,string> result){
        if(result.ContainsKey("uid") && result.ContainsKey("name")){
            state_ = "found";
            //avisar al panel para que cree el prefab
            Debug.Log($"ENCONTRADO: uid " + result["uid"] + " name "+ result["name"] );
            panel_.GetComponent<SearchedFriendsPanel>().addSearchedFriendToPanel(result);
        }else{
            state_ = "notFound";
        }
    }

    public string getState(){
        return state_;
    }
}
