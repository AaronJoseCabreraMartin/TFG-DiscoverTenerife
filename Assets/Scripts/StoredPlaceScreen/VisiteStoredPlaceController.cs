using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
  * @brief Class that controls the panel that shows if the user has visited the 
  * chosen stored place.
  */
public class VisiteStoredPlaceController : MonoBehaviour
{
    /**
      * @brief GameObject that has the background image of the panel.
      */
    [SerializeField] private GameObject image_;

    /**
      * @brief GameObject that has the text of the panel.
      */
    [SerializeField] private GameObject text_;

    /**
      * @brief string that contains the text that will be shown when the user 
      * havent visited the current chosen stored place.
      */
    [SerializeField] private string notVisitedText_;

    /**
      * @brief string that contains the text that will be shown when the user 
      * have visited the current chosen stored place.
      */
    [SerializeField] private string visitedText_;

    /**
      * @brief Color of the panel when the user havent visited the current 
      * chosen stored place.
      */
    private Color32 notVisitedColor_;

    /**
      * @brief Color of the panel when the user have visited the current 
      * chosen stored place.
      */
    private Color32 visitedColor_;
    
    /**
      * @brief This method is called before the first frame. It initiaize both the
      * notVisitedColor_ and the visitedColor_ attributes and calls the CheckNewState
      * method.
      */
    void Awake(){
        notVisitedColor_ = new Color32(255,98,38,255);
        visitedColor_ = new Color32(90,255,83,255);
        CheckNewState();
    }

    /**
      * @brief This method is called once each frame. It calls the CheckNewState
      * method.
      */
    void Update(){
        CheckNewState();
    }

    /**
      * @brief This method check if the current stored chosen place was already visited
      * and then, shows both the correspondent background and text.
      */
    public void CheckNewState(){
        bool playerHasVisitThisPlace = StoredPlacesController.choosenStoredPlace_.visited();
        image_.GetComponent<Image>().color = (playerHasVisitThisPlace ? visitedColor_ : notVisitedColor_);
        text_.GetComponent<Text>().text = (playerHasVisitThisPlace ? visitedText_ : notVisitedText_);
    }
}
