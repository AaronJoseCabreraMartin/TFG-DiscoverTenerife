using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceImageController : MonoBehaviour
{
    private bool loaded_ = false;
    // Start is called before the first frame update
    void Start()
    {
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
      gameObject.transform.Find("Name").gameObject.GetComponent<Text>().text = PlaceHandler.choosenPlace_.getName();
      gameObject.transform.Find("Address").gameObject.GetComponent<Text>().text = PlaceHandler.choosenPlace_.getAddress();
      optionsController options = GameObject.FindGameObjectsWithTag("optionsController")[0].GetComponent<optionsController>();
      gameObject.transform.Find("Distance").gameObject.GetComponent<Text>().text = Math.Round(CalculateDistance()).ToString() + (options.distanceInKM() ? " kms" : " milles");
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
