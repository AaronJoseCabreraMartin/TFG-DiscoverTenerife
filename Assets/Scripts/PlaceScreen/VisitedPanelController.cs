using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that shows if the user has already visited the chosen place.
  */
public class VisitedPanelController : MonoBehaviour
{
    /**
      * @brief GameObject that contains the background image of the sign.
      */
    [SerializeField] private GameObject image_;

    /**
      * @brief GameObject that contains the text of the sign.
      */
    [SerializeField] private GameObject text_;

    /**
      * @brief String that contains the not visited message.
      */
    [SerializeField] private string notVisitedText_;

    /**
      * @brief String that contains the visited message.
      */
    [SerializeField] private string visitedText_;

    /**
      * @brief Color of the background of the sign when it isnt already visited.
      */
    private Color32 notVisitedColor_;

    /**
      * @brief Color of the background of the sign when it is already visited.
      */
    private Color32 visitedColor_;
    
    /**
      * @brief Bolean that controls if the information was already checked to 
      * show the correct one.
      */
    private bool loaded_ = false;

    /**
      * @brief Reference to the firebaseHandler instance.
      */
    firebaseHandler firebaseHandlerObject_;

    /**
      * @brief This method is called on the first frame, it intializes the 
      * firebaseHandlerObject_, notVisitedColor_ and visitedColor_ properties.
      * If both the placesAreReady and userDataIsReady methods of firebaseHandler return
      * true, it calls the CheckNewState method.
      */
    void Awake(){
        firebaseHandlerObject_ = firebaseHandler.firebaseHandlerInstance_;
        notVisitedColor_ = new Color32(255,98,38,255);
        visitedColor_ = new Color32(90,255,83,255);
        if(firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            CheckNewState();
        }
    }

    /**
      * @brief This method is called once per frame, it checks if the information hasnt been
      * loaded and both placesAreReady and userDataIsReady return true it calls the CheckNewState
      * method.
      */
    void Update(){
        if(!loaded_ && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            CheckNewState();
        }
    }

    /**
      * It checks if the user has already visited the chosen place, and change both the text
      * and the background color to show if the user has already visited it or not. It also
      * sets the loaded_ attribute as true. 
      */
    public void CheckNewState(){
        bool playerHasVisitThisPlace = firebaseHandlerObject_.hasUserVisitPlaceByName(PlaceHandler.choosenPlace_.getName());
        image_.GetComponent<Image>().color = (playerHasVisitThisPlace ? visitedColor_ : notVisitedColor_);
        text_.GetComponent<Text>().text = (playerHasVisitThisPlace ? visitedText_ : notVisitedText_);
        loaded_ = true;
    }
}
