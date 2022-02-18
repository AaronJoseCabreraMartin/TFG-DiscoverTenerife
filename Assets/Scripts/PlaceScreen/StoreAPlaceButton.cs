using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  * @brief Class that controls the button that allows the user store a place to visit it
  * as a internetless visit.
  */
public class StoreAPlaceButton : MonoBehaviour
{
    /**
      * @brief GameObject that has the toast message that will show the information
      * to the user.
      */
    [SerializeField] private GameObject toastMessageObject_;

    /**
      * @brief This method should be called when the user press the "store this place
      * for an internetless visit" button.
      * - If the user has already stored this place, it shows a toast saying "You already 
      * have this place stored!"
      * - If the user has space for store one more place and it doesnt have the current
      * chosen place stored, this calls storePlace method of StoredPlace class and makes
      * a toast message saying that the place was stored successfully.
      * - If the user hasnt got any space for store one more place it shows a toast
      * message saying "Error, you cant store more places!"
      */
    public void OnClick(){
        toastMessage toastMessageInstance = toastMessageObject_.GetComponent<toastMessage>();
        if(StoredPlace.isPlaceStoredByName(PlaceHandler.choosenPlace_.getName())){
            toastMessageInstance.makeAnimation("You already have this place stored!", new Color32(255,145,15,255), 5);
        }else if(StoredPlace.thereIsSpaceForOtherPlaceStored()){
            StoredPlace.storePlace(PlaceHandler.choosenPlace_);
            toastMessageInstance.makeAnimation("You stored this place successfully", new Color32(76,175,80,255), 5);
        }else{
            toastMessageInstance.makeAnimation("Error, you cant store more places!", new Color32(255,0,0,255), 5);   
        }
    }
}
