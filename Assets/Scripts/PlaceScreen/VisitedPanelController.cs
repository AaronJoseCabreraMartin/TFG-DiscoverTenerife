using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisitedPanelController : MonoBehaviour
{
    [SerializeField] private GameObject image_;
    [SerializeField] private GameObject text_;

    [SerializeField] private string notVisitedText_;
    [SerializeField] private string visitedText_;

    private Color32 notVisitedColor_;
    private Color32 visitedColor_;
    
    private bool loaded_ = false;
    firebaseHandler firebaseHandlerObject_;

    void Awake(){
        firebaseHandlerObject_ = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        notVisitedColor_ = new Color32(255,98,38,255);
        visitedColor_ = new Color32(90,255,83,255);
        if(firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            CheckNewState();
        }
    }

    void Update(){
        if(!loaded_ && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            CheckNewState();
        }
    }


    public void CheckNewState(){
        bool playerHasVisitThisPlace = firebaseHandlerObject_.hasUserVisitPlaceByName(PlaceHandler.choosenPlace_.getName());
        image_.GetComponent<Image>().color = (playerHasVisitThisPlace ? visitedColor_ : notVisitedColor_);
        text_.GetComponent<Text>().text = (playerHasVisitThisPlace ? visitedText_ : notVisitedText_);
        loaded_ = true;
    }
}
