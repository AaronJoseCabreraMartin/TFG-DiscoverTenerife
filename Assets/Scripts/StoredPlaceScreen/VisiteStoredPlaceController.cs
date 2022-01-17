using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisiteStoredPlaceController : MonoBehaviour
{
    [SerializeField] private GameObject image_;
    [SerializeField] private GameObject text_;

    [SerializeField] private string notVisitedText_;
    [SerializeField] private string visitedText_;

    private Color32 notVisitedColor_;
    private Color32 visitedColor_;
    

    void Awake(){
        notVisitedColor_ = new Color32(255,98,38,255);
        visitedColor_ = new Color32(90,255,83,255);
        CheckNewState();
    }

    void Update(){
        CheckNewState();
    }


    public void CheckNewState(){
        bool playerHasVisitThisPlace = StoredPlacesController.choosenStoredPlace_.visited();
        image_.GetComponent<Image>().color = (playerHasVisitThisPlace ? visitedColor_ : notVisitedColor_);
        text_.GetComponent<Text>().text = (playerHasVisitThisPlace ? visitedText_ : notVisitedText_);
    }
}
