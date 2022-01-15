using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSErrorImage : MonoBehaviour
{
    [SerializeField] private GameObject image_;
    [SerializeField] private GameObject text_;

    // Update is called once per frame
    void Update(){
        image_.SetActive(!gpsController.gpsControllerInstance_.gpsIsRunning());
        text_.SetActive(!gpsController.gpsControllerInstance_.gpsIsRunning());
    }
}
