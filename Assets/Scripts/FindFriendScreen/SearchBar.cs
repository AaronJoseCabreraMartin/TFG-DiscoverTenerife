using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This class controls the search bar of the screen that allow the user find other users
  * to send them a new friendship invitation.
  */
public class SearchBar : MonoBehaviour
{
    /**
      * @brief This should be a reference to the text of the input panel.
      */   
    [SerializeField] private GameObject text_;
    
    /**
      * @brief This should be a reference to the panel that shows the results of the search.
      */
    [SerializeField] private GameObject panel_;

    /**
      * @brief This property controls the state of the search. It could be:
      * - waiting: That means that the search not even started.
      * - searching: That means that it is searching right now.
      * - notFound: That means that the search didnt found any results.
      * - found: That means that the search found some results.
      */
    private string state_;

    /**
      * @brief This method is called before the first frame, it sets the state_ property to "waiting" value.
      */
    void Awake(){
        state_ = "waiting";
    }

    /**
      * @brief This method is called when the user is writing on the search bar input box, 
      * it sets the state_ property to "waiting" value.
      */
    public void OnWriting(string whatIsWriting){
        state_ = "waiting";
    }

    /**
      * @brief This method is called when the user finish to write on the search bar input box, 
      * if there is no internet connection or the searched user is the current user it set the 
      * state_ property to notFound. If there is internet connection and the user wrote something
      * on the search bar it calls the SearchOtherUserByName method of firebaseHandler.
      */
    public void OnEndWriting(string searchedName){
        
        Debug.Log("WTF DEBERIA COMPROBAR QUE NO ESTÁ EN TU LISTA DE AMIGOS YA!!!");
        
        //si no hay internet o es tu propio uid ni lo intenta buscar
        if( !firebaseHandler.firebaseHandlerInstance_.internetConnection() || 
            //WTF no deberia ser el firebaseHandler.firebaseHandlerInstance_.auth.CurrentUser.DisplayName!?!?
            text_.GetComponent<Text>().text == firebaseHandler.firebaseHandlerInstance_.auth.CurrentUser.UserId){
                state_ = "notFound";
        }else if(firebaseHandler.firebaseHandlerInstance_.internetConnection() && text_.GetComponent<Text>().text.Length != 0){
            Debug.Log("Searching "+text_.GetComponent<Text>().text);
            state_ = "searching";
            firebaseHandler.firebaseHandlerInstance_.SearchOtherUserByName(text_.GetComponent<Text>().text,"usersThatAllowFriendshipInvitations",this);  
        }
    }

    /**
      * @param Dictionary<string,string> dictionary that contains the result of the search 
      * @brief this method checks the result of the search and set the correct state
      * if the result finished successfully, it calls the addSearchedFriendToPanel method 
      * of the SearchedFriendsPanel class. 
      */
    public void resultsOfTheSearch(Dictionary<string,string> result){
        // el resultado de la busqueda tendra 
        //    uid  -> uid del usuario
        //    name -> displayName del usuario
        if(result.ContainsKey("uid") && result.ContainsKey("name")){
            state_ = "found";
            //avisar al panel para que cree el prefab
            Debug.Log($"ENCONTRADO: uid " + result["uid"] + " name "+ result["name"] );
            panel_.GetComponent<SearchedFriendsPanel>().addSearchedFriendToPanel(result);
        }else{
            state_ = "notFound";
        }
    }

    /**
      * @brief getter of the current state of the search.
      */
    public string getState(){
        return state_;
    }
}
