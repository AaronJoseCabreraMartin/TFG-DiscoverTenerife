using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceImageController : MonoBehaviour
{
    [SerializeField] private GameObject name_;
    [SerializeField] private GameObject address_;
    [SerializeField] private GameObject distance_;
    
    private bool loaded_;
    // Start is called before the first frame update
    void Start()
    {
        loaded_ = false;
        if(PlaceHandler.choosenPlace_ != null){
          FillFields();
        }
    }

    void Update()
    {
        if(PlaceHandler.choosenPlace_ != null && !loaded_){
          FillFields();
        }
    }

    void FillFields(){
      name_.GetComponent<Text>().text = PlaceHandler.choosenPlace_.getName();
      address_.GetComponent<Text>().text = PlaceHandler.choosenPlace_.getAddress();
      optionsController options = GameObject.FindGameObjectsWithTag("optionsController")[0].GetComponent<optionsController>();
      distance_.GetComponent<Text>().text = Math.Round(CalculateDistance(),2).ToString() + (options.distanceInKM() ? " kms" : " milles");
      gameObject.GetComponent<Image>().sprite = PlaceHandler.choosenPlace_.getImage();
      loaded_ = true;
    }

    private double CalculateDistance(){
      double placeLatitude = PlaceHandler.choosenPlace_.getLatitude();
      double placeLongitude = PlaceHandler.choosenPlace_.getLongitude();
      gpsController gps = GameObject.FindGameObjectsWithTag("gpsController")[0].GetComponent<gpsController>(); 
      return gps.CalculateDistanceToUser(placeLatitude,placeLongitude);
    }

    
}
