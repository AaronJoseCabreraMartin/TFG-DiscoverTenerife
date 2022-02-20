using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Controls the panel that shows the state of the search in the find users screen.
  */
public class SearchingStateController : MonoBehaviour
{
    /**
      * @brief string that contains the text that will be showed when the searching is on process
      */
    [SerializeField] private string searchingText_ = "Searching...";

    /**
      * @brief string that contains the text that will be showed when the user is writing on the 
      * input box of the search bar.
      */
    [SerializeField] private string waitingText_ = "Waiting until you end the edition...";
    
    /**
      * @brief string that contains the text that will be showed when the search didnt find any results.
      */
    [SerializeField] private string noResultText_ = "There isn't results for:";
    
    /**
      * @brief A reference to the GameObject that contains the text that will be showed.
      */
    [SerializeField] private GameObject text_;

    /** 
      * @brief A reference to the GameObject of the bouncing image that is show while the 
      * searching is on process.
      */
    [SerializeField] private GameObject searchingGif_;

    /** 
      * @brief A reference to the GameObject that contains the image that will be showed
      * as a background during the searching.
      */
    [SerializeField] private GameObject waitingBackground_;
    
    /** 
      * @brief A reference to the GameObject that contains the image that will be showed
      * if the search didnt found any results.
      */
    [SerializeField] private GameObject noResultBackground_;
    
    /** 
      * @brief A reference to the GameObject of the search bar.
      */
    [SerializeField] private GameObject searchBar_;

    /** 
      * @brief A reference to the SearchBar class to make the code more clean.
      */
    private SearchBar searchBarInstance_;


    /** 
      * @brief This method is called before the first frame, it only instanciate 
      * the searchBarInstance_ property.
      */
    void Awake(){
        searchBarInstance_ = searchBar_.GetComponent<SearchBar>();
    } 

    /** 
      * @brief This method is called every frame, it checks the state of the search
      * and activate or desactivate the correspondent backgrounds and images. It also changes
      * the showed text.
      */
    void Update(){
        string state = searchBarInstance_.getState();
        searchingGif_.SetActive(state == "searching");
        waitingBackground_.SetActive(state == "waiting" || state == "searching");
        noResultBackground_.SetActive(state == "notFound");
        text_.SetActive(state == "searching" || state == "notFound" || state == "waiting");
        if(state == "searching"){
            text_.GetComponent<Text>().text = searchingText_;
        }else if(state == "notFound"){
            text_.GetComponent<Text>().text = noResultText_ + "\n" + searchBarInstance_.getWordsToSearch();
        }else{
            text_.GetComponent<Text>().text = waitingText_;
        }
        
    }
}
