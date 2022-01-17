using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoredPlaceImageController : MonoBehaviour
{
    [SerializeField] private GameObject name_;
    [SerializeField] private GameObject address_;
    [SerializeField] private GameObject distance_;
    
    private bool loaded_;
    // Start is called before the first frame update
    void Start()
    {
        loaded_ = false;
        if(StoredPlacesController.choosenStoredPlace_ != null){
          FillFields();
        }
    }

    void Update()
    {
        if(StoredPlacesController.choosenStoredPlace_ != null && !loaded_){
          FillFields();
        }
    }

    void FillFields(){
      name_.GetComponent<Text>().text = StoredPlacesController.choosenStoredPlace_.getName();
      address_.GetComponent<Text>().text = StoredPlacesController.choosenStoredPlace_.getAddress();
      optionsController options = optionsController.optionsControllerInstance_;
      distance_.GetComponent<Text>().text = Math.Round(CalculateDistance(),2).ToString() + (options.distanceInKM() ? " kms" : " milles");
      //gameObject.GetComponent<Image>().sprite = StoredPlacesController.choosenStoredPlace_.getImage();
      loaded_ = true;
    }

    private double CalculateDistance(){
      double placeLatitude = StoredPlacesController.choosenStoredPlace_.getLatitude();
      double placeLongitude = StoredPlacesController.choosenStoredPlace_.getLongitude();
      gpsController gps = gpsController.gpsControllerInstance_; 
      return gps.CalculateDistanceToUser(placeLatitude,placeLongitude);
    }

    
}
