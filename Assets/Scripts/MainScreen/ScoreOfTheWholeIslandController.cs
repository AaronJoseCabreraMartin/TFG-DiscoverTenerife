using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreOfTheWholeIslandController : MonoBehaviour
{
    [SerializeField] private GameObject text_;
    private bool scoreLoaded_ = false;
    private firebaseHandler firebaseHandlerObject_;
    // Start is called before the first frame update
    void Start()
    {
        firebaseHandlerObject_ = GameObject.FindGameObjectsWithTag("firebaseHandler")[0].GetComponent<firebaseHandler>();
        if(firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            fillScore();
        }
    }

    void Update(){
        if(!scoreLoaded_ && firebaseHandlerObject_.placesAreReady() && firebaseHandlerObject_.userDataIsReady()){
            fillScore();
        }
    }

    private void fillScore(){
        float proportionOfVisited = (float)firebaseHandlerObject_.actualUser_.coutOfVisitedPlaces()/firebaseHandlerObject_.totalOfPlaces();
        text_.GetComponent<Text>().text = "You have visited " + Math.Round(proportionOfVisited*100,2) + "% of the island";
        gameObject.GetComponent<Slider>().value = proportionOfVisited;
        scoreLoaded_ = true;
    }
}
