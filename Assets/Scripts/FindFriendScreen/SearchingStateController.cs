using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchingStateController : MonoBehaviour
{
    [SerializeField] private string searchingText_ = "Searching...";
    [SerializeField] private string waitingText_ = "Waiting until you end the edition...";
    [SerializeField] private string noResultText_ = "There isn't results for:";
    [SerializeField] private GameObject text_;
    [SerializeField] private GameObject searchingGif_;
    [SerializeField] private GameObject waitingBackground_;
    [SerializeField] private GameObject noResultBackground_;
    [SerializeField] private GameObject searchBar_;
    private SearchBar searchBarInstance_;

    void Awake(){
        searchBarInstance_ = searchBar_.GetComponent<SearchBar>();
    } 

    // Update is called once per frame
    void Update(){
        
        string state = searchBarInstance_.getState();
        searchingGif_.SetActive(state == "searching");
        waitingBackground_.SetActive(state == "waiting" || state == "searching");
        noResultBackground_.SetActive(state == "notFound");
        text_.SetActive(state == "searching" || state == "notFound" || state == "waiting");
        if(state == "searching"){
            text_.GetComponent<Text>().text = searchingText_;
        }else if(state == "notFound"){
            text_.GetComponent<Text>().text = noResultText_;
        }else{
            text_.GetComponent<Text>().text = waitingText_;
        }
        
    }
}
