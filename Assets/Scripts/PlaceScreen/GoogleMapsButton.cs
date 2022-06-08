using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that handles the button that open the Google Maps aplication with the correspondent coordenades.
  */
public class GoogleMapsButton : MonoBehaviour
{
    /**
      * @brief Method that open Google Maps aplication on the chosen place
      */
    public void OpenURLFromChosenPlace(){
        Application.OpenURL("https://www.google.com/maps/search/?api=1&query="
                            + PlaceHandler.chosenPlace_.getLatitude().ToString().Replace(",",".")
                            + ","
                            + PlaceHandler.chosenPlace_.getLongitude().ToString().Replace(",",".") );
    }

    /**
      * @brief Method that open Google Maps aplication on the chosen stored place
      */
    public void OpenURLFromChosenStoredPlace(){
        Application.OpenURL("https://www.google.com/maps/search/?api=1&query="
                            + StoredPlacesController.choosenStoredPlace_.getLatitude().ToString().Replace(",",".")
                            + ","
                            + StoredPlacesController.choosenStoredPlace_.getLongitude().ToString().Replace(",",".") );
    }
}
