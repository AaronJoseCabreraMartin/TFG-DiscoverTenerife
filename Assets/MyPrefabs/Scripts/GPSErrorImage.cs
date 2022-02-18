using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
  * @brief This class controls the error image that show that there is not
  * GPS service.
  */
public class GPSErrorImage : MonoBehaviour
{
    /**
      * @brief GameObject that contains the background image of the error.
      */
    [SerializeField] private GameObject image_;

    /**
      * @brief GameObject that contains the text of the error.
      */
    [SerializeField] private GameObject text_;

    /**
      * @brief This method is called on each frame. It shows the background
      * image and the text of the error if the gps is not running.
      */
    void Update(){
        image_.SetActive(!gpsController.gpsControllerInstance_.gpsIsRunning());
        text_.SetActive(!gpsController.gpsControllerInstance_.gpsIsRunning());
    }
}
