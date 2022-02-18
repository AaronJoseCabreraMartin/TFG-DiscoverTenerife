using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
  * @brief Class that controls the information of the chosen stored place
  * that its shown on the stored place screen.
  */
public class StoredPlaceImageController : MonoBehaviour
{
    /**
      * @brief GameObject that shows the name of the chosen stored place.
      */
    [SerializeField] private GameObject name_;

    /**
      * @brief GameObject that shows the address of the chosen stored place.
      */
    [SerializeField] private GameObject address_;

    /**
      * @brief GameObject that shows the distance from the current user
      * to the chosen stored place.
      */
    [SerializeField] private GameObject distance_;
    
    /**
      * @brief bool that controls if the information of the chosen 
      * stored place was already loaded or not.
      */
    private bool loaded_;
    
    /**
      * @brief This method is called on the first frame. It sets the 
      * loaded_ attribute to false and if the current user has chosen a 
      * stored place it calls the FillFields method.
      */
    void Start(){
        loaded_ = false;
        if(StoredPlacesController.choosenStoredPlace_ != null){
          FillFields();
        }
    }

    /**
      * @brief This method is called once on each frame.
      * It cheks if the current user has chosen a stored place and 
      * the loaded_ property is false, it calls the FillFields method.
      */
    void Update(){
        if(StoredPlacesController.choosenStoredPlace_ != null && !loaded_){
          FillFields();
        }
    }

    /**
      * @brief This method gets all the data of the stored place using the getters
      * and put that information on the gameobjects that shown it. It shows the 
      * distance from the current user to the chosen stored place on the unit that the
      * player has chosen. It also puts the loaded_ attribute as true.
      */
    void FillFields(){
      name_.GetComponent<Text>().text = StoredPlacesController.choosenStoredPlace_.getName();
      address_.GetComponent<Text>().text = StoredPlacesController.choosenStoredPlace_.getAddress();
      optionsController options = optionsController.optionsControllerInstance_;
      distance_.GetComponent<Text>().text = Math.Round(CalculateDistance(),2).ToString() + (options.distanceInKM() ? " kms" : " milles");
      //gameObject.GetComponent<Image>().sprite = StoredPlacesController.choosenStoredPlace_.getImage();
      loaded_ = true;
    }

    /**
      * @return double that contains the distance from the current chosen place to the 
      * current player.
      * @brief This method calculates the distance between the currenr user and the chosen
      * stored place on the unit that the user has chosen.
      */
    private double CalculateDistance(){
      double placeLatitude = StoredPlacesController.choosenStoredPlace_.getLatitude();
      double placeLongitude = StoredPlacesController.choosenStoredPlace_.getLongitude();
      gpsController gps = gpsController.gpsControllerInstance_; 
      return gps.CalculateDistanceToUser(placeLatitude,placeLongitude);
    }

    
}
