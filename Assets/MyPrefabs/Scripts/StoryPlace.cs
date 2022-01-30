using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryPlace : MonoBehaviour
{
    [SerializeField] private GameObject name_;
    [SerializeField] private GameObject type_;
    [SerializeField] private GameObject dateOfTheVisit_;
    [SerializeField] private GameObject timesVisited_;

    public void setData(VisitedPlace visitedPlace){
        Dictionary<string,string> placeData = firebaseHandler.firebaseHandlerInstance_.getPlaceData(visitedPlace.type_, visitedPlace.id_.ToString());
        name_.GetComponent<Text>().text = placeData["name_"];
        type_.GetComponent<Text>().text = "Type: " + visitedPlace.type_;
        dateOfTheVisit_.GetComponent<Text>().text = "Last Visit: " + (new DateTime(visitedPlace.lastVisitTimestamp_)).ToString();
        timesVisited_.GetComponent<Text>().text = "Times visited: " + visitedPlace.timesVisited_.ToString();
    }
}
