using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controls the bar that shows the whole island score of the current user.
  */
public class ScoreOfTheWholeIslandController : MonoBehaviour
{
    /**
      * @brief GameObject that has the text where the scored will be shown.
      */
    [SerializeField] private GameObject text_;

    /**
      * @brief boolean value, its true if the score was already downloaded and false in other case. 
      */
    private bool scoreLoaded_ = false;

    /**
      * @brief reference to the firebaseHandler instance.
      */
    private firebaseHandler firebaseHandlerObject_;

    /**
      * @brief this method is called on the first frame, if both placesAreReady and userDataIsReady
      * methods of firebaseHandler class return true it calls the fillScore method.
      */
    void Start(){
    // Start is called before the first frame update
        firebaseHandlerObject_ = firebaseHandler.firebaseHandlerInstance_;
        if(firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            fillScore();
        }
    }

    /**
      * @brief this method is called on each frame, if both placesAreReady and userDataIsReady
      * methods of firebaseHandler class return true and scoreLoaded_ property is false,
      * it calls the fillScore method.
      */
    void Update(){
        if(!scoreLoaded_ && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            fillScore();
        }
    }

    /**
      * @brief this method calculates the proportion of places of the whole island that
      * the current user has visited and it sets the text and the value of the slider
      * to show that proportion. It also sets the scoreLoaded_ attribute to true.
      */
    private void fillScore(){
        float proportionOfVisited = (float)firebaseHandlerObject_.actualUser_.countOfVisitedPlaces()/firebaseHandlerObject_.totalOfPlaces();
        text_.GetComponent<Text>().text = "You have visited " + Math.Round(proportionOfVisited*100,2) + "% of the island";
        gameObject.GetComponent<Slider>().value = proportionOfVisited;
        scoreLoaded_ = true;
    }
}
