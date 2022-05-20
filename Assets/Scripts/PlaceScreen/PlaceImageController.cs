using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief Class that controls the panel that shows the information of the chosen place on the
  * place selected screen.
  */
public class PlaceImageController : MonoBehaviour
{
    /**
      * @brief Reference to the GameObject that shows the name of the chosen place.
      */
    [SerializeField] private GameObject name_;
    
    /**
      * @brief Reference to the GameObject that shows the address of the chosen place.
      */
    [SerializeField] private GameObject address_;

    /**
      * @brief Reference to the GameObject that shows the distance between the current
      * user and the chosen place.
      */
    [SerializeField] private GameObject distance_;

    /**
      * @brief GameObject with the uninteractuable slider bar that represent how much visited
      * is the represented place
      */    
    [SerializeField] private GameObject sliderPercentajeOfVisits_;

    /**
      * @brief Image that is on the sliderPercentajeOfVisits_ gameObject, it changes its color
      * proporcionally from green to red with the percentaje of visits of the represented place
      */
    [SerializeField] private Image FillPercentajeOfVisits_;
    
    /**
      * @brief Bolean value that sais if the place information was already loaded.
      */
    private bool loaded_;

    /**
      * @brief This method is called on the first frame, it sets the loaded_ attribute to
      * true and if the chosenPlace_ property of PlaceHandler isnt null it calls the FillFields method.
      */
    void Start(){
        loaded_ = false;
        if(PlaceHandler.chosenPlace_ != null){
          FillFields();
        }
    }

    /**
      * @brief This method is called on each frame, if loaded_ attribute is false and 
      * if the chosenPlace_ property of PlaceHandler isnt null it calls the FillFields method.
      */
    void Update(){
        if(PlaceHandler.chosenPlace_ != null && !loaded_){
          FillFields();
        }
    }

    /**
      * @brief This method change the text of all the gameobjects that shown the 
      * current place characteristics. It ask those values calling the getters of 
      * PlaceHandler. To calculate the distance to the user, it calls the
      * CalculateDistance method. It takes aware of the options that the user has 
      * chosen to show the distance to the place. It also sets the loaded_ property to true.
      */
    void FillFields(){
      name_.GetComponent<Text>().text = PlaceHandler.chosenPlace_.getName();
      address_.GetComponent<Text>().text = PlaceHandler.chosenPlace_.getAddress();
      optionsController options = optionsController.optionsControllerInstance_;
      distance_.GetComponent<Text>().text = Math.Round(CalculateDistance(),2).ToString() + (options.distanceInKM() ? " kms" : " milles");
      gameObject.GetComponent<Image>().sprite = PlaceHandler.chosenPlace_.getImage();

      //Le sumo 1 a ambos para que no pueda dar n/0
      float visitsOfPlace = (float)(PlaceHandler.chosenPlace_.getTimesItHasBeenVisited()+1);
      float visitsOfMostVisitedPlace = (float)(firebaseHandler.firebaseHandlerInstance_.requestHandler_.visitsOfMostVisitedPlace()+1);
      float percentaje = visitsOfPlace/visitsOfMostVisitedPlace;
      sliderPercentajeOfVisits_.GetComponent<Slider>().value = percentaje;

      FillPercentajeOfVisits_.color = new Color32((byte)(percentaje*255f), (byte)((1-percentaje)*255f), (byte)0,(byte)255);

      loaded_ = true;
    }

    /**
      * @return double with the distance from the chosen place to the current user.
      * @brief This method returns the distance from the chosen place to the
      * current user on the chosen unit. 
      */
    private double CalculateDistance(){
      double placeLatitude = PlaceHandler.chosenPlace_.getLatitude();
      double placeLongitude = PlaceHandler.chosenPlace_.getLongitude();
      gpsController gps = gpsController.gpsControllerInstance_; 
      return gps.CalculateDistanceToUser(placeLatitude,placeLongitude);
    }

    
}
