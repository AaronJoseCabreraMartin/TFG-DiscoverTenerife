using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controls the panels that shows the information of 
  * a visited place on the story panel.
  */
public class StoryPlace : MonoBehaviour
{
    /**
      * @brief Reference to the GameObject that shows the name of the place
      */
    [SerializeField] private GameObject name_;

    /**
      * @brief Reference to the GameObject that shows the type of the place
      */
    [SerializeField] private GameObject type_;

    /**
      * @brief Reference to the GameObject that shows the date of the
      * last visit to that place.
      */
    [SerializeField] private GameObject dateOfTheVisit_;

    /**
      * @brief Reference to the GameObject that shows the times that
      * a place was visited.
      */
    [SerializeField] private GameObject timesVisited_;

    /**
      * @param VisitedPlace a reference to the represented visited place.
      * @brief This method update all the text that are showed to show the 
      * information of the given place.
      */
    public void setData(VisitedPlace visitedPlace){
        Dictionary<string,string> placeData = firebaseHandler.firebaseHandlerInstance_.getPlaceData(visitedPlace.type_, visitedPlace.id_.ToString());
        name_.GetComponent<Text>().text = placeData["name_"];
        type_.GetComponent<Text>().text = "Type: " + visitedPlace.type_;
        dateOfTheVisit_.GetComponent<Text>().text = "Last Visit: " + (new DateTime(visitedPlace.lastVisitTimestamp_)).ToString();
        timesVisited_.GetComponent<Text>().text = "Times visited: " + visitedPlace.timesVisited_.ToString();
    }
}
