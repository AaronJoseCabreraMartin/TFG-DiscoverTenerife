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
      gameObject.transform.Find("Distance").gameObject.GetComponent<Text>().text = Math.Round(CalculateDistance()).ToString() + " kms";
      gameObject.GetComponent<Image>().sprite = PlaceHandler.choosenPlace_.getImage();
      loaded_ = true;
    }

    private double CalculateDistance(){
      double placeLatitude = sexagecimalToRadian(PlaceHandler.choosenPlace_.getLatitude());
      double placeLongitude = sexagecimalToRadian(PlaceHandler.choosenPlace_.getLongitude());
      gpsController gps = GameObject.FindGameObjectsWithTag("gpsController")[0].GetComponent<gpsController>(); 
      double userLatitude = sexagecimalToRadian(gps.getLatitude());
      double userLongitude = sexagecimalToRadian(gps.getLongitude());

      //Debug.Log($"placeLatitude = {placeLatitude} placeLongitude = {placeLongitude}");
      //Debug.Log($"userLatitude = {userLatitude} userLongitude = {userLongitude}");
      
      double earthRadious = 6377.830272;

      /*
      if option in milles
        return same*0,621371;
      */
      return earthRadious*Math.Acos((Math.Sin(placeLatitude) * Math.Sin(userLatitude)) + 
                                     Math.Cos(placeLatitude) * Math.Cos(userLatitude) * Math.Cos(userLongitude - placeLongitude));
    }

    private double sexagecimalToRadian(double sexagecimal) {
      return sexagecimal * (Math.PI/180);
    }
}
